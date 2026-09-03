
using AutoMapper;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.EmployeeEvaluations.Interfaces;
using MicroERP.Domin.Entities.EmployeeEvaluations;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.EmployeeEvaluations.Services;

public class EvaluationPeriodService : IEvaluationPeriodService
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;
    private readonly IMapper _mapper;
    public EvaluationPeriodService(IApplicationDbContext context, IAuditService auditService, IMapper mapper)
    {
        _context = context;
        _auditService = auditService;
        _mapper = mapper;
    }


    public async Task<Result<EvaluationPeriodDto>> CreateAsync(
     CreateEvaluationPeriodDto dto,
     CancellationToken ct = default)
    {
        // =========================================================
        // Validation
        // =========================================================

        if (dto == null)
        {
            return Result<EvaluationPeriodDto>.Failure(
                "Evaluation period data is required.");
        }


        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return Result<EvaluationPeriodDto>.Failure(
                "Evaluation period name is required.");
        }


        if (!IsValidYear(dto.Year))
        {
            return Result<EvaluationPeriodDto>.Failure(
                "Invalid year.");
        }


        if (!IsValidMonth(dto.Month))
        {
            return Result<EvaluationPeriodDto>.Failure(
                "Invalid month.");
        }


        // =========================================================
        // Check Duplicate Period
        // =========================================================

        var periodExists =
            await PeriodExistsAsync(
                dto.Year,
                dto.Month,
                ct: ct);

        if (periodExists)
        {
            return Result<EvaluationPeriodDto>.Failure(
                "An evaluation period already exists for this year and month.");
        }


        // =========================================================
        // Calculate Dates
        // =========================================================

        var (startDate, endDate) =
            GetMonthDates(
                dto.Year,
                dto.Month);


        // =========================================================
        // Create
        // =========================================================

        var period = new EvaluationPeriod
        {
            Name = dto.Name.Trim(),

            StartDate = startDate,

            EndDate = endDate,

            Status = EvaluationPeriodStatus.Draft
        };


        _context.EvaluationPeriods.Add(period);


        // =========================================================
        // Save
        // =========================================================

        await _context.SaveChangesAsync(ct);


        // =========================================================
        // Audit
        // =========================================================

        var newValues = new
        {
            period.Name,
            Year = period.StartDate.Year,
            Month = period.StartDate.Month,
            period.StartDate,
            period.EndDate,
            period.Status
        };

        await _auditService.LogAsync(
            "Create",
            "EvaluationPeriod",
            period.Id.ToString(),
            null,
            newValues);


        // =========================================================
        // Result
        // =========================================================

        var result = new EvaluationPeriodDto
        {
            Id = period.Id,
            Name = period.Name,
            StartDate = period.StartDate,
            EndDate = period.EndDate,
            Status = period.Status,
            IsActive = period.IsActive
        };


        return Result<EvaluationPeriodDto>.Succeeded(result);
    }

    public async Task<Result<EvaluationPeriodDto>> GetByIdAsync(int id,
     CancellationToken ct = default)
    {
        var period =
            await _context.EvaluationPeriods
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    ct);

        if (period == null)
        {
            return Result<EvaluationPeriodDto>.Failure(
                "Evaluation period not found.");
        }

        var result =
            _mapper.Map<EvaluationPeriodDto>(period);

        return Result<EvaluationPeriodDto>.Succeeded(result);
    }

    public async Task<Result<List<EvaluationPeriodDto>>> GetAllAsync(
    CancellationToken ct = default)
    {
        var periods =
            await _context.EvaluationPeriods
                .AsNoTracking()
                .OrderByDescending(x => x.StartDate)
                .ToListAsync(ct);

        var result =
            _mapper.Map<List<EvaluationPeriodDto>>(periods);

        return Result<List<EvaluationPeriodDto>>.Succeeded(result);
    }

    public async Task<Result<EvaluationPeriodDto>> UpdateAsync(int id,
     UpdateEvaluationPeriodDto dto,
     CancellationToken ct = default)
    {
        // =========================================================
        // Validation
        // =========================================================

        if (dto == null)
        {
            return Result<EvaluationPeriodDto>.Failure(
                "Evaluation period data is required.");
        }


        // =========================================================
        // Get Period
        // =========================================================

        var period =
            await _context.EvaluationPeriods
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    ct);

        if (period == null)
        {
            return Result<EvaluationPeriodDto>.Failure(
                "Evaluation period not found.");
        }


        // =========================================================
        // Validate Status
        // =========================================================

        if (!Enum.IsDefined(
            typeof(EvaluationPeriodStatus),
            period.Status))
        {
            return Result<EvaluationPeriodDto>.Failure(
                "The evaluation period has an invalid status.");
        }


        // =========================================================
        // Only Draft Can Be Modified
        // =========================================================

        if (period.Status != EvaluationPeriodStatus.Draft)
        {
            return Result<EvaluationPeriodDto>.Failure(
                "Only draft evaluation periods can be modified.");
        }


        // =========================================================
        // Validate Name
        // =========================================================

        if (dto.Name != null &&
            string.IsNullOrWhiteSpace(dto.Name))
        {
            return Result<EvaluationPeriodDto>.Failure(
                "Evaluation period name cannot be empty.");
        }


        // =========================================================
        // Validate Year / Month Pair
        // =========================================================

        var hasYear =
            dto.Year.HasValue;

        var hasMonth =
            dto.Month.HasValue;


        if (hasYear != hasMonth)
        {
            return Result<EvaluationPeriodDto>.Failure(
                "Year and month must be provided together.");
        }


        // =========================================================
        // Old Values
        // =========================================================

        var oldValues = new
        {
            period.Name,
            Year = period.StartDate.Year,
            Month = period.StartDate.Month,
            period.StartDate,
            period.EndDate,
            period.Status
        };


        // =========================================================
        // Partial Update - Name
        // =========================================================

        if (dto.Name != null)
        {
            period.Name =
                dto.Name.Trim();
        }


        // =========================================================
        // Partial Update - Year / Month
        // =========================================================

        if (hasYear && hasMonth)
        {
            var year =
                dto.Year!.Value;

            var month =
                dto.Month!.Value;


            // -----------------------------------------------------
            // Validate Year
            // -----------------------------------------------------

            if (!IsValidYear(year))
            {
                return Result<EvaluationPeriodDto>.Failure(
                    "Invalid year.");
            }


            // -----------------------------------------------------
            // Validate Month
            // -----------------------------------------------------

            if (!IsValidMonth(month))
            {
                return Result<EvaluationPeriodDto>.Failure(
                    "Invalid month.");
            }


            // -----------------------------------------------------
            // Check Duplicate Period
            // -----------------------------------------------------

            var periodExists =
                await PeriodExistsAsync(
                    year,
                    month,
                    id,
                    ct);

            if (periodExists)
            {
                return Result<EvaluationPeriodDto>.Failure(
                    "An evaluation period already exists for this year and month.");
            }


            // -----------------------------------------------------
            // Calculate Dates
            // -----------------------------------------------------

            var (startDate, endDate) =
                GetMonthDates(
                    year,
                    month);


            period.StartDate =
                startDate;

            period.EndDate =
                endDate;
        }


        // =========================================================
        // New Values
        // =========================================================

        var newValues = new
        {
            period.Name,
            Year = period.StartDate.Year,
            Month = period.StartDate.Month,
            period.StartDate,
            period.EndDate,
            period.Status
        };


        // =========================================================
        // Save
        // =========================================================

        await _context.SaveChangesAsync(ct);


        // =========================================================
        // Audit
        // =========================================================

        await _auditService.LogAsync(
            "Update",
            "EvaluationPeriod",
            period.Id.ToString(),
            oldValues,
            newValues);


        // =========================================================
        // Result
        // =========================================================

        var result = new EvaluationPeriodDto
        {
            Id = period.Id,
            Name = period.Name,
            StartDate = period.StartDate,
            EndDate = period.EndDate,
            Status = period.Status,
            IsActive = period.IsActive
        };


        return Result<EvaluationPeriodDto>.Succeeded(result);
    }

    public async Task<Result<bool>> DeleteAsync(int id,
      CancellationToken ct = default)
    {
        // =========================================================
        // Get Period
        // =========================================================

        var period =
            await _context.EvaluationPeriods
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    ct);

        if (period == null)
        {
            return Result<bool>.Failure(
                "Evaluation period not found.");
        }


        // =========================================================
        // Validate Status
        // =========================================================

        if (!Enum.IsDefined(
            typeof(EvaluationPeriodStatus),
            period.Status))
        {
            return Result<bool>.Failure(
                "The evaluation period has an invalid status.");
        }


        // =========================================================
        // Prevent Delete
        // =========================================================

        if (period.Status != EvaluationPeriodStatus.Draft)
        {
            return Result<bool>.Failure(
                "Only draft evaluation periods can be deleted.");
        }


        // =========================================================
        // Old Values
        // =========================================================

        var oldValues = new
        {
            period.Name,
            Year = period.StartDate.Year,
            Month = period.StartDate.Month,
            period.StartDate,
            period.EndDate,
            period.Status,
            period.IsActive,
            period.IsDeleted
        };


        // =========================================================
        // Soft Delete
        // =========================================================

        period.IsDeleted = true;
        period.IsActive = false;


        // =========================================================
        // Save
        // =========================================================

        await _context.SaveChangesAsync(ct);


        // =========================================================
        // Audit
        // =========================================================

        var newValues = new
        {
            period.Name,
            Year = period.StartDate.Year,
            Month = period.StartDate.Month,
            period.StartDate,
            period.EndDate,
            period.Status,
            period.IsActive,
            period.IsDeleted
        };

        await _auditService.LogAsync(
            "Delete",
            "EvaluationPeriod",
            period.Id.ToString(),
            oldValues,
            newValues);


        // =========================================================
        // Return
        // =========================================================

        return Result<bool>.Succeeded(true);
    }

    public async Task<Result<bool>> OpenAsync(
    int id,
    CancellationToken ct = default)
    {
        // =========================================================
        // Get Period
        // =========================================================

        var period =
            await _context.EvaluationPeriods
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    ct);

        if (period == null)
        {
            return Result<bool>.Failure(
                "Evaluation period not found.");
        }


        // =========================================================
        // Validate Status
        // =========================================================

        if (!Enum.IsDefined(
            typeof(EvaluationPeriodStatus),
            period.Status))
        {
            return Result<bool>.Failure(
                "The evaluation period has an invalid status.");
        }


        if (period.Status != EvaluationPeriodStatus.Draft)
        {
            return Result<bool>.Failure(
                "Only draft evaluation periods can be opened.");
        }


        // =========================================================
        // Validate Start Date
        // =========================================================

        var today =
       DateOnly.FromDateTime(DateTime.Today);

        if (today < period.StartDate)
        {
            return Result<bool>.Failure(
                $"The evaluation period cannot be opened before {period.StartDate:yyyy-MM-dd}.");
        }

        if (today > period.EndDate)
        {
            return Result<bool>.Failure(
                $"The evaluation period cannot be opened after {period.EndDate:yyyy-MM-dd}.");
        }


        // =========================================================
        // Old Values
        // =========================================================

        var oldValues = new
        {
            period.Status
        };


        // =========================================================
        // Open
        // =========================================================

        period.Status =
            EvaluationPeriodStatus.Open;


        // =========================================================
        // Save
        // =========================================================

        await _context.SaveChangesAsync(ct);


        // =========================================================
        // Audit
        // =========================================================

        var newValues = new
        {
            period.Status
        };

        await _auditService.LogAsync(
            "Open",
            "EvaluationPeriod",
            period.Id.ToString(),
            oldValues,
            newValues);


        return Result<bool>.Succeeded(true);
    }

    public async Task<Result<bool>> CloseAsync(int id,
    CancellationToken ct = default)
    {
        // =========================================================
        // Get Period
        // =========================================================

        var period =
            await _context.EvaluationPeriods
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    ct);

        if (period == null)
        {
            return Result<bool>.Failure(
                "Evaluation period not found.");
        }


        // =========================================================
        // Validate Status
        // =========================================================

        if (!Enum.IsDefined(
            typeof(EvaluationPeriodStatus),
            period.Status))
        {
            return Result<bool>.Failure(
                "The evaluation period has an invalid status.");
        }


        if (period.Status != EvaluationPeriodStatus.Open)
        {
            return Result<bool>.Failure(
                "Only open evaluation periods can be closed.");
        }


        // =========================================================
        // Validate End Date
        // =========================================================

        var today = DateOnly.FromDateTime(
            DateTime.Today);

        if (today < period.EndDate)
        {
            return Result<bool>.Failure(
                $"The evaluation period cannot be closed before {period.EndDate:yyyy-MM-dd}.");
        }


        // =========================================================
        // Old Values
        // =========================================================

        var oldValues = new
        {
            period.Status
        };


        // =========================================================
        // Close
        // =========================================================

        period.Status =
            EvaluationPeriodStatus.Closed;


        // =========================================================
        // Save
        // =========================================================

        await _context.SaveChangesAsync(ct);


        // =========================================================
        // Audit
        // =========================================================

        var newValues = new
        {
            period.Status
        };

        await _auditService.LogAsync(
            "Close",
            "EvaluationPeriod",
            period.Id.ToString(),
            oldValues,
            newValues);


        return Result<bool>.Succeeded(true);
    }

    private static bool IsValidYear(int year)
    {
        return year >= 1 && year <= 9999;
    }

    private static bool IsValidMonth(int month)
    {
        return month >= 1 && month <= 12;
    }

    private static (DateOnly StartDate, DateOnly EndDate) GetMonthDates(
        int year,
        int month)
    {
        var startDate =
            new DateOnly(
                year,
                month,
                1);

        var endDate =
            new DateOnly(
                year,
                month,
                DateTime.DaysInMonth(
                    year,
                    month));

        return (startDate, endDate);
    }

    private async Task<bool> PeriodExistsAsync(
    int year,
    int month,
    int? excludeId = null,
    CancellationToken ct = default)
    {
        var startDate =
            new DateOnly(
                year,
                month,
                1);

        return await _context.EvaluationPeriods
            .AnyAsync(
                x =>
                    x.StartDate == startDate &&
                    (!excludeId.HasValue ||
                     x.Id != excludeId.Value),
                ct);
    }
}
