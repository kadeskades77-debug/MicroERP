using Domin.Entities;
using MicroERP.Application.Common.DTOs;
using MicroERP.Application.Common.Exceptions;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Mappings;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.Auth.DTOs;
using MicroERP.Application.Features.Auth.Interfaces;
using MicroERP.Application.Features.Departments.Interfaces;
using MicroERP.Application.Features.EmployeeLeaveBalances.Interfaces;
using MicroERP.Application.Features.Employees.DTOs;
using MicroERP.Application.Features.Employees.Interfaces;
using MicroERP.Domain.Audit;
using MicroERP.Domin.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace MicroERP.Application.Features.Employees.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IApplicationDbContext _context;
        private readonly IdentityService _authService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditService _auditService;
        private readonly IEmployeeQueries _employeeQueries;
        private readonly IDepartmentQueries _departmentQueries;
        private readonly ILeaveBalanceGenerator _leaveBalanceGenerator;

        public EmployeeService(
           IApplicationDbContext context,
           IdentityService authService,
           IUnitOfWork unitOfWork,
           IAuditService auditService,
           IEmployeeQueries employeeQueries,
           IDepartmentQueries departmentQueries,
           ILeaveBalanceGenerator leaveBalanceGenerator)
        {
            _context = context;
            _authService = authService;
            _unitOfWork = unitOfWork;
            _auditService = auditService;
            _employeeQueries = employeeQueries;
            _departmentQueries = departmentQueries;
            _leaveBalanceGenerator = leaveBalanceGenerator;
        }

        public async Task<CreateEmployeeResultDto> CreateAsync(CreateEmployeeDto dto,
            CancellationToken cancellationToken = default)
        {
            dto.FullName = dto.FullName.Trim();
            dto.Phone = dto.Phone.Trim();

            if (!string.IsNullOrWhiteSpace(dto.Email))
                dto.Email = dto.Email.Trim().ToLower();


            if (dto.Salary <= 0)
                throw new BusinessException(
                    "Salary must be greater than zero.");


            var department = await _departmentQueries
                .GetByIdAsync(dto.DepartmentId);


            if (department is null)
                throw new NotFoundException(
                    "Department not found.");


            var phoneExists = await _employeeQueries
                .PhoneExistsAsync(dto.Phone);


            if (phoneExists)
                throw new BusinessException(
                    "Phone already exists.");


            var userResult = await _authService.CreateEmployeeUserAsync(
                new CreateEmployeeUserDto
                {
                    FullName = dto.FullName,
                    Email = dto.Email
                });


            if (!userResult.Success || userResult.Data is null)
                throw new BusinessException(
                    userResult.Message);


            try
            {
                return await _unitOfWork.ExecuteAsync(async () =>
                {
                    var employee = new Employee
                    {
                        Phone = dto.Phone,
                        Salary = dto.Salary,
                        DepartmentId = dto.DepartmentId,
                        UserId = userResult.Data.UserId
                    };


                    _context.Employees.Add(employee);


                    var employeeGroup =
                        await _context.PermissionGroups
                            .FirstOrDefaultAsync(x =>
                                x.Key == "Employee");


                    if (employeeGroup is null)
                        throw new BusinessException(
                            "Employee permission group not found.");


                    var assignmentExists =
                        await _context.UserPermissionAssignments
                            .AnyAsync(x =>
                                x.UserId == userResult.Data.UserId &&
                                x.PermissionGroupId == employeeGroup.Id);


                    if (!assignmentExists)
                    {
                        _context.UserPermissionAssignments.Add(
                            new UserPermissionAssignment
                            {
                                UserId = userResult.Data.UserId,
                                PermissionGroupId = employeeGroup.Id
                            });
                    }
            
                    await _context.SaveChangesAsync();


                    await _auditService.LogAsync(
                        AuditActions.Create,
                        nameof(Employee),
                        employee.Id.ToString(),
                        null,
                        new
                        {
                            employee.Phone,
                            employee.Salary,
                            employee.DepartmentId,
                            employee.UserId
                        });


                    await _context.SaveChangesAsync();
                    await _leaveBalanceGenerator
           .GenerateForEmployeeAsync(employee.Id, DateTime.UtcNow.Year, cancellationToken);


                    return new CreateEmployeeResultDto
                    {
                        EmployeeId = employee.Id,
                        UserName = userResult.Data.UserName,
                        GeneratedPassword = userResult.Data.Password
                    };

                });
            }
            catch
            {
                await _authService.DeleteUserAsync(
                    userResult.Data.UserId);

                throw;
            }
        }
        public async Task<Result<List<EmployeeListDto>>> GetAllAsync()
        {
            var data = await _employeeQueries.GetAllAsync();

            return Result<List<EmployeeListDto>>.Succeeded(data);
        }
        public async Task<EmployeeDto> GetByIdAsync(int id)
        {
            var employee =
                await _employeeQueries
                    .GetByIdWithUserAndDepartmentAsync(id);


            if (employee is null)
                throw new NotFoundException(
                    "Employee not found.");
            return employee.ToDto();

            //  return new EmployeeDto
            //{
            //    Id = employee.Id,
            //    FullName = employee.User.FullName,
            //    UserName = employee.User.UserName!,
            //    Email = employee.User.Email,
            //    Phone = employee.Phone,
            //    Salary = employee.Salary,
            //    DepartmentCode = employee.Department.Code,
            //    DepartmentName = employee.Department.NameEn,
            //    IsActive = employee.IsActive
            //};
        }
        public async Task<Result> TransferEmployeeAsync(int employeeId,TransferEmployeeDto dto)
        {
            return await _unitOfWork.ExecuteAsync(async () =>
            {
                var employee =
                    await _employeeQueries.GetByIdAsync(employeeId);


                if (employee is null)
                    return Result.Failure(
                        "Employee not found.");


                var newDepartment =
                    await _departmentQueries.GetByIdAsync(
                        dto.DepartmentId);


                if (newDepartment is null)
                    return Result.Failure(
                        "Department not found.");


                if (employee.DepartmentId == dto.DepartmentId)
                    return Result.Failure(
                        "Employee already belongs to this department.");


                var oldDepartmentId = employee.DepartmentId;


                var oldDepartment =
                    await _departmentQueries.GetByIdAsync(
                        oldDepartmentId);


                var removedAsManager = false;


                if (oldDepartment != null &&
                    oldDepartment.ManagerEmployeeId == employee.Id)
                {
                    oldDepartment.ManagerEmployeeId = null;
                    oldDepartment.HasManager = false;

                    removedAsManager = true;
                }


                employee.DepartmentId = dto.DepartmentId;


                await _auditService.LogAsync(
                    AuditActions.Update,
                    nameof(Employee),
                    employee.Id.ToString(),
                    new
                    {
                        DepartmentId = oldDepartmentId
                    },
                    new
                    {
                        employee.DepartmentId,
                        RemovedAsManager = removedAsManager
                    });


                await _context.SaveChangesAsync();


                return Result.Succeeded(
                    "Employee transferred successfully.");
            });
        }
        public async Task<Result> UpdateSalaryAsync(int employeeId,UpdateEmployeeSalaryDto dto)
        {
            return await _unitOfWork.ExecuteAsync(async () =>
            {
                if (dto.Salary <= 0)
                    return Result.Failure(
                        "Salary must be greater than zero.");

                var employee = await _employeeQueries.GetByIdAsync(employeeId);

                if (employee is null)
                    return Result.Failure(
                        "Employee not found.");

                if (employee.Salary == dto.Salary)
                    return Result.Failure(
                        "The new salary is the same as the current salary.");

                var oldValues = new
                {
                    employee.Salary
                };

                employee.Salary = dto.Salary;

                await _auditService.LogAsync(
                    AuditActions.Update,
                    nameof(Employee),
                    employee.Id.ToString(),
                    oldValues,
                    new
                    {
                        employee.Salary
                    });
                await _context.SaveChangesAsync();
                return Result.Succeeded(
                    "Employee salary updated successfully.");
            });
        }
        public async Task<Result> UpdateAsync(int id, UpdateEmployeeDto dto)
        {
            var employee =
       await _employeeQueries.GetByIdWithUserAsync(id);

            if (employee is null)
                throw new NotFoundException("Employee not found.");

            var oldValues = new
            {
                employee.User.FullName,
                employee.User.Email,
                employee.Phone
            };

            if (!string.IsNullOrWhiteSpace(dto.Phone))
            {
                dto.Phone = dto.Phone.Trim();

                var phoneExists =
                 await _employeeQueries.PhoneExistsAsync(
               dto.Phone,
               id);

                if (phoneExists)
                    throw new BusinessException("Phone already exists.");

                employee.Phone = dto.Phone;
            }

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var result = await _authService.ChangeEmailAsync(
                    new ChangeEmailDto
                    {
                        UserId = employee.UserId,
                        Email = dto.Email.Trim().ToLower()
                    });

                if (!result.Success)
                    throw new BusinessException(result.Message);
            }
            if (!string.IsNullOrWhiteSpace(dto.FullName)) 
                
            {
                var authResult = await _authService.UpdateEmployeeUserAsync(
                    employee.UserId,
                    dto.FullName);

                if (!authResult.Success)
                    throw new BusinessException(authResult.Message);
            }

            await _auditService.LogAsync(
                AuditActions.Update,
                nameof(Employee),
                employee.Id.ToString(),
                oldValues,
                new
                {
                    FullName = employee.User.FullName,
                    Email = employee.User.Email,
                    employee.Phone
                });

            await _context.SaveChangesAsync();

            return Result.Succeeded(
                "Employee updated successfully.");
        }
        public async Task DeleteAsync(int id)
        {
            var employee =
                await _employeeQueries.GetByIdAsync(id);


            if (employee is null)
                throw new NotFoundException(
                    "Employee not found.");


            var result =
                await _authService.DeactivateUserAsync(
                    employee.UserId);


            if (!result.Success)
                throw new BusinessException(
                    result.Message);


            var oldValues = new
            {
                employee.IsDeleted
            };


            var managedDepartment =
                await _departmentQueries
                    .GetByManagerIdAsync(employee.Id);


            if (managedDepartment is not null)
            {
                managedDepartment.ManagerEmployeeId = null;
                managedDepartment.HasManager = false;
            }


            employee.IsDeleted = true;
            employee.IsActive = false;


            await _auditService.LogAsync(
                AuditActions.Delete,
                nameof(Employee),
                employee.Id.ToString(),
                oldValues,
                new
                {
                    employee.IsDeleted
                });


            await _context.SaveChangesAsync();
        }
        public async Task RestoreAsync(int id)
        {
            var employee =
                await _employeeQueries.GetDeletedByIdAsync(id);


            if (employee is null)
                throw new NotFoundException(
                    "Employee not found.");


            if (!employee.IsDeleted)
                throw new BusinessException(
                    "Employee is already active.");


            var oldValues = new
            {
                employee.IsDeleted,
                employee.IsActive
            };


            employee.IsDeleted = false;
            employee.IsActive = true;


            await _auditService.LogAsync(
                AuditActions.Restore,
                nameof(Employee),
                employee.Id.ToString(),
                oldValues,
                new
                {
                    employee.IsDeleted,
                    employee.IsActive
                });


            await _context.SaveChangesAsync();
        }
        public async Task ActivateAsync(int id)
        {
            var employee =
                await _employeeQueries.GetByIdAsync(id);


            if (employee is null)
                throw new NotFoundException(
                    "Employee not found.");


            var result =
                await _authService.ActivateUserAsync(
                    employee.UserId);


            if (!result.Success)
                throw new BusinessException(
                    result.Message);


            employee.IsActive = true;


            await _context.SaveChangesAsync();
        }
        public async Task DeactivateAsync(int id)
        {
            var employee =
                await _employeeQueries.GetByIdAsync(id);


            if (employee is null)
                throw new NotFoundException(
                    "Employee not found.");


            var result =
                await _authService.DeactivateUserAsync(
                    employee.UserId);


            if (!result.Success)
                throw new BusinessException(
                    result.Message);


            employee.IsActive = false;


            await _context.SaveChangesAsync();
        }
        public async Task<List<LookupDto>> GetLookupAsync()
        {
            return await _context.Employees
                .AsNoTracking()
                .Include(x => x.User)
                .OrderBy(x => x.User.FullName)
                .Select(x => new LookupDto
                {
                    Value = x.Id.ToString(),
                    Text = x.User.FullName
                })
                .ToListAsync();
        }
    }
}
