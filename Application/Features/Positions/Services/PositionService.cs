using MicroERP.Application.Common.DTOs;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.Positions.DTOs;
using MicroERP.Application.Features.Positions.Interfaces;
using MicroERP.Domain.Audit;
using MicroERP.Domin.Entities.Employees;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.Positions.Services;

public class PositionService : IPositionService
{
    private readonly IApplicationDbContext _context;
    private readonly IPositionQueries _positionQueries;
    private readonly IAuditService _auditService;

    public PositionService(
        IApplicationDbContext context,
        IPositionQueries positionQueries,
        IAuditService auditService)
    {
        _context = context;
        _positionQueries = positionQueries;
        _auditService = auditService;
    }

    public async Task<Result<PositionDto>> CreateAsync(
     CreatePositionDto dto,
     CancellationToken cancellationToken = default)
    {
        try
        {
            var code = dto.Code.Trim().ToUpperInvariant();

            if (await _positionQueries.CodeExistsAsync(
                    code,
                    null,
                    cancellationToken))
            {
                return Result<PositionDto>.Failure(
                    "Position code already exists.");
            }

            var position = new Position
            {
                Code = code,
                NameAr = dto.NameAr.Trim(),
                NameEn = dto.NameEn.Trim(),

                Description =
                    string.IsNullOrWhiteSpace(dto.Description)
                        ? null
                        : dto.Description.Trim(),

                AssignmentType = dto.AssignmentType
            };

            _context.Positions.Add(position);

            await _context.SaveChangesAsync(
                cancellationToken);

            await _auditService.LogAsync(
                AuditActions.Create,
                nameof(Position),
                position.Id.ToString(),
                null,
                new
                {
                    position.Code,
                    position.NameAr,
                    position.NameEn,
                    position.Description,
                    position.AssignmentType,
                    position.IsActive
                },
                cancellationToken);

            return Result<PositionDto>.Succeeded(
                ToDto(position),
                "Position created successfully.");
        }
        catch (Exception)
        {
            return Result<PositionDto>.Failure(
                "An unexpected error occurred.");
        }
    }

    public async Task<Result<PositionDto>> GetByIdAsync(int id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var position = await _positionQueries.GetByIdAsync(id, cancellationToken);

            if (position is null)
                return Result<PositionDto>.Failure("Position not found.");

            return Result<PositionDto>.Succeeded(ToDto(position));
        }
        catch (Exception)
        {
            return Result<PositionDto>.Failure("An unexpected error occurred.");
        }
    }

    public async Task<Result<List<PositionListDto>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var positions = await _positionQueries.GetAllAsync(cancellationToken);

            var result = positions.Select(x => new PositionListDto
            {
                Id = x.Id,
                Code = x.Code,
                NameAr = x.NameAr,
                NameEn = x.NameEn,
                IsActive = x.IsActive
            }).ToList();

            return Result<List<PositionListDto>>.Succeeded(result);
        }
        catch (Exception)
        {
            return Result<List<PositionListDto>>.Failure("An unexpected error occurred.");
        }
    }

    public async Task<Result> UpdateAsync(int id,
     UpdatePositionDto dto,
     CancellationToken cancellationToken = default)
    {
        try
        {
            var position =
                await _positionQueries.GetByIdAsync(
                    id,
                    cancellationToken);

            if (position is null)
                return Result.Failure(
                    "Position not found.");

            var oldValues = new
            {
                position.Code,
                position.NameAr,
                position.NameEn,
                position.Description,
                position.AssignmentType
            };

            if (!string.IsNullOrWhiteSpace(dto.Code))
            {
                var code =
                    dto.Code.Trim().ToUpperInvariant();

                if (!string.Equals(
                        position.Code,
                        code,
                        StringComparison.OrdinalIgnoreCase) &&
                    await _positionQueries.CodeExistsAsync(
                        code,
                        id,
                        cancellationToken))
                {
                    return Result.Failure(
                        "Position code already exists.");
                }

                position.Code = code;
            }

            if (!string.IsNullOrWhiteSpace(dto.NameAr))
                position.NameAr = dto.NameAr.Trim();

            if (!string.IsNullOrWhiteSpace(dto.NameEn))
                position.NameEn = dto.NameEn.Trim();

            if (dto.Description is not null)
            {
                position.Description =
                    string.IsNullOrWhiteSpace(dto.Description)
                        ? null
                        : dto.Description.Trim();
            }

            if (dto.AssignmentType.HasValue)
            {
                position.AssignmentType =
                    dto.AssignmentType.Value;
            }

            await _context.SaveChangesAsync(
                cancellationToken);

            await _auditService.LogAsync(
                AuditActions.Update,
                nameof(Position),
                position.Id.ToString(),
                oldValues,
                new
                {
                    position.Code,
                    position.NameAr,
                    position.NameEn,
                    position.Description,
                    position.AssignmentType
                },
                cancellationToken);

            return Result.Succeeded(
                "Position updated successfully.");
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
            var position =
                await _positionQueries.GetByIdAsync(
                    id,
                    cancellationToken);

            if (position is null)
                return Result.Failure(
                    "Position not found.");

            if (await _positionQueries.HasEmployeesAsync(
                    id,
                    cancellationToken))
            {
                return Result.Failure(
                    "Cannot delete a position assigned to employees.");
            }

            var oldValues = new
            {
                position.IsActive,
                position.IsDeleted
            };

            position.IsDeleted = true;
            position.IsActive = false;

            await _context.SaveChangesAsync(
                cancellationToken);

            await _auditService.LogAsync(
                AuditActions.Delete,
                nameof(Position),
                position.Id.ToString(),
                oldValues,
                new
                {
                    position.IsActive,
                    position.IsDeleted
                },
                cancellationToken);

            return Result.Succeeded(
                "Position deleted successfully.");
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
            var position =
                await _context.Positions
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(
                        x => x.Id == id,
                        cancellationToken);

            if (position is null)
                return Result.Failure(
                    "Position not found.");

            if (!position.IsDeleted)
                return Result.Failure(
                    "Position is not deleted.");

            var oldValues = new
            {
                position.IsActive,
                position.IsDeleted
            };

            position.IsDeleted = false;
            position.IsActive = true;

            await _context.SaveChangesAsync(
                cancellationToken);

            await _auditService.LogAsync(
                AuditActions.Restore,
                nameof(Position),
                position.Id.ToString(),
                oldValues,
                new
                {
                    position.IsActive,
                    position.IsDeleted
                },
                cancellationToken);

            return Result.Succeeded(
                "Position restored successfully.");
        }
        catch (Exception)
        {
            return Result.Failure(
                "An unexpected error occurred.");
        }
    }

    public async Task<Result> AssignEmployeeAsync(
      int positionId,
      int employeeId,
      CancellationToken cancellationToken = default)
    {
        try
        {
            var position =
                await _positionQueries.GetByIdAsync(
                    positionId,
                    cancellationToken);

            if (position is null)
                return Result.Failure(
                    "Position not found.");

            var employee =
                await _context.Employees
                    .FirstOrDefaultAsync(
                        x => x.Id == employeeId,
                        cancellationToken);

            if (employee is null)
                return Result.Failure(
                    "Employee not found.");

            var alreadyAssigned =
                await _positionQueries
                    .EmployeeAssignedToPositionAsync(
                        employeeId,
                        positionId,
                        cancellationToken);

            if (alreadyAssigned)
            {
                return Result.Failure(
                    "Employee is already assigned to this position.");
            }

            switch (position.AssignmentType)
            {
                case PositionAssignmentType.SinglePerDepartment:

                    var existsInDepartment =
                        await _positionQueries
                            .PositionAlreadyAssignedInDepartmentAsync(
                                positionId,
                                employee.DepartmentId,
                                employeeId,
                                cancellationToken);

                    if (existsInDepartment)
                    {
                        return Result.Failure(
                            "This position is already assigned to another employee in the same department.");
                    }

                    break;

                case PositionAssignmentType.SingleCompanyWide:

                    var existsCompanyWide =
                        await _positionQueries
                            .PositionAlreadyAssignedCompanyWideAsync(
                                positionId,
                                employeeId,
                                cancellationToken);

                    if (existsCompanyWide)
                    {
                        return Result.Failure(
                            "This position is already assigned to another employee.");
                    }

                    break;
            }

            var oldPositionId = employee.PositionId;

            employee.PositionId = positionId;

            await _context.SaveChangesAsync(
                cancellationToken);

            await _auditService.LogAsync(
                AuditActions.Update,
                nameof(Employee),
                employee.Id.ToString(),
                new
                {
                    PositionId = oldPositionId
                },
                new
                {
                    PositionId = employee.PositionId
                },
                cancellationToken);

            return Result.Succeeded(
                "Employee assigned to position successfully.");
        }
        catch (Exception)
        {
            return Result.Failure(
                "An unexpected error occurred.");
        }
    }

    public async Task<Result> RemoveEmployeeAsync(
    int positionId,
    int employeeId,
    CancellationToken cancellationToken = default)
    {
        try
        {
            var position =
                await _positionQueries.GetByIdAsync(
                    positionId,
                    cancellationToken);

            if (position is null)
                return Result.Failure(
                    "Position not found.");

            var employee =
                await _context.Employees
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id == employeeId &&
                            x.PositionId == positionId,
                        cancellationToken);

            if (employee is null)
                return Result.Failure(
                    "Employee is not assigned to this position.");

            var oldPositionId = employee.PositionId;

            employee.PositionId = null;

            await _context.SaveChangesAsync(
                cancellationToken);

            await _auditService.LogAsync(
                AuditActions.Update,
                nameof(Employee),
                employee.Id.ToString(),
                new
                {
                    PositionId = oldPositionId
                },
                new
                {
                    PositionId = (int?)null
                },
                cancellationToken);

            return Result.Succeeded(
                "Employee removed from position successfully.");
        }
        catch (Exception)
        {
            return Result.Failure(
                "An unexpected error occurred.");
        }
    }

    public async Task<Result<List<EmployeePositionListDto>>> GetEmployeesAsync(int positionId,
    CancellationToken cancellationToken = default)
    {
        try
        {
            var position =
                await _positionQueries.GetByIdAsync(
                    positionId,
                    cancellationToken);

            if (position is null)
                return Result<List<EmployeePositionListDto>>.Failure(
                    "Position not found.");

            var employees =
                await _positionQueries.GetEmployeesAsync(
                    positionId,
                    cancellationToken);

            var result =
                employees
                    .Select(x => new EmployeePositionListDto
                    {
                        EmployeeId = x.Id,
                        EmployeeName =
                            $"{x.User.FullName}".Trim(),
                        PositionId = x.PositionId!.Value,
                        PositionName = x.Position!.NameEn
                    })
                    .ToList();

            return Result<List<EmployeePositionListDto>>.Succeeded(
                result);
        }
        catch (Exception)
        {
            return Result<List<EmployeePositionListDto>>.Failure(
                "An unexpected error occurred.");
        }
    }

    public async Task<Result<List<LookupDto>>> GetLookupAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var positions = await _context.Positions
                .AsNoTracking()
                .OrderBy(x => x.NameEn)
                .Select(x => new LookupDto
                {
                    Value = x.Id.ToString(),
                    Text = x.NameEn
                })
                .ToListAsync(cancellationToken);

            return Result<List<LookupDto>>.Succeeded(positions);
        }
        catch (Exception)
        {
            return Result<List<LookupDto>>.Failure("An unexpected error occurred.");
        }
    }

    private static PositionDto ToDto(Position position)
    {
        return new PositionDto
        {
            Id = position.Id,
            Code = position.Code,
            NameAr = position.NameAr,
            NameEn = position.NameEn,
            Description = position.Description,
            AssignmentType = position.AssignmentType,
            IsActive = position.IsActive
        };
    }
}
