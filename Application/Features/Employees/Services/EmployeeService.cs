using MicroERP.Application.Common.DTOs;
using MicroERP.Application.Common.Exceptions;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Mappings;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.Authorization.Auth.DTOs;
using MicroERP.Application.Features.Authorization.Auth.Interfaces;
using MicroERP.Application.Features.Departments.Interfaces;
using MicroERP.Application.Features.Documents.EmployeeDocuments.Interfaces;
using MicroERP.Application.Features.Employees.DTOs;
using MicroERP.Application.Features.Employees.Interfaces;
using MicroERP.Application.Features.Leaves.EmployeeLeaveBalances.Interfaces;
using MicroERP.Domain.Audit;
using MicroERP.Domin.Entities.Employees;
using MicroERP.Domin.Enums;
using MicroERP.Domin.Identity;
using Microsoft.EntityFrameworkCore;


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
        private readonly IEmployeeDocumentService _employeeDocumentService;

        public EmployeeService(
           IApplicationDbContext context,
           IdentityService authService,
           IUnitOfWork unitOfWork,
           IAuditService auditService,
           IEmployeeQueries employeeQueries,
           IDepartmentQueries departmentQueries,
           ILeaveBalanceGenerator leaveBalanceGenerator,
           IEmployeeDocumentService employeeDocumentService)
        {
            _context = context;
            _authService = authService;
            _unitOfWork = unitOfWork;
            _auditService = auditService;
            _employeeQueries = employeeQueries;
            _departmentQueries = departmentQueries;
            _leaveBalanceGenerator = leaveBalanceGenerator;
            _employeeDocumentService = employeeDocumentService;
        }


        public async Task<Result<CreateEmployeeResultDto>> CreateAsync(CreateEmployeeDto dto,
        CancellationToken cancellationToken = default)
        {
            string? createdUserId = null;

            try
            {
                dto.FullName = dto.FullName.Trim();
                dto.Phone = dto.Phone.Trim();

                if (!string.IsNullOrWhiteSpace(dto.Email))
                    dto.Email = dto.Email.Trim().ToLower();

                if (dto.Salary <= 0)
                    return Result<CreateEmployeeResultDto>.Failure(
                        "Salary must be greater than zero.");

                var department =
                    await _departmentQueries
                        .GetByIdAsync(dto.DepartmentId);

                if (department is null)
                    return Result<CreateEmployeeResultDto>.Failure(
                        "Department not found.");

                var phoneExists =
                    await _employeeQueries
                        .PhoneExistsAsync(dto.Phone);

                if (phoneExists)
                    return Result<CreateEmployeeResultDto>.Failure(
                        "Phone already exists.");

                var userResult =
                    await _authService.CreateEmployeeUserAsync(
                        new CreateEmployeeUserDto
                        {
                            FullName = dto.FullName,
                            Email = dto.Email
                        });

                if (!userResult.Success ||
                    userResult.Data is null)
                {
                    return Result<CreateEmployeeResultDto>.Failure(
                        string.IsNullOrWhiteSpace(userResult.Message)
                            ? "Failed to create employee user."
                            : userResult.Message);
                }

                createdUserId = userResult.Data.UserId;

                var result =
                    await _unitOfWork.ExecuteAsync(async () =>
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
                                .FirstOrDefaultAsync(
                                    x => x.Key == "Employee",
                                    cancellationToken);

                        if (employeeGroup is null)
                            return Result<CreateEmployeeResultDto>.Failure(
                                "Employee permission group not found.");

                        var assignmentExists =
                            await _context.UserPermissionAssignments
                                .AnyAsync(
                                    x =>
                                        x.UserId ==
                                            userResult.Data.UserId &&
                                        x.PermissionGroupId ==
                                            employeeGroup.Id,
                                    cancellationToken);

                        if (!assignmentExists)
                        {
                            _context.UserPermissionAssignments.Add(
                                new UserPermissionAssignment
                                {
                                    UserId = userResult.Data.UserId,
                                    PermissionGroupId =
                                        employeeGroup.Id
                                });
                        }

                        await _context.SaveChangesAsync(
                            cancellationToken);

                        await _leaveBalanceGenerator
                            .GenerateForEmployeeAsync(
                                employee.Id,
                                DateTime.UtcNow.Year,
                                cancellationToken);

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

                        return Result<CreateEmployeeResultDto>.Succeeded(
                            new CreateEmployeeResultDto
                            {
                                EmployeeId = employee.Id,
                                UserName =
                                    userResult.Data.UserName,
                                GeneratedPassword =
                                    userResult.Data.Password
                            },
                            "Employee created successfully.");
                    });

                if (!result.Success)
                {
                    if (!string.IsNullOrWhiteSpace(createdUserId))
                        await _authService.DeleteUserAsync(
                            createdUserId);

                    return result;
                }

                return result;
            }
            catch (Exception)
            {
                if (!string.IsNullOrWhiteSpace(createdUserId))
                {
                    try
                    {
                        await _authService.DeleteUserAsync(
                            createdUserId);
                    }
                    catch
                    {
                        // Do not replace the original failure.
                    }
                }

                return Result<CreateEmployeeResultDto>.Failure(
                    "An unexpected error occurred.");
            }
        }

        public async Task<Result<List<EmployeeListDto>>> GetAllAsync()
        {
            var data = await _employeeQueries.GetAllAsync();

            return Result<List<EmployeeListDto>>.Succeeded(data);
        }

        public async Task<Result<EmployeeDto>> GetByIdAsync(int id,
        CancellationToken cancellationToken = default)
        {
            try
            {
                var employee =
                    await _employeeQueries
                        .GetByIdWithUserAndDepartmentAsync(id);

                if (employee is null)
                {
                    return Result<EmployeeDto>.Failure(
                        "Employee not found.");
                }

                return Result<EmployeeDto>.Succeeded(
                    employee.ToDto());
            }
            catch (Exception)
            {
                return Result<EmployeeDto>.Failure(
                    "An unexpected error occurred.");
            }
        }

        public async Task<Result<List<EmployeeListDto>>> GetDeletedAsync()
        {
            var data = await _employeeQueries.GetDeletedAsync();

            return Result<List<EmployeeListDto>>.Succeeded(data);
        }

        public async Task<Result> TransferEmployeeAsync(int employeeId,
        TransferEmployeeDto dto,
        CancellationToken cancellationToken = default)
        {
            try
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

                    var oldDepartmentId =
                        employee.DepartmentId;

                    var oldDepartment =
                        await _departmentQueries.GetByIdAsync(
                            oldDepartmentId);

                    var removedAsManager = false;

                    if (oldDepartment is not null &&
                        oldDepartment.ManagerEmployeeId == employee.Id)
                    {
                        oldDepartment.ManagerEmployeeId = null;
                        oldDepartment.HasManager = false;

                        removedAsManager = true;
                    }

                    employee.DepartmentId =
                        dto.DepartmentId;

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

                    await _context.SaveChangesAsync(
                        cancellationToken);

                    return Result.Succeeded(
                        "Employee transferred successfully.");
                });
            }
            catch (Exception)
            {
                return Result.Failure(
                    "An unexpected error occurred.");
            }
        }


        public async Task<Result> UpdateSalaryAsync(int employeeId,
        UpdateEmployeeSalaryDto dto,
        CancellationToken cancellationToken = default)
        {
            try
            {
                return await _unitOfWork.ExecuteAsync(async () =>
                {
                    if (dto.Salary <= 0)
                        return Result.Failure(
                            "Salary must be greater than zero.");

                    var employee =
                        await _employeeQueries
                            .GetByIdAsync(employeeId);

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

                    employee.Salary =
                        dto.Salary;

                    await _context.SaveChangesAsync(
                        cancellationToken);

                    await _auditService.LogAsync(
                        AuditActions.Update,
                        nameof(Employee),
                        employee.Id.ToString(),
                        oldValues,
                        new
                        {
                            employee.Salary
                        });

                    return Result.Succeeded(
                        "Employee salary updated successfully.");
                });
            }
            catch (Exception)
            {
                return Result.Failure(
                    "An unexpected error occurred.");
            }
        }


        public async Task<Result> UpdateAsync(int id,
        UpdateEmployeeDto dto,
        CancellationToken cancellationToken = default)
        {
            try
            {
                var employee =
                    await _employeeQueries
                        .GetByIdWithUserAsync(id);

                if (employee is null)
                    return Result.Failure(
                        "Employee not found.");

                var oldValues = new
                {
                    FullName = employee.User.FullName,
                    Email = employee.User.Email,
                    employee.Phone
                };

              var fullName =
    string.IsNullOrWhiteSpace(dto.FullName)
        ? employee.User.FullName
        : dto.FullName.Trim();

     var email =
    string.IsNullOrWhiteSpace(dto.Email)
        ? employee.User.Email
        : dto.Email.Trim().ToLower();

    var phone =
    string.IsNullOrWhiteSpace(dto.Phone)
        ? employee.Phone
        : dto.Phone.Trim();

                if (!string.Equals(
                        phone,
                        employee.Phone,
                        StringComparison.Ordinal))
                {
                    var phoneExists =
                        await _employeeQueries
                            .PhoneExistsAsync(
                                phone,
                                id);

                    if (phoneExists)
                        return Result.Failure(
                            "Phone already exists.");
                }

                if (!string.Equals(
              email,
              employee.User.Email,
              StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(email))
                    {
                        return Result.Failure(
                            "Employee email cannot be empty.");
                    }

                    var result =
                        await _authService.ChangeEmailAsync(
                            new ChangeEmailDto
                            {
                                UserId = employee.UserId,
                                Email = email
                            });

                    if (!result.Success)
                        return Result.Failure(
                            string.IsNullOrWhiteSpace(result.Message)
                                ? "Failed to update employee email."
                                : result.Message);
                }

                if (!string.Equals(
                        fullName,
                        employee.User.FullName,
                        StringComparison.Ordinal))
                {
                    var authResult =
                        await _authService
                            .UpdateEmployeeUserAsync(
                                employee.UserId,
                                fullName);

                    if (!authResult.Success)
                        return Result.Failure(
                            string.IsNullOrWhiteSpace(
                                authResult.Message)
                                    ? "Failed to update employee name."
                                    : authResult.Message);
                }

                employee.Phone = phone;

                await _context.SaveChangesAsync(
                    cancellationToken);

                await _auditService.LogAsync(
                    AuditActions.Update,
                    nameof(Employee),
                    employee.Id.ToString(),
                    oldValues,
                    new
                    {
                        FullName = fullName,
                        Email = email,
                        Phone = phone
                    });

                return Result.Succeeded(
                    "Employee updated successfully.");
            }
            catch (Exception)
            {
                return Result.Failure(
                    "An unexpected error occurred.");
            }
        }

        public async Task<Result> AssignWorkScheduleToEmployeeAsync(int employeeId,int workScheduleId,
        CancellationToken cancellationToken = default)
        {
            try
            {
                var employee =
                    await _context.Employees
                        .FirstOrDefaultAsync(
                            x => x.Id == employeeId,
                            cancellationToken);

                if (employee is null)
                    return Result.Failure(
                        "Employee not found.");

                var workScheduleExists =
                    await _context.WorkSchedules
                        .AnyAsync(
                            x => x.Id == workScheduleId,
                            cancellationToken);

                if (!workScheduleExists)
                    return Result.Failure(
                        "Work schedule not found.");

                if (employee.WorkScheduleId == workScheduleId)
                    return Result.Failure(
                        "Employee is already assigned to this work schedule.");

                employee.WorkScheduleId = workScheduleId;

                await _context.SaveChangesAsync(
                    cancellationToken);

                return Result.Succeeded(
                    "Work schedule assigned to employee successfully.");
            }
            catch (Exception)
            {
                return Result.Failure(
                    "An unexpected error occurred.");
            }
        }

        public async Task<Result> AssignWorkScheduleToDepartmentAsync(int departmentId,int workScheduleId,
        CancellationToken cancellationToken = default)
        {
            try
            {
                var departmentExists =
                    await _context.Departments
                        .AnyAsync(
                            x => x.Id == departmentId,
                            cancellationToken);

                if (!departmentExists)
                    return Result.Failure(
                        "Department not found.");

                var workScheduleExists =
                    await _context.WorkSchedules
                        .AnyAsync(
                            x => x.Id == workScheduleId,
                            cancellationToken);

                if (!workScheduleExists)
                    return Result.Failure(
                        "Work schedule not found.");

                var employees =
                    await _context.Employees
                        .Where(x =>
                            x.DepartmentId == departmentId)
                        .ToListAsync(cancellationToken);

                if (employees.Count == 0)
                    return Result.Failure(
                        "No employees found in this department.");

                foreach (var employee in employees)
                {
                    employee.WorkScheduleId = workScheduleId;
                }

                await _context.SaveChangesAsync(
                    cancellationToken);

                return Result.Succeeded(
                    "Work schedule assigned to department employees successfully.");
            }
            catch (Exception)
            {
                return Result.Failure(
                    "An unexpected error occurred.");
            }
        }



        public async Task<Result> DeleteAsync(int id,
        CancellationToken cancellationToken = default)
        {
            try
            {
                var employee =
                    await _employeeQueries.GetByIdAsync(id);

                if (employee is null)
                    return Result.Failure(
                        "Employee not found.");

                if (employee.IsDeleted)
                    return Result.Failure(
                        "Employee is already deleted.");

                var documentsResult =
                   await _employeeDocumentService.
                  DeleteByEmployeeAsync(
                   employee.Id,
                   cancellationToken);

                if (!documentsResult.Success)
                {
                    return Result.Failure(
                        documentsResult.Message);
                }
                var result =
                    await _authService.DeactivateUserAsync(
                        employee.UserId);

                if (!result.Success)
                    return Result.Failure(
                        result.Message);

               

                var oldValues = new
                {
                    employee.IsDeleted,
                    employee.IsActive
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

                await _context.SaveChangesAsync(
                    cancellationToken);

                await _auditService.LogAsync(
                    AuditActions.Delete,
                    nameof(Employee),
                    employee.Id.ToString(),
                    oldValues,
                    new
                    {
                        employee.IsDeleted,
                        employee.IsActive
                    },
                    cancellationToken);

                return Result.Succeeded(
                    "Employee deleted successfully.");
            }
            catch (Exception)
            {
                return Result.Failure(
                    "An unexpected error occurred.");
            }
        }


        public async Task<Result> RestoreAsync(int id,
       CancellationToken cancellationToken = default)
        {
            try
            {
                var employee =
                    await _employeeQueries.GetDeletedByIdAsync(id);

                if (employee is null)
                    return Result.Failure(
                        "Employee not found.");

                if (!employee.IsDeleted)
                    return Result.Failure(
                        "Employee is already active.");

                var oldValues = new
                {
                    employee.IsDeleted,
                    employee.IsActive
                };

                var result =
                    await _authService.ActivateUserAsync(
                        employee.UserId);

                if (!result.Success)
                    return Result.Failure(
                        string.IsNullOrWhiteSpace(result.Message)
                            ? "Failed to activate the employee user."
                            : result.Message);

                employee.IsDeleted = false;
                employee.IsActive = true;

                await _context.SaveChangesAsync(
                    cancellationToken);

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

                return Result.Succeeded(
                    "Employee restored successfully.");
            }
            catch (Exception)
            {
                return Result.Failure(
                    "An unexpected error occurred.");
            }
        }
      
      
       public async Task<Result> ActivateAsync(int id,
       CancellationToken cancellationToken = default)
        {
            try
            {
                var employee =
                    await _employeeQueries.GetByIdAsync(id);

                if (employee is null)
                    return Result.Failure(
                        "Employee not found.");

                if (employee.IsActive)
                    return Result.Failure(
                        "Employee is already active.");

                var result =
                    await _authService.ActivateUserAsync(
                        employee.UserId);

                if (!result.Success)
                    return Result.Failure(
                        string.IsNullOrWhiteSpace(result.Message)
                            ? "Failed to activate the employee user."
                            : result.Message);

                employee.IsActive = true;

                await _context.SaveChangesAsync(
                    cancellationToken);

                return Result.Succeeded(
                    "Employee activated successfully.");
            }
            catch (Exception)
            {
                return Result.Failure(
                    "An unexpected error occurred.");
            }
        }
      
      
       public async Task<Result> DeactivateAsync(int id,
       CancellationToken cancellationToken = default)
        {
            try
            {
                var employee =
                    await _employeeQueries.GetByIdAsync(id);

                if (employee is null)
                    return Result.Failure(
                        "Employee not found.");

                if (!employee.IsActive)
                    return Result.Failure(
                        "Employee is already inactive.");

                var result =
                    await _authService.DeactivateUserAsync(
                        employee.UserId);

                if (!result.Success)
                    return Result.Failure(
                        string.IsNullOrWhiteSpace(result.Message)
                            ? "Failed to deactivate the employee user."
                            : result.Message);

                employee.IsActive = false;

                await _context.SaveChangesAsync(
                    cancellationToken);

                return Result.Succeeded(
                    "Employee deactivated successfully.");
            }
            catch (Exception)
            {
                return Result.Failure(
                    "An unexpected error occurred.");
            }
        }

        public async Task<Result> ChangeStatusAsync(int id,
         ChangeEmployeeStatusDto dto,
         CancellationToken cancellationToken = default)
        {
            try
            {
                var employee =
                    await _employeeQueries.GetByIdAsync(id);

                if (employee is null)
                {
                    return Result.Failure(
                        "Employee not found.");
                }

                if (employee.Status == dto.Status)
                {
                    return Result.Failure(
                        "Employee already has this status.");
                }

                var oldValues = new
                {
                    employee.Status,
                    employee.IsActive
                };

                // =========================================================
                // Active
                // =========================================================

                if (dto.Status == EmployeeStatus.Active)
                {
                    var activateResult =
                        await _authService.ActivateUserAsync(
                            employee.UserId);

                    if (!activateResult.Success)
                    {
                        return Result.Failure(
                            string.IsNullOrWhiteSpace(activateResult.Message)
                                ? "Failed to activate the employee user."
                                : activateResult.Message);
                    }

                    employee.Status = EmployeeStatus.Active;
                    employee.IsActive = true;
                }

                // =========================================================
                // On Leave
                // =========================================================

                else if (dto.Status == EmployeeStatus.OnLeave)
                {
                    employee.Status = EmployeeStatus.OnLeave;
                    employee.IsActive = true;
                }

                // =========================================================
                // Suspended
                // =========================================================

                else if (dto.Status == EmployeeStatus.Suspended)
                {
                    var deactivateResult =
                        await _authService.DeactivateUserAsync(
                            employee.UserId);

                    if (!deactivateResult.Success)
                    {
                        return Result.Failure(
                            string.IsNullOrWhiteSpace(deactivateResult.Message)
                                ? "Failed to deactivate the employee user."
                                : deactivateResult.Message);
                    }

                    employee.Status = EmployeeStatus.Suspended;
                    employee.IsActive = false;
                }

                // =========================================================
                // Resigned
                // =========================================================

                else if (dto.Status == EmployeeStatus.Resigned)
                {
                    var deactivateResult =
                        await _authService.DeactivateUserAsync(
                            employee.UserId);

                    if (!deactivateResult.Success)
                    {
                        return Result.Failure(
                            string.IsNullOrWhiteSpace(deactivateResult.Message)
                                ? "Failed to deactivate the employee user."
                                : deactivateResult.Message);
                    }

                    employee.Status = EmployeeStatus.Resigned;
                    employee.IsActive = false;
                }

                await _context.SaveChangesAsync(
                    cancellationToken);

                await _auditService.LogAsync(
                    AuditActions.Update,
                    nameof(Employee),
                    employee.Id.ToString(),
                    oldValues,
                    new
                    {
                        employee.Status,
                        employee.IsActive
                    });

                return Result.Succeeded(
                    "Employee status changed successfully.");
            }
            catch (Exception)
            {
                return Result.Failure(
                    "An unexpected error occurred.");
            }
        }




        public async Task<Result<List<LookupDto>>> GetLookupAsync(
    CancellationToken cancellationToken = default)
        {
            try
            {
                var employees =
                 await _context.Employees
                     .AsNoTracking()
                     .OrderBy(x => x.User.FullName)
                     .Select(x => new LookupDto
                     {
                         Value = x.Id.ToString(),
                         Text = x.User.FullName
                     })
                     .ToListAsync(cancellationToken);

                return Result<List<LookupDto>>.Succeeded(
                    employees);
            }
            catch (Exception)
            {
                return Result<List<LookupDto>>.Failure(
                    "An unexpected error occurred.");
            }
        }


    }
}
