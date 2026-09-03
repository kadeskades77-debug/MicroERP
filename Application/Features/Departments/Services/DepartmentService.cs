using MicroERP.Application.Common.DTOs;
using MicroERP.Application.Common.Exceptions;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Mappings;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.Departments.DTOs;
using MicroERP.Application.Features.Departments.Interfaces;
using MicroERP.Application.Features.Employees.Interfaces;
using MicroERP.Domain.Audit;
using MicroERP.Domin.Entities.Employees;
using Microsoft.EntityFrameworkCore;
namespace MicroERP.Application.Features.Departments.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IApplicationDbContext _context;
        private readonly IAuditService _auditService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployeeQueries _employeeQueries;
        private readonly IDepartmentQueries _departmentQueries;
        public DepartmentService(IApplicationDbContext context, IAuditService auditService, IUnitOfWork unitOfWork, IEmployeeQueries employeeQueries, IDepartmentQueries departmentQueries)
        {
            _context = context;
            _auditService = auditService;
            _unitOfWork = unitOfWork;
            _employeeQueries = employeeQueries;
            _departmentQueries = departmentQueries;
        }

        public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto)
        {
            var exists = await _context.Departments
                .AnyAsync(x => x.NameAr == dto.NameAr);

            if (exists)
                throw new Exception("Department name already exists");

            var department = new Department
            {
                Code = await GenerateDepartmentCodeAsync(),
                NameAr = dto.NameAr.Trim(),
                NameEn = dto.NameEn?.Trim()
            };

            _context.Departments.Add(department);
         
             await _context.SaveChangesAsync();

            await _auditService.LogAsync(
             AuditActions.Create,
             nameof(Department),
             department.Id.ToString(),
             null,
             new
             {
                 department.Code,
                 department.NameAr,
                 department.NameEn,
                 department.ManagerEmployeeId
             });
            return department.ToDto();
        }
        public async Task<Result<List<DepartmentListDto>>> GetAllAsync()
        {
            var data = await _departmentQueries.GetAllAsync();

            return Result<List<DepartmentListDto>>.Succeeded(data);
        }
        public async Task<DepartmentDto?> GetByIdAsync(int id)
        {
            var department = await _departmentQueries.GetByIdAsync(id);

            if (department == null)
                throw new NotFoundException("Department not found");

            return department.ToDto();
        }
        public async Task<DepartmentDto> UpdateAsync(int id,UpdateDepartmentDto dto)
        {
            var department = await _departmentQueries.GetByIdAsync(id);

            if (department is null)
                throw new NotFoundException("Department not found.");

            var oldValues = new
            {
                department.Code,
                department.NameAr,
                department.NameEn,
                department.ManagerEmployeeId
            };

            if (!string.IsNullOrWhiteSpace(dto.NameAr))
            {
                dto.NameAr = dto.NameAr.Trim();

                var exists = await _departmentQueries.NameArExistsAsync(dto.NameAr, id);

                if (exists)
                    throw new BusinessException("Department name already exists.");

                department.NameAr = dto.NameAr;
            }

            if (dto.NameEn is not null)
            {
                department.NameEn = string.IsNullOrWhiteSpace(dto.NameEn)
                    ? null
                    : dto.NameEn.Trim();
            }
            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
                AuditActions.Update,
                nameof(Department),
                department.Id.ToString(),
                oldValues,
                new
                {
                    department.Code,
                    department.NameAr,
                    department.NameEn,
                    department.ManagerEmployeeId
                });

         

            return department.ToDto();
        }
        public async Task<DepartmentDto> AssignManagerAsync(int id,AssignDepartmentManagerDto dto)
        {
            var department = await _departmentQueries.GetByIdAsync(id);
            if (department == null)
                throw new NotFoundException("Department not found");

            if (dto.ManagerEmployeeId.HasValue)
            {
                var employee = await _context.Employees
                    .FirstOrDefaultAsync(x =>
                        x.Id == dto.ManagerEmployeeId.Value);

                if (employee == null)
                    throw new NotFoundException("Employee not found");

                if (employee.DepartmentId != department.Id)
                    throw new BusinessException(
                        "Manager must belong to the same department");
            }
            var oldValues = new
            {
                department.ManagerEmployeeId,
                department.HasManager
            };

            department.ManagerEmployeeId = dto.ManagerEmployeeId;
            department.HasManager = true;
            await _context.SaveChangesAsync();
            await _auditService.LogAsync(
                  AuditActions.AssignManager,
                  nameof(Department),
                  department.Id.ToString(),
                  oldValues,
                  new
                  {
                      department.ManagerEmployeeId,
                      department.HasManager
                  });
           

            return department.ToDto();
        }
        public async Task<Result> TransferDepartmentManagerAsync(int managerEmployeeId,TransferDepartmentManagerDto dto)
        {
            return await _unitOfWork.ExecuteAsync(async () =>
            {
                var employee = await _employeeQueries.GetByIdAsync(managerEmployeeId);
                if (employee is null)
                    return Result.Failure("Employee not found.");

                var currentDepartment = await _departmentQueries.GetByManagerIdAsync(managerEmployeeId);

                if (currentDepartment is null)
                    return Result.Failure(
                        "Employee is not assigned as a department manager.");

            var newDepartment = await _departmentQueries.GetByIdAsync(dto.DepartmentId);

                if (newDepartment is null)
                    return Result.Failure("Department not found.");

                if (currentDepartment.Id == newDepartment.Id)
                    return Result.Failure(
                        "Manager already belongs to this department.");
                if (employee.DepartmentId != currentDepartment.Id)
                {
                    return Result.Failure(
                        "The manager must belong to the department they manage.");
                }
                var oldManagerId = newDepartment.ManagerEmployeeId;

                // إزالة المدير الحالي من القسم الجديد إن وجد
                newDepartment.ManagerEmployeeId = null;

                // إزالة المدير من القسم القديم
                currentDepartment.ManagerEmployeeId = null;

                // نقل الموظف للقسم الجديد
                employee.DepartmentId = newDepartment.Id;

                // تعيينه مديراً للقسم الجديد
                newDepartment.ManagerEmployeeId = employee.Id;
                await _context.SaveChangesAsync();
                await _auditService.LogAsync(
                    AuditActions.Update,
                    nameof(Department),
                    currentDepartment.Id.ToString(),
                    new
                    {
                        OldDepartmentId = currentDepartment.Id,
                        NewDepartmentId = newDepartment.Id,
                        PreviousManager = oldManagerId
                    },
                    new
                    {
                        NewManager = employee.Id
                    });

                await _context.SaveChangesAsync();

                return Result.Succeeded(
                    "Department manager transferred successfully.");
            });
        }
        public async Task DeleteAsync(int id)
        {
            var department = await _departmentQueries.GetByIdAsync(id);

            if (department == null)
                throw new NotFoundException("Department not found");

            var hasEmployees = await _context.Employees
                .AnyAsync(x =>
                    x.DepartmentId == id);

            if (hasEmployees)
                throw new BusinessException(
                    "Cannot delete department because it contains employees");
            var oldValues = new
            {
                department.IsDeleted
            };
            department.IsDeleted = true;
            department.IsActive = false;
            await _context.SaveChangesAsync();
            await _auditService.LogAsync(
                AuditActions.Delete,
                nameof(Department),
                department.Id.ToString(),
                oldValues,
                new
                {
                    department.IsDeleted
                });
 
            await _context.SaveChangesAsync();
        }
        public async Task RestoreAsync(int id)
        {
            var department = await _departmentQueries.GetDeletedByIdAsync(id);

            if (department is null)
                throw new NotFoundException("Department not found.");

            if (!department.IsDeleted)
                throw new BusinessException("Department is already active.");

            var oldValues = new
            {
                department.IsDeleted,
                department.IsActive
            };

            department.IsDeleted = false;
            department.IsActive = true;

            await _auditService.LogAsync(
                AuditActions.Restore,
                nameof(Department),
                department.Id.ToString(),
                oldValues,
                new
                {
                    department.IsDeleted,
                    department.IsActive
                });

            await _context.SaveChangesAsync();
        }
        public async Task<List<LookupDto>> GetLookupAsync()
        {
            return await _context.Departments
                .AsNoTracking()
                .OrderBy(x => x.NameAr)
                .Select(x => new LookupDto
                {
                    Value = x.Id.ToString(),
                    Text = x.NameAr
                })
                .ToListAsync();
        }

        //===================Helpers=================
        private async Task<string> GenerateDepartmentCodeAsync()
        {
            var lastDepartment = await _context.Departments
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            if (lastDepartment == null)
                return "DEP0001";

            return $"DEP{lastDepartment.Id + 1:D4}";
        }
    }
}