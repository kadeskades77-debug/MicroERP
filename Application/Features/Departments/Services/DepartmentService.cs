using MicroERP.Application.Common.DTOs;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Mappings;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.Departments.DTOs;
using MicroERP.Application.Features.Departments.Interfaces;
using MicroERP.Application.Features.Employees.Interfaces;
using MicroERP.Domain.Audit;
using MicroERP.Domin.Entities.Employees;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;
namespace MicroERP.Application.Features.Departments.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IApplicationDbContext _context;
        private readonly IAuditService _auditService;
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentService(IApplicationDbContext context,
            IAuditService auditService, 
            IUnitOfWork unitOfWork, IDepartmentQueries departmentQueries)
        {
            _context = context;
            _auditService = auditService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<DepartmentDto>> CreateAsync(
        CreateDepartmentDto dto,
        CancellationToken cancellationToken = default)
        {
            // =========================================================
            // Validate Arabic Name
            // =========================================================

            if (string.IsNullOrWhiteSpace(dto.NameAr))
            {
                return Result<DepartmentDto>.Failure(
                    "Arabic department name is required.");
            }

            // =========================================================
            // Validate English Name
            // =========================================================

            if (string.IsNullOrWhiteSpace(dto.NameEn))
            {
                return Result<DepartmentDto>.Failure(
                    "English department name is required.");
            }

            // =========================================================
            // Check Duplicate Arabic Name
            // =========================================================

            var arabicNameExists =
                await _context.Departments
                    .AnyAsync(
                        x => x.NameAr == dto.NameAr,
                        cancellationToken);

            if (arabicNameExists)
            {
                return Result<DepartmentDto>.Failure(
                    "A department with the same Arabic name already exists.");
            }

            // =========================================================
            // Generate Department Code
            // =========================================================

            var code =
                await GenerateDepartmentCodeAsync();

            // =========================================================
            // Create Department
            // =========================================================

            var department = new Department
            {
                Code = code,
                NameAr = dto.NameAr.Trim(),
                NameEn = dto.NameEn.Trim(),
                IsActive = true,
                HasManager = false
            };

            // =========================================================
            // Save
            // =========================================================

            await _unitOfWork.ExecuteAsync(async () =>
            {
                await _context.Departments.AddAsync(
                    department,
                    cancellationToken);

                await _context.SaveChangesAsync(
                    cancellationToken);

                await _auditService.LogAsync(
                    action: "Create",
                    entityName: nameof(Department),
                    entityId: department.Id.ToString(),
                    oldValues: null,
                    newValues: department,
                    cancellationToken: cancellationToken);
            }, cancellationToken);
            // =========================================================
            // Return DTO
            // =========================================================

            var result = new DepartmentDto
            {
                Id = department.Id,
                Code = department.Code,
                NameAr = department.NameAr,
                NameEn = department.NameEn,
                ManagerEmployeeId = department.ManagerEmployeeId,
                IsActive = department.IsActive,
                HasManager = department.HasManager
            };

            return Result<DepartmentDto>.Succeeded(
                result,
                "Department created successfully.");
        }
        public async Task<Result<List<DepartmentListDto>>> GetAllAsync(
        DepartmentFilterDto filter,
        CancellationToken cancellationToken = default)
        {
            var query =
                _context.Departments
                    .AsNoTracking()
                    .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim();

                query = query.Where(x =>
                    x.Code.Contains(search) ||
                    x.NameAr.Contains(search) ||
                    x.NameEn.Contains(search));
            }

            if (filter.IsActive.HasValue)
            {
                query = query.Where(x =>
                    x.IsActive == filter.IsActive.Value);
            }

            if (filter.HasManager.HasValue)
            {
                query = query.Where(x =>
                    x.HasManager == filter.HasManager.Value);
            }

            var departments =
                await query
                    .Select(x => new DepartmentListDto
                    {
                        Id = x.Id,
                        Code = x.Code,
                        NameAr = x.NameAr,
                        NameEn = x.NameEn,

                        ManagerName = x.ManagerEmployee != null
                            ? x.ManagerEmployee.User.FullName
                            : null,

                        IsActive = x.IsActive
                    })
                    .OrderBy(x => x.Id)
                    .ToListAsync(cancellationToken);

            return Result<List<DepartmentListDto>>.Succeeded(
                departments);
        }
        public async Task<Result<DepartmentDto>> GetByIdAsync(int id,
        CancellationToken cancellationToken = default)
        {
            var department =
                await _context.Departments
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x => x.Id == id,
                        cancellationToken);

            if (department == null)
            {
                return Result<DepartmentDto>.Failure(
                    "Department not found.");
            }

            var result = new DepartmentDto
            {
                Id = department.Id,
                Code = department.Code,
                NameAr = department.NameAr,
                NameEn = department.NameEn,
                ManagerEmployeeId = department.ManagerEmployeeId,
                IsActive = department.IsActive,
                HasManager = department.HasManager
            };

            return Result<DepartmentDto>.Succeeded(result);
        }
        public async Task<Result> UpdateAsync(int id,
        UpdateDepartmentDto dto,
        CancellationToken cancellationToken = default)
        {
            // =========================================================
            // Get Department
            // =========================================================

            var department =
                await _context.Departments
                    .FirstOrDefaultAsync(
                        x => x.Id == id,
                        cancellationToken);

            if (department == null)
            {
                return Result.Failure(
                    "Department not found.");
            }

            // =========================================================
            // Validate At Least One Property
            // =========================================================

            if (dto.NameAr is null &&
                dto.NameEn is null)
            {
                return Result.Failure(
                    "At least one field must be provided.");
            }

            // =========================================================
            // Validate Arabic Name
            // =========================================================

            if (dto.NameAr is not null &&
                string.IsNullOrWhiteSpace(dto.NameAr))
            {
                return Result.Failure(
                    "Arabic department name cannot be empty.");
            }

            // =========================================================
            // Validate English Name
            // =========================================================

            if (dto.NameEn is not null &&
                string.IsNullOrWhiteSpace(dto.NameEn))
            {
                return Result.Failure(
                    "English department name cannot be empty.");
            }

            // =========================================================
            // Check Duplicate Arabic Name
            // =========================================================

            if (dto.NameAr is not null)
            {
                var arabicNameExists =
                    await _context.Departments
                        .AnyAsync(
                            x => x.Id != id &&
                                 x.NameAr == dto.NameAr,
                            cancellationToken);

                if (arabicNameExists)
                {
                    return Result.Failure(
                        "A department with the same Arabic name already exists.");
                }
            }

            // =========================================================
            // Check Duplicate English Name
            // =========================================================

            if (dto.NameEn is not null)
            {
                var englishNameExists =
                    await _context.Departments
                        .AnyAsync(
                            x => x.Id != id &&
                                 x.NameEn == dto.NameEn,
                            cancellationToken);

                if (englishNameExists)
                {
                    return Result.Failure(
                        "A department with the same English name already exists.");
                }
            }

            // =========================================================
            // Capture Old Values
            // =========================================================

            var oldValues = new
            {
                department.NameAr,
                department.NameEn
            };

            // =========================================================
            // Update
            // =========================================================

            if (dto.NameAr is not null)
            {
                department.NameAr = dto.NameAr.Trim();
            }

            if (dto.NameEn is not null)
            {
                department.NameEn = dto.NameEn.Trim();
            }

            // =========================================================
            // Save + Audit
            // =========================================================

            await _unitOfWork.ExecuteAsync(async () =>
            {
                await _context.SaveChangesAsync(
                    cancellationToken);

                var newValues = new
                {
                    department.NameAr,
                    department.NameEn
                };

                await _auditService.LogAsync(
                    action: "Update",
                    entityName: nameof(Department),
                    entityId: department.Id.ToString(),
                    oldValues: oldValues,
                    newValues: newValues,
                    cancellationToken: cancellationToken);
            }, cancellationToken);

            return Result.Succeeded(
                "Department updated successfully.");
        }
        public async Task<Result<DepartmentDto>> AssignManagerAsync(int id,
         AssignDepartmentManagerDto dto,
         CancellationToken cancellationToken = default)
        {
            // =========================================================
            // Get Department
            // =========================================================

            var department =
                await _context.Departments
                    .FirstOrDefaultAsync(
                        x => x.Id == id,
                        cancellationToken);

            if (department == null)
            {
                return Result<DepartmentDto>.Failure(
                    "Department not found.");
            }

            // =========================================================
            // Validate Manager
            // =========================================================

            if (!dto.ManagerEmployeeId.HasValue)
            {
                return Result<DepartmentDto>.Failure(
                    "Manager employee is required.");
            }

            // =========================================================
            // Get Employee
            // =========================================================

            var employee =
                await _context.Employees
                    .FirstOrDefaultAsync(
                        x => x.Id == dto.ManagerEmployeeId.Value,
                        cancellationToken);

            if (employee == null)
            {
                return Result<DepartmentDto>.Failure(
                    "Manager employee not found.");
            }

            // =========================================================
            // Validate Employee Status
            // =========================================================

            if (employee.Status != EmployeeStatus.Active)
            {
                return Result<DepartmentDto>.Failure(
                    "Only an active employee can be assigned as department manager.");
            }

            // =========================================================
            // Capture Old Values
            // =========================================================

            var oldValues = new
            {
                department.ManagerEmployeeId,
                department.HasManager
            };

            // =========================================================
            // Assign Manager
            // =========================================================

            department.ManagerEmployeeId =
                dto.ManagerEmployeeId.Value;

            department.HasManager = true;

            // =========================================================
            // Save + Audit
            // =========================================================

            await _unitOfWork.ExecuteAsync(async () =>
            {
                await _context.SaveChangesAsync(
                    cancellationToken);

                var newValues = new
                {
                    department.ManagerEmployeeId,
                    department.HasManager
                };

                await _auditService.LogAsync(
                    action: "AssignManager",
                    entityName: nameof(Department),
                    entityId: department.Id.ToString(),
                    oldValues: oldValues,
                    newValues: newValues,
                    cancellationToken: cancellationToken);
            }, cancellationToken);

            // =========================================================
            // Return DTO
            // =========================================================

            var result = new DepartmentDto
            {
                Id = department.Id,
                Code = department.Code,
                NameAr = department.NameAr,
                NameEn = department.NameEn,
                ManagerEmployeeId = department.ManagerEmployeeId,
                IsActive = department.IsActive,
                HasManager = department.HasManager
            };

            return Result<DepartmentDto>.Succeeded(
                result,
                "Department manager assigned successfully.");
        }
        public async Task<Result> TransferDepartmentManagerAsync(int managerEmployeeId,
        TransferDepartmentManagerDto dto,
        CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.ExecuteAsync(async () =>
            {
                // =====================================================
                // Get Employee
                // =====================================================

                var employee =
                    await _context.Employees
                        .FirstOrDefaultAsync(
                            x => x.Id == managerEmployeeId,
                            cancellationToken);

                if (employee == null)
                {
                    return Result.Failure(
                        "Employee not found.");
                }

                // =====================================================
                // Get Current Department
                // =====================================================

                var currentDepartment =
                    await _context.Departments
                        .FirstOrDefaultAsync(
                            x => x.ManagerEmployeeId == managerEmployeeId,
                            cancellationToken);

                if (currentDepartment == null)
                {
                    return Result.Failure(
                        "Employee is not assigned as a department manager.");
                }

                // =====================================================
                // Get New Department
                // =====================================================

                var newDepartment =
                    await _context.Departments
                        .FirstOrDefaultAsync(
                            x => x.Id == dto.DepartmentId,
                            cancellationToken);

                if (newDepartment == null)
                {
                    return Result.Failure(
                        "Department not found.");
                }

                // =====================================================
                // Same Department
                // =====================================================

                if (currentDepartment.Id == newDepartment.Id)
                {
                    return Result.Failure(
                        "Manager already belongs to this department.");
                }

                // =====================================================
                // Validate Manager Department
                // =====================================================

                if (employee.DepartmentId != currentDepartment.Id)
                {
                    return Result.Failure(
                        "The manager must belong to the department they manage.");
                }

                // =====================================================
                // Capture Old Values
                // =====================================================
                var oldManagerId =
                    newDepartment.ManagerEmployeeId;

                var oldValues = new
                {
                    OldDepartmentId = currentDepartment.Id,
                    OldManagerId = currentDepartment.ManagerEmployeeId,
                    NewDepartmentId = newDepartment.Id,
                    PreviousNewDepartmentManagerId = oldManagerId,
                    EmployeeDepartmentId = employee.DepartmentId
                };

                // =====================================================
                // Remove Manager From Current Department
                // =====================================================

                currentDepartment.ManagerEmployeeId = null;
                currentDepartment.HasManager = false;

                // =====================================================
                // Transfer Employee
                // =====================================================

                employee.DepartmentId = newDepartment.Id;

                // =====================================================
                // Assign New Manager To Target Department
                // =====================================================

                newDepartment.ManagerEmployeeId = employee.Id;
                newDepartment.HasManager = true;
                // =====================================================
                // Save
                // =====================================================

                await _context.SaveChangesAsync(
                    cancellationToken);

                // =====================================================
                // Audit
                // =====================================================

                var newValues = new
                {
                    OldDepartmentId = currentDepartment.Id,
                    NewDepartmentId = newDepartment.Id,
                    NewManagerId = employee.Id,
                    EmployeeDepartmentId = employee.DepartmentId
                };

                await _auditService.LogAsync(
                    action: AuditActions.Update,
                    entityName: nameof(Department),
                    entityId: currentDepartment.Id.ToString(),
                    oldValues: oldValues,
                    newValues: newValues,
                    cancellationToken: cancellationToken);

                return Result.Succeeded(
                    "Department manager transferred successfully.");
            }, cancellationToken);
        }
        public async Task<Result> DeleteAsync(int id,
        CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.ExecuteAsync(async () =>
            {
                // =====================================================
                // Get Department
                // =====================================================

                var department =
                    await _context.Departments
                        .FirstOrDefaultAsync(
                            x => x.Id == id,
                            cancellationToken);

                if (department == null)
                {
                    return Result.Failure(
                        "Department not found.");
                }

                // =====================================================
                // Check Employees
                // =====================================================

                var hasEmployees =
                    await _context.Employees
                        .AnyAsync(
                            x => x.DepartmentId == id,
                            cancellationToken);

                if (hasEmployees)
                {
                    return Result.Failure(
                        "Cannot delete department because it contains employees.");
                }

                // =====================================================
                // Capture Old Values
                // =====================================================

                var oldValues = new
                {
                    department.IsDeleted,
                    department.IsActive
                };

                // =====================================================
                // Soft Delete
                // =====================================================

                department.IsDeleted = true;
                department.IsActive = false;

                // =====================================================
                // Save
                // =====================================================

                await _context.SaveChangesAsync(
                    cancellationToken);

                // =====================================================
                // Audit
                // =====================================================

                await _auditService.LogAsync(
                    action: AuditActions.Delete,
                    entityName: nameof(Department),
                    entityId: department.Id.ToString(),
                    oldValues: oldValues,
                    newValues: new
                    {
                        department.IsDeleted,
                        department.IsActive
                    },
                    cancellationToken: cancellationToken);

                return Result.Succeeded(
                    "Department deleted successfully.");
            }, cancellationToken);
        }
        public async Task<Result> RestoreAsync(int id,
        CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.ExecuteAsync(async () =>
            {
                // =====================================================
                // Get Deleted Department
                // =====================================================

                var department =
                    await _context.Departments
                        .IgnoreQueryFilters()
                        .FirstOrDefaultAsync(
                            x => x.Id == id,
                            cancellationToken);

                if (department == null)
                {
                    return Result.Failure(
                        "Department not found.");
                }

                // =====================================================
                // Check Current State
                // =====================================================

                if (!department.IsDeleted)
                {
                    return Result.Failure(
                        "Department is already active.");
                }

                // =====================================================
                // Capture Old Values
                // =====================================================

                var oldValues = new
                {
                    department.IsDeleted,
                    department.IsActive
                };

                // =====================================================
                // Restore
                // =====================================================

                department.IsDeleted = false;
                department.IsActive = true;

                // =====================================================
                // Save
                // =====================================================

                await _context.SaveChangesAsync(
                    cancellationToken);

                // =====================================================
                // Audit
                // =====================================================

                await _auditService.LogAsync(
                    action: AuditActions.Restore,
                    entityName: nameof(Department),
                    entityId: department.Id.ToString(),
                    oldValues: oldValues,
                    newValues: new
                    {
                        department.IsDeleted,
                        department.IsActive
                    },
                    cancellationToken: cancellationToken);

                return Result.Succeeded(
                    "Department restored successfully.");
            }, cancellationToken);
        }
        public async Task<Result<List<LookupDto>>> GetLookupAsync(
        CancellationToken cancellationToken = default)
        {
            var departments =
                await _context.Departments
                    .AsNoTracking()
                    .Where(x => x.IsActive)
                    .Select(x => new LookupDto
                    {
                        Value = x.Id.ToString(),
                        Text = x.NameAr
                    })
                    .OrderBy(x => x.Text)
                    .ToListAsync(cancellationToken);

            return Result<List<LookupDto>>.Succeeded(
                departments);
        }
        public async Task<Result<List<LookupDto>>> GetDepartmentsWithoutManagerAsync(
        CancellationToken cancellationToken = default)
        {
            var departments =
                await _context.Departments
                    .AsNoTracking()
                    .Where(x =>
                        x.IsActive &&
                        !x.HasManager)
                    .Select(x => new LookupDto
                    {
                        Value = x.Id.ToString(),
                        Text = x.NameAr
                    })
                    .OrderBy(x => x.Text)
                    .ToListAsync(cancellationToken);

            return Result<List<LookupDto>>.Succeeded(
                departments);
        }

        //===================Helpers=================
        private async Task<string> GenerateDepartmentCodeAsync(
          CancellationToken cancellationToken = default)
        {
            var lastDepartment =
                await _context.Departments
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefaultAsync(cancellationToken);

            if (lastDepartment == null)
                return "DEP0001";

            return $"DEP{lastDepartment.Id + 1:D4}";
        }
    }
}