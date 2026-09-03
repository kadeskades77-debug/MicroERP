using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Overtime;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Helper;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using MicroERP.Application.Features.EmployeeAttendance.AttendanceLogs.Services;
using MicroERP.Application.Features.Payrolls.SalaryComponents.Services;
using MicroERP.Domin.Entities.EmployeeAttendance;
using MicroERP.Domin.Entities.Policies;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Services;

public class EmployeeOvertimeService
    : IEmployeeOvertimeService
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _auditService;
    private readonly IOvertimeCalculationService _overtimeCalculation;
    public EmployeeOvertimeService(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IAuditService auditService,
        IOvertimeCalculationService overtimeCalculation)
    {
        _context = context;
        _currentUser = currentUser;
        _auditService = auditService;
        _overtimeCalculation = overtimeCalculation;
    }



   public async Task<Result> CreateManualOvertimeAsync(
    CreateManualOvertimeDto dto,
    CancellationToken cancellationToken = default)
    {
        // =========================================================
        // Validate Overtime Type
        // =========================================================

        if (!Enum.IsDefined(dto.Type))
        {
            return Result.Failure(
                "Invalid overtime type.");
        }


        // =========================================================
        // Validate Date/Time
        // =========================================================

        if (dto.StartDateTime >= dto.EndDateTime)
        {
            return Result.Failure(
                "Overtime start time must be before end time.");
        }


        // =========================================================
        // Employee
        // =========================================================

        var employee =
            await _context.Employees
                .Include(x => x.WorkSchedule)
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == dto.EmployeeId &&
                        x.IsActive,
                    cancellationToken);

        if (employee == null)
        {
            return Result.Failure(
                "Employee not found or inactive.");
        }


        // =========================================================
        // Work Schedule
        // =========================================================

        var workSchedule =
            employee.WorkSchedule;

        if (workSchedule == null)
        {
            return Result.Failure(
                "Employee does not have a work schedule.");
        }


        // =========================================================
        // Overtime Policy
        // =========================================================

        var overtimePolicy =
            await _context.OvertimePolicies
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.IsDefault,
                    cancellationToken);

        if (overtimePolicy == null)
        {
            return Result.Failure(
                "Default overtime policy was not found.");
        }


        // =========================================================
        // Calculate Total Minutes
        // =========================================================

        var totalMinutes =
            (int)(
                dto.EndDateTime -
                dto.StartDateTime)
            .TotalMinutes;

        if (totalMinutes <= 0)
        {
            return Result.Failure(
                "Overtime duration must be greater than zero.");
        }


        // =========================================================
        // Max Hours Per Day
        // =========================================================

        var maxMinutes =
            overtimePolicy.MaxHoursPerDay * 60;

        if (maxMinutes <= 0)
        {
            return Result.Failure(
                "Maximum overtime hours per day is not configured.");
        }


        if (totalMinutes > maxMinutes)
        {
            return Result.Failure(
                $"Overtime cannot exceed " +
                $"{overtimePolicy.MaxHoursPerDay} " +
                "hours per day.");
        }


        // =========================================================
        // Validate Outside Official Working Hours
        // =========================================================
        //
        // For an overnight overtime period, validate every
        // calendar-day portion against the employee's schedule.
        //

        var currentDate =
            DateOnly.FromDateTime(
                dto.StartDateTime);

        var endDate =
            DateOnly.FromDateTime(
                dto.EndDateTime);

        while (currentDate <= endDate)
        {
            var dayStart =
                currentDate == DateOnly.FromDateTime(
                    dto.StartDateTime)
                    ? dto.StartDateTime
                    : currentDate.ToDateTime(
                        TimeOnly.MinValue);

            var dayEnd =
                currentDate == DateOnly.FromDateTime(
                    dto.EndDateTime)
                    ? dto.EndDateTime
                    : currentDate.AddDays(1)
                        .ToDateTime(
                            TimeOnly.MinValue);

            var startTime =
        TimeOnly.FromDateTime(dayStart);

            var endTime =
                currentDate == endDate
                    ? TimeOnly.FromDateTime(dayEnd)
                    : TimeOnly.MaxValue;

            if (IsOverlappingOfficialShift(
                 dto.StartDateTime,
                 dto.EndDateTime,
                 workSchedule))
            {
                return Result.Failure(
                    "Manual overtime must be outside official working hours.");
            }

            currentDate =
                currentDate.AddDays(1);
        }


        // =========================================================
        // Prevent Duplicate / Overlapping Overtime
        // =========================================================

        var existingOvertimes =
            await _context.EmployeeOvertimes
                .AsNoTracking()
                .Where(
                    x =>
                        x.EmployeeId ==
                            dto.EmployeeId &&

                        x.Status !=
                            OvertimeStatus.Rejected &&

                        x.Status !=
                            OvertimeStatus.Cancelled &&

                        x.StartDateTime <
                            dto.EndDateTime &&

                        x.EndDateTime >
                            dto.StartDateTime)
                .Select(
                    x => x.Id)
                .AnyAsync(
                    cancellationToken);

        if (existingOvertimes)
        {
            return Result.Failure(
                "The overtime period overlaps with an existing overtime record.");
        }


        // =========================================================
        // Basic Salary
        // =========================================================

        var basicSalary =
            await _overtimeCalculation
                .GetBasicSalaryAsync(
                    dto.EmployeeId,
                    cancellationToken);

        if (basicSalary <= 0)
        {
            return Result.Failure(
                "Employee basic salary was not found.");
        }


        // =========================================================
        // Payroll Policy
        // =========================================================

        var payrollPolicy =
            await _context.PayrollPolicys
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.IsDefault,
                    cancellationToken);

        if (payrollPolicy == null)
        {
            return Result.Failure(
                "Default payroll policy was not found.");
        }
        

        var totalWorkingMinutes =
            payrollPolicy.WorkingDays *
            payrollPolicy.WorkingMinutesPerDay;

        if (totalWorkingMinutes <= 0)
        {
            return Result.Failure(
                "Payroll working minutes configuration is invalid.");
        }


        // =========================================================
        // Multiplier
        // =========================================================

        var multiplier =
            _overtimeCalculation
                .GetMultiplier(
                    overtimePolicy,
                    dto.Type);

        if (multiplier <= 0)
        {
            return Result.Failure(
                "Overtime multiplier is invalid.");
        }


        // =========================================================
        // Calculate Hourly Rate + Amount
        // =========================================================

        var calculated =
            _overtimeCalculation
                .CalculateOvertimeAmount(
                    totalMinutes,
                    multiplier,
                    basicSalary,
                    totalWorkingMinutes);


        // =========================================================
        // Create
        // =========================================================

        var overtime =
            new EmployeeOvertime
            {
                EmployeeId =
                    dto.EmployeeId,

                StartDateTime =
                    dto.StartDateTime,

                EndDateTime =
                    dto.EndDateTime,

                TotalMinutes =
                    totalMinutes,

                HourlyRate =
                    calculated.HourlyRate,

                Multiplier =
                    multiplier,

                Amount =
                    calculated.Amount,

                Type =
                    dto.Type,

                Status =
                    OvertimeStatus.Pending,

                Source =
                    OvertimeSource.Manual,

                Reason =
                    dto.Reason,

                IsPaid =
                    false,

                PayrollItemId =
                    null
            };


        await _context.EmployeeOvertimes
            .AddAsync(
                overtime,
                cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);


        return Result.Succeeded(
            "Manual overtime created successfully.");
    }


    public async Task<Result> UpdateOvertimeAsync(int id,
    UpdateOvertimeDto dto,
    CancellationToken cancellationToken = default)
    {
        var overtime =
            await _context.EmployeeOvertimes
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        var policy =
         await _context.OvertimePolicies
        .AsNoTracking()
        .FirstOrDefaultAsync(
            x => x.IsDefault,
            cancellationToken);

        if (policy == null)
        {
            return Result.Failure(
                "Default overtime policy not found.");
        }

        var maxMinutesPerDay =
        policy.MaxHoursPerDay * 60;


        if (overtime == null)
        {
            return Result.Failure(
                "Overtime record not found.");
        }

        // =========================================================
        // Payroll Policy
        // =========================================================

        // لا نسمح بتعديل القيم بعد الاعتماد
        if (overtime.Status != OvertimeStatus.Pending)
        {
            return Result.Failure(
                "Only pending overtime can be modified.");
        }


        // ---------------- Validation ----------------
        if (dto.Type.HasValue)
        {
            if (!Enum.IsDefined(typeof(OvertimeType), dto.Type.Value))
            {
                return Result.Failure(
                    "Invalid overtime type.");
            }
        }
        if (dto.HourlyRate.HasValue &&
            dto.HourlyRate.Value <= 0)
        {
            return Result.Failure(
                "Hourly rate must be greater than zero.");
        }

        if (dto.Multiplier.HasValue &&
            dto.Multiplier.Value <= 0)
        {
            return Result.Failure(
                "Multiplier must be greater than zero.");
        }

        if (dto.Amount.HasValue &&
            dto.Amount.Value < 0)
        {
            return Result.Failure(
                "Amount cannot be negative.");
        }
        if (HasMultipleCalculationChanges(dto))
        {
            return Result.Failure(
                "Only one calculation value can be changed at a time.");
        }
        
        // ---------------- Apply values ----------------

        var newStartDateTime =
            dto.StartDateTime ??
            overtime.StartDateTime;

        var newType =
           dto.Type ?? overtime.Type;

        var newEndDateTime = dto.EndDateTime ??
            overtime.EndDateTime;

        if (newEndDateTime <= newStartDateTime)
        {
            return Result.Failure(
                "End time must be greater than start time.");
        }

        var overlap =
       await _context.EmployeeOvertimes
       .AnyAsync(
           x =>
           x.Id != id &&
           x.EmployeeId == overtime.EmployeeId &&
           x.StartDateTime < newEndDateTime &&
           x.EndDateTime > newStartDateTime ,
           cancellationToken);

        var oldValues = new
        {

            overtime.StartDateTime,
            overtime.EndDateTime,
            overtime.TotalMinutes,
            overtime.Type,
            overtime.Reason,
            overtime.Multiplier,
            overtime.Amount
        };



        if (overlap)
        {
            return Result.Failure(
                "Overtime overlaps with another request.");
        }

        var totalMinutes =
       CalculateMinutes(
       newStartDateTime,
       newEndDateTime);

     


        var minutesValidation =
       ValidateOvertimeMinutes(totalMinutes,policy.MaxHoursPerDay);

        if (!minutesValidation.Success)
        {
            return minutesValidation;
        }

        var hourlyRate =
            dto.HourlyRate ??
            overtime.HourlyRate;

        var multiplier =
            dto.Multiplier ??
            overtime.Multiplier;

        var amount =
            overtime.Amount;


        // =================================================
        // 1. HourlyRate changed
        //    → calculate Amount
        // =================================================

        if (dto.HourlyRate.HasValue)
        {
            amount =
                OvertimeCalculationHelper.CalculateAmount(
                    totalMinutes,
                    hourlyRate,
                    multiplier);
        }


        // =================================================
        // 2. Multiplier changed
        //    → calculate Amount
        // =================================================

        else if (dto.Multiplier.HasValue)
        {
            amount =
                OvertimeCalculationHelper.CalculateAmount(
                    totalMinutes,
                    hourlyRate,
                    multiplier);
        }

        // =================================================
        // 2. Type changed→ Multiplier changed
        //    → calculate Amount
        // =================================================

        else if (dto.Type.HasValue)
        {
            
                  multiplier =
           _overtimeCalculation
               .GetMultiplier(
                   policy,
                   newType);
       
            amount =
             OvertimeCalculationHelper.CalculateAmount(
                    totalMinutes,
                    hourlyRate,
                    multiplier);
        }


        // =================================================
        // 3. Amount changed
        //    → calculate Multiplier
        //    → HourlyRate remains unchanged
        // =================================================

        else if (dto.Amount.HasValue)
        {
            amount =
                dto.Amount.Value;

            multiplier =
                OvertimeCalculationHelper.CalculateMultiplier(
                    totalMinutes,
                    hourlyRate,
                    amount);

            hourlyRate =
                OvertimeCalculationHelper.CalculateHourlyRate(
                    totalMinutes,
                    multiplier,
                    amount);
        }


        // =================================================
        // 4. TotalMinutes changed
        //    → calculate Amount
        // =================================================

        else if (dto.StartDateTime.HasValue || dto.EndDateTime.HasValue)
        {
            amount =
                OvertimeCalculationHelper.CalculateAmount(
                    totalMinutes,
                    hourlyRate,
                    multiplier);
        }


        // ---------------- Save ----------------
        overtime.StartDateTime = newStartDateTime;
        overtime.EndDateTime = newEndDateTime;
        overtime.TotalMinutes =totalMinutes;
        overtime.Type = newType;
        overtime.HourlyRate =hourlyRate;
        overtime.Multiplier =multiplier;
        overtime.Amount =amount;
        if (dto.Reason != null)
        {
            overtime.Reason =
                dto.Reason;
        }
        var newValues = new
        {
            overtime.StartDateTime,
            overtime.EndDateTime,
            overtime.TotalMinutes,
            overtime.Type,
            overtime.Reason,
            overtime.Multiplier,
            overtime.Amount
        };


        await _auditService.LogAsync(
            "Update",
            "EmployeeOvertime",
            overtime.Id.ToString(),
            oldValues,
            newValues);
        await _context.SaveChangesAsync(
          cancellationToken);

        return Result.Succeeded(
        "Overtime request updated successfully.");
    }

    public async Task<Result> ApproveAsync(int id,
    CancellationToken cancellationToken = default)
    {
        var overtime =
            await _context.EmployeeOvertimes
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);



        if (overtime == null)
        {
            return Result.Failure(
                "Overtime request not found.");
        }



        if (overtime.Status != OvertimeStatus.Pending)
        {
            return Result.Failure(
                "Only pending requests can be approved.");
        }

        var oldValues = new
        {
            overtime.Status
        };

        overtime.Status =
            OvertimeStatus.Approved;


        overtime.ApprovedOn =
            DateTime.UtcNow;


        overtime.ApprovedByUserId =
            _currentUser.UserId;

        var newValues = new
        {
            overtime.Status,
            overtime.ApprovedOn,
            overtime.ApprovedByUserId
        };


        await _auditService.LogAsync(
            "Approve",
            "EmployeeOvertime",
            overtime.Id.ToString(),
            oldValues,
            newValues);

        await _context.SaveChangesAsync(
            cancellationToken);



        return Result.Succeeded(
      "Overtime request approved successfully.");
    }

    public async Task<Result> RejectAsync(int id,
    string rejectionReason,
    CancellationToken cancellationToken = default)
    {
        var overtime =
            await _context.EmployeeOvertimes
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);



        if (overtime == null)
        {
            return Result.Failure(
                "Overtime request not found.");
        }



        if (overtime.Status != OvertimeStatus.Pending)
        {
            return Result.Failure(
                "Only pending requests can be rejected.");
        }

        var oldValues = new
        {
            overtime.Status
        };

        overtime.Status =
            OvertimeStatus.Rejected;


        overtime.RejectedOn =
            DateTime.UtcNow;


        overtime.RejectedByUserId =
            _currentUser.UserId;


        overtime.RejectionReason =
            rejectionReason;

        var newValues = new
        {
            overtime.Status,
            overtime.RejectionReason,
            overtime.RejectedOn,
            overtime.RejectedByUserId
        };


        await _auditService.LogAsync(
            "Reject",
            "EmployeeOvertime",
            overtime.Id.ToString(),
            oldValues,
            newValues);

        await _context.SaveChangesAsync(
            cancellationToken);



        return Result.Succeeded(
       "Overtime request rejected successfully.");
    }

    public async Task<Result> CancelAsync(
     int id,
     CancelEmployeeOvertimeDto dto,
     CancellationToken cancellationToken = default)
    {
        var overtime =
            await _context.EmployeeOvertimes
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);


        if (overtime == null)
        {
            return Result.Failure(
                "Overtime request not found.");
        }


        if (overtime.Status != OvertimeStatus.Pending)
        {
            return Result.Failure(
                "Only pending overtime can be cancelled.");
        }
        var oldValues = new
        {
            overtime.Status
        };

        overtime.Status = OvertimeStatus.Cancelled;

        overtime.CancellationReason =
            dto.Reason;


        overtime.CancelledOn =
            DateTime.UtcNow;


        overtime.CancelledByUserId =
            _currentUser.UserId;
        var newValues = new
        {
            overtime.Status,
            overtime.CancellationReason,
            overtime.CancelledOn,
            overtime.CancelledByUserId
        };


        await _auditService.LogAsync(
            "Cancel",
            "EmployeeOvertime",
            overtime.Id.ToString(),
            oldValues,
            newValues);

        await _context.SaveChangesAsync(
            cancellationToken);


        return Result.Succeeded(
            "Overtime request cancelled successfully.");
    }

    public async Task<Result> DeleteAsync(int id,
    CancellationToken cancellationToken = default)
    {
        var overtime =
            await _context.EmployeeOvertimes
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);



        if (overtime == null)
        {
            return Result.Failure(
                "Overtime request not found.");
        }



        if (overtime.Status != OvertimeStatus.Pending)
        {
            return Result.Failure(
                "Only pending overtime can be deleted.");
        }

        if (overtime.IsPaid)
        {
            return Result.Failure(
                "Paid overtime cannot be deleted.");
        }

        var oldValues = new
        {
            overtime.EmployeeId,
            overtime.StartDateTime,                                    
            overtime.TotalMinutes,
            overtime.Amount,
            overtime.Status,
            overtime.IsPaid
        };

        overtime.IsDeleted = true;

        overtime.IsActive = false;

        var newValues = new
        {
            overtime.IsDeleted,
            overtime.IsActive
        };


        await _auditService.LogAsync(
            "Delete",
            "EmployeeOvertime",
            overtime.Id.ToString(),
            oldValues,
            newValues);

        await _context.SaveChangesAsync(
            cancellationToken);



        return Result.Succeeded(
     "Overtime request deleted successfully.");
    }


    //===================== Helpers ===========

    private static int CalculateMinutes(
    DateTime startDateTime,
    DateTime endDateTime)
    {
        if (endDateTime <= startDateTime)
            return 0;

        return (int)(endDateTime - startDateTime).TotalMinutes;
    }

    private static bool HasMultipleCalculationChanges(
    UpdateOvertimeDto dto)
    {
        var count = 0;

        if (dto.StartDateTime.HasValue|| dto.EndDateTime.HasValue||dto.Type.HasValue)
            count++;

        if (dto.HourlyRate.HasValue)
            count++;

        if (dto.Multiplier.HasValue)
            count++;

        if (dto.Amount.HasValue)
            count++;

        return count > 1;
    }

    private static Result ValidateOvertimeMinutes(
    int totalMinutes,
    decimal maxHoursPerDay)
    {
        if (totalMinutes <= 0)
        {
            return Result.Failure(
                "Total minutes must be greater than zero.");
        }

        var maxMinutes =
            maxHoursPerDay * 60;

        if (totalMinutes > maxMinutes)
        {
            return Result.Failure(
                $"Total overtime cannot exceed " +
                $"{maxHoursPerDay} hours per day.");
        }

        return Result.Succeeded();
    }

    private static bool IsOverlappingOfficialShift(
      DateTime startDateTime,
      DateTime endDateTime,
      WorkSchedule schedule)
    {
        if (startDateTime >= endDateTime)
        {
            return false;
        }

        var currentDate =
            DateOnly.FromDateTime(startDateTime);

        var endDate =
            DateOnly.FromDateTime(endDateTime);

        while (currentDate <= endDate)
        {
            // =====================================================
            // First Shift
            // =====================================================

            var firstShiftStart =
                currentDate.ToDateTime(
                    schedule.FirstShiftStart);

            var firstShiftEnd =
                CreateShiftEndDateTime(
                    currentDate,
                    schedule.FirstShiftStart,
                    schedule.FirstShiftEnd);

            if (IsOverlapping(
                    startDateTime,
                    endDateTime,
                    firstShiftStart,
                    firstShiftEnd))
            {
                return true;
            }


            // =====================================================
            // Second Shift
            // =====================================================

            if (schedule.SecondShiftStart.HasValue &&
                schedule.SecondShiftEnd.HasValue)
            {
                var secondShiftStart =
                    currentDate.ToDateTime(
                        schedule.SecondShiftStart.Value);

                var secondShiftEnd =
                    CreateShiftEndDateTime(
                        currentDate,
                        schedule.SecondShiftStart.Value,
                        schedule.SecondShiftEnd.Value);

                if (IsOverlapping(
                        startDateTime,
                        endDateTime,
                        secondShiftStart,
                        secondShiftEnd))
                {
                    return true;
                }
            }

            currentDate =
                currentDate.AddDays(1);
        }

        return false;
    }

    private static DateTime CreateShiftEndDateTime(
    DateOnly date,
    TimeOnly shiftStart,
    TimeOnly shiftEnd)
    {
        var start =
            date.ToDateTime(shiftStart);

        var end =
            date.ToDateTime(shiftEnd);

        // الشفت يعبر منتصف الليل
        if (shiftEnd <= shiftStart)
        {
            end = end.AddDays(1);
        }

        return end;
    }

    private static bool IsOverlapping(
     DateTime start,
     DateTime end,
     DateTime shiftStart,
     DateTime shiftEnd)
    {
        return start < shiftEnd &&
               end > shiftStart;
    }

}