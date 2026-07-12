using Domin.Entities;
using MicroERP.Application.Common.Exceptions;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.Departments.DTOs;
using MicroERP.Application.Features.Departments.Interfaces;
using MicroERP.Domain.Audit;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.Departments.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IApplicationDbContext _context;
        private readonly IAuditService _auditService;
        public DepartmentService(IApplicationDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
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
             await _context.SaveChangesAsync();

            return MapToDto(department);
        }

        public async Task<Result<List<DepartmentListDto>>> GetAllAsync()
        {
            var data = await _context.Departments
                .Select(x => new DepartmentListDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    NameAr = x.NameAr,
                    ManagerName = x.ManagerEmployee != null
                        ? x.ManagerEmployee.User.FullName
                        : null,
                    IsActive = x.IsActive
                })
                .ToListAsync();

            return Result<List<DepartmentListDto>>.Succeeded(data);
        }
        public async Task<DepartmentDto> GetByIdAsync(int id)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(x =>
                    x.Id == id);

            if (department == null)
                throw new NotFoundException("Department not found");

            return MapToDto(department);
        }
        public async Task<DepartmentDto> UpdateAsync(int id,UpdateDepartmentDto dto)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(x => x.Id == id);

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

                var exists = await _context.Departments
                    .AnyAsync(x =>
                        x.Id != id &&
                        x.NameAr == dto.NameAr);

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

            await _context.SaveChangesAsync();

            return MapToDto(department);
        }
        public async Task<DepartmentDto> AssignManagerAsync(int id,AssignDepartmentManagerDto dto)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(x =>
                    x.Id == id);
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
                department.ManagerEmployeeId
            };

            department.ManagerEmployeeId = dto.ManagerEmployeeId;
            department.HasManager =true;

            await _auditService.LogAsync(
                  AuditActions.AssignManager,
                  nameof(Department),
                  department.Id.ToString(),
                  oldValues,
                  new
                  {
                      department.ManagerEmployeeId
                  });
            await _context.SaveChangesAsync();

            return MapToDto(department);
        }

        public async Task DeleteAsync(int id)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(x =>
                    x.Id == id);

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
            var department = await _context.Departments
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Id == id);

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

        private static DepartmentDto MapToDto(Department department)
        {
            return new DepartmentDto
            {
                Id = department.Id,
                Code = department.Code,
                NameAr = department.NameAr,
                NameEn = department.NameEn,
                ManagerEmployeeId = department.ManagerEmployeeId,
                HasManager = department.ManagerEmployeeId.HasValue,
                IsActive = department.IsActive
            };
        }
    }
}