using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using MicroERP.Application.Features.EmployeeAttendance.AttendanceLogs.Calculators;
using MicroERP.Application.Features.EmployeeAttendance.AttendanceLogs.Interfaces;
using MicroERP.Domin.Entities.EmployeeAttendance;
using MicroERP.Domin.Entities.Policies;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.EmployeeAttendance.AttendanceLogs.Services;

public class AttendanceOvertimeService
    : IAttendanceOvertimeService
{
    private readonly IApplicationDbContext _context;
    private readonly IOvertimeCalculationService _overtimeCalculation;

    public AttendanceOvertimeService(
        IApplicationDbContext context, IOvertimeCalculationService overtimeCalculation)
    {
        _context = context;
        _overtimeCalculation = overtimeCalculation;
    }



    public async Task CreateAsync(
       AttendanceRecord attendance,
       ShiftCalculationResult firstShift,
       ShiftCalculationResult? secondShift,
       CancellationToken cancellationToken = default)
    {
        // =========================================================
        // Ignore non-working statuses
        // =========================================================

        if (attendance.Status == AttendanceStatus.OnLeave ||
            attendance.Status == AttendanceStatus.Holiday ||
            attendance.Status == AttendanceStatus.Weekend)
        {
            return;
        }

        // =========================================================
        // Calculate daily overtime from attendance
        //
        // Example:
        // Worked   = 616
        // Expected = 480
        // Overtime = 136
        // =========================================================

        var totalOvertimeMinutes =
            attendance.WorkedMinutes -
            attendance.ExpectedMinutes;

        if (totalOvertimeMinutes <= 0)
            return;


        // =========================================================
        // Get overtime shifts
        //
        // We use the overtime period itself to determine
        // how many overtime minutes are available in each shift.
        // =========================================================

        var shifts = new[]
        {
        firstShift,
        secondShift
    }
        .Where(x =>
            x != null &&
            x.OvertimeStart.HasValue &&
            x.CheckOut.HasValue &&
            x.CheckOut.Value > x.OvertimeStart.Value)
        .Select(x => x!)
        .OrderBy(x => x.OvertimeStart)
        .ToList();

        if (shifts.Count == 0)
            return;


        // =========================================================
        // Calculate available overtime minutes from shifts
        // =========================================================

        var availableShiftOvertimeMinutes =
            shifts.Sum(x =>
                (int)(x.CheckOut!.Value - x.OvertimeStart!.Value)
                    .TotalMinutes);

        if (availableShiftOvertimeMinutes <= 0)
            return;


        // =========================================================
        // Prevent inconsistency between attendance and shifts
        //
        // If Attendance says 136 overtime minutes but the shifts
        // only contain 100 minutes, we cannot safely create 136
        // minutes because we do not know their actual period.
        // =========================================================

        if (availableShiftOvertimeMinutes < totalOvertimeMinutes)
        {
            throw new InvalidOperationException(
                $"Attendance overtime ({totalOvertimeMinutes} minutes) " +
                $"exceeds the available overtime periods " +
                $"from shifts ({availableShiftOvertimeMinutes} minutes) " +
                $"for employee {attendance.EmployeeId} on {attendance.Date}.");
        }


        // =========================================================
        // Previous monthly balance
        //
        // Positive = previous surplus
        // Negative = previous lost time
        // =========================================================

        var previousBalance =
            await GetPreviousBalanceAsync(
                attendance,
                cancellationToken);


        // =========================================================
        // Previous lost time
        //
        // Only negative balance affects today's overtime.
        //
        // Positive previous balance does NOT reduce today's overtime.
        // =========================================================

        var previousLostMinutes =
            previousBalance < 0
                ? Math.Abs(previousBalance)
                : 0;


        // =========================================================
        // Calculate eligible overtime
        //
        // If there is no previous lost time:
        //
        //     today's overtime = eligible overtime
        //
        // If there is previous lost time:
        //
        //     today's overtime - previous lost time
        // =========================================================

        var eligibleOvertimeMinutes =
            Math.Max(
                totalOvertimeMinutes -
                previousLostMinutes,
                0);

        if (eligibleOvertimeMinutes <= 0)
            return;


        var remainingEligibleOvertime =
            eligibleOvertimeMinutes;


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
            throw new InvalidOperationException(
                "Default overtime policy was not found.");
        }


        // =========================================================
        // Maximum Overtime
        // =========================================================

        var maxMinutesPerDay =
            overtimePolicy.MaxHoursPerDay * 60;

        if (totalOvertimeMinutes > maxMinutesPerDay)
        {
            throw new InvalidOperationException(
                $"Overtime cannot exceed " +
                $"{overtimePolicy.MaxHoursPerDay} " +
                "hours per day.");
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
            throw new InvalidOperationException(
                "Default payroll policy was not found.");
        }


        // =========================================================
        // Working Minutes
        // =========================================================

        var totalWorkingMinutes =
            payrollPolicy.WorkingDays *
            payrollPolicy.WorkingMinutesPerDay;

        if (totalWorkingMinutes <= 0)
        {
            throw new InvalidOperationException(
                "Payroll working minutes configuration is invalid.");
        }


        // =========================================================
        // Basic Salary
        // =========================================================

        var basicSalary =
            await _overtimeCalculation.GetBasicSalaryAsync(
                attendance.EmployeeId,
                cancellationToken);

        if (basicSalary <= 0)
        {
            throw new InvalidOperationException(
                $"Basic salary was not found " +
                $"for employee {attendance.EmployeeId}.");
        }


        // =========================================================
        // Overtime Type
        // =========================================================

        var overtimeType =
            OvertimeType.Normal;


        // =========================================================
        // Multiplier
        // =========================================================

        var multiplier =
            _overtimeCalculation.GetMultiplier(
                overtimePolicy,
                overtimeType);


        // =========================================================
        // Create Overtime Per Shift
        // =========================================================

        foreach (var shift in shifts)
        {
            if (remainingEligibleOvertime <= 0)
                break;


            // =====================================================
            // Available overtime in this shift
            // =====================================================

            var shiftOvertimeMinutes =
                (int)(
                    shift.CheckOut!.Value -
                    shift.OvertimeStart!.Value)
                    .TotalMinutes;

            if (shiftOvertimeMinutes <= 0)
                continue;


            // =====================================================
            // Take only the eligible part
            // =====================================================

            var eligibleMinutes =
                Math.Min(
                    shiftOvertimeMinutes,
                    remainingEligibleOvertime);

            if (eligibleMinutes <= 0)
                continue;


            // =====================================================
            // Overtime Period
            //
            // We take the last eligibleMinutes from the overtime
            // period.
            //
            // Example:
            //
            // OvertimeStart = 12:00
            // CheckOut      = 12:40
            // Eligible      = 30
            //
            // Result:
            // 12:10 -> 12:40
            // =====================================================

            var checkOutTime =
                shift.CheckOut.Value;

            var overtimeStart =
                checkOutTime.AddMinutes(
                    -eligibleMinutes);


            // =====================================================
            // Duplicate Check
            // =====================================================

            var alreadyExists =
                await _context.EmployeeOvertimes
                    .AnyAsync(
                        x =>
                            x.EmployeeId ==
                                attendance.EmployeeId &&

                            x.Source ==
                                OvertimeSource.Attendance &&

                            x.StartDateTime ==
                                overtimeStart &&

                            x.EndDateTime ==
                                checkOutTime,
                        cancellationToken);

            if (alreadyExists)
            {
                // This overtime has already been generated.
                // Count it as processed so it isn't recreated.
                remainingEligibleOvertime -=
                    eligibleMinutes;

                continue;
            }


            // =====================================================
            // Calculate Amount
            // =====================================================

            var calculated =
                _overtimeCalculation
                    .CalculateOvertimeAmount(
                        eligibleMinutes,
                        multiplier,
                        basicSalary,
                        totalWorkingMinutes);


            // =====================================================
            // Create Entity
            // =====================================================

            var overtime =
                new EmployeeOvertime
                {
                    EmployeeId =
                        attendance.EmployeeId,

                    StartDateTime =
                        overtimeStart,

                    EndDateTime =
                        checkOutTime,

                    TotalMinutes =
                        eligibleMinutes,

                    HourlyRate =
                        calculated.HourlyRate,

                    Multiplier =
                        multiplier,

                    Amount =
                        calculated.Amount,

                    Type =
                        overtimeType,

                    Status =
                        OvertimeStatus.Pending,

                    Source =
                        OvertimeSource.Attendance,

                    IsPaid =
                        false,

                    PayrollItemId =
                        null,

                    Reason =
                        "Automatically generated from attendance."
                };


            await _context.EmployeeOvertimes
                .AddAsync(
                    overtime,
                    cancellationToken);


            // =====================================================
            // Remaining overtime
            // =====================================================

            remainingEligibleOvertime -=
                eligibleMinutes;
        }


        // =========================================================
        // Safety Check
        // =========================================================

        if (remainingEligibleOvertime > 0)
        {
            throw new InvalidOperationException(
                $"Could not allocate all eligible overtime. " +
                $"Remaining: {remainingEligibleOvertime} minutes " +
                $"for employee {attendance.EmployeeId} " +
                $"on {attendance.Date}.");
        }


        // =========================================================
        // Save
        // =========================================================

        await _context.SaveChangesAsync(
            cancellationToken);
    }

    public async Task CreateHolidayWeekendOvertimeAsync(
    AttendanceRecord attendance,
    CancellationToken cancellationToken = default)
    {
        // فقط الجمعة أو الإجازة الرسمية
        if (attendance.Status != AttendanceStatus.Weekend &&
            attendance.Status != AttendanceStatus.Holiday)
        {
            return;
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
            throw new Exception(
                "Default overtime policy was not found.");
        }


        // =========================================================
        // Minimum Work Minutes
        // =========================================================

        if (attendance.WorkedMinutes <=
            overtimePolicy.MinimumWeekendHolidayWorkMinutes)
        {
            return;
        }


        // =========================================================
        // Overtime Type
        // =========================================================

        var overtimeType =
            attendance.Status ==
                AttendanceStatus.Weekend
                    ? OvertimeType.Weekend
                    : OvertimeType.Holiday;


        // =========================================================
        // Prevent Duplicate
        // =========================================================

        var startDateTime = attendance.Date.ToDateTime(TimeOnly.MinValue);
        var endDateTime = attendance.Date.AddDays(1).ToDateTime(TimeOnly.MinValue);

        var alreadyExists =
            await _context.EmployeeOvertimes
                .AnyAsync(
                    x =>
                        x.EmployeeId == attendance.EmployeeId &&
                        x.StartDateTime >= startDateTime &&
                        x.StartDateTime < endDateTime,
                    cancellationToken);

        if (alreadyExists)
            return;


        // =========================================================
        // Check In
        // =========================================================

        var firstCheckIn =
            await _context.AttendanceLogs
                .Where(x =>
                    x.EmployeeId ==
                        attendance.EmployeeId &&

                    DateOnly.FromDateTime(
                        x.LogTime) ==
                        attendance.Date &&

                    x.Type ==
                        AttendanceLogType.CheckIn)
                .OrderBy(
                    x => x.LogTime)
                .FirstOrDefaultAsync(
                    cancellationToken);


        // =========================================================
        // Check Out
        // =========================================================

        var lastCheckOut =
            await _context.AttendanceLogs
                .Where(x =>
                    x.EmployeeId ==
                        attendance.EmployeeId &&

                    DateOnly.FromDateTime(
                        x.LogTime) ==
                        attendance.Date &&

                    x.Type ==
                        AttendanceLogType.CheckOut)
                .OrderByDescending(
                    x => x.LogTime)
                .FirstOrDefaultAsync(
                    cancellationToken);


        if (firstCheckIn == null ||
            lastCheckOut == null)
        {
            return;
        }


        // =========================================================
        // Validate Minutes
        // =========================================================

      
        var maxMinutesPerDay =
    overtimePolicy.MaxHoursPerDay * 60;

        var totalMinutes =
            Math.Min(
                attendance.WorkedMinutes,
                maxMinutesPerDay);

        if (totalMinutes <= 0)
        {
            return;
        }


        // =========================================================
        // Get Basic Salary
        // =========================================================

        var basicSalary =
            await _overtimeCalculation.GetBasicSalaryAsync(
                attendance.EmployeeId,
                cancellationToken);


        if (basicSalary <= 0)
        {
            throw new Exception(
                $"Basic salary was not found " +
                $"for employee " +
                $"{attendance.EmployeeId}.");
        }


        // =========================================================
        // Get Payroll Policy
        // =========================================================

        var payrollPolicy =
            await _context.PayrollPolicys
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.IsDefault,
                    cancellationToken);


        if (payrollPolicy == null)
        {
            throw new Exception(
                "Default payroll policy was not found.");
        }


        var totalWorkingMinutes =
            payrollPolicy.WorkingDays *
            payrollPolicy.WorkingMinutesPerDay;


        if (totalWorkingMinutes <= 0)
        {
            throw new Exception(
                "Payroll working minutes configuration is invalid.");
        }


        // =========================================================
        // Get Multiplier
        // =========================================================

        var multiplier =
            _overtimeCalculation.GetMultiplier(
                overtimePolicy,
                overtimeType);


        // =========================================================
        // Calculate Hourly Rate + Amount
        // =========================================================

        var calculated =
            _overtimeCalculation.CalculateOvertimeAmount(
                totalMinutes,
                multiplier,
                basicSalary,
                totalWorkingMinutes);


        // =========================================================
        // Create Overtime
        // =========================================================

        var overtime =
            new EmployeeOvertime
            {
                EmployeeId =
                    attendance.EmployeeId,

                StartDateTime =
                        firstCheckIn.LogTime,

                EndDateTime =
                        lastCheckOut.LogTime,

                TotalMinutes =
                    totalMinutes,

                HourlyRate =
                    calculated.HourlyRate,

                Multiplier =
                    multiplier,

                Amount =
                      calculated.Amount,

                Type =
                    overtimeType,

                Status =
                    OvertimeStatus.Pending,
                Source =
          OvertimeSource.Attendance,

                IsPaid =
                    false,

                PayrollItemId =
                    null,

                Reason =
                    overtimeType ==
                        OvertimeType.Weekend
                            ? "عمل أثناء العطلة الأسبوعية"
                            : "عمل أثناء الإجازة الرسمية"
            };


        // =========================================================
        // Save
        // =========================================================

        await _context.EmployeeOvertimes
            .AddAsync(
                overtime,
                cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);
    }

    private async Task<int> GetPreviousBalanceAsync(
        AttendanceRecord attendance,
        CancellationToken cancellationToken = default)
    {
        var firstDayOfMonth =
            new DateOnly(
                attendance.Date.Year,
                attendance.Date.Month,
                1);

        var previousDay =
            attendance.Date.AddDays(-1);

        if (previousDay < firstDayOfMonth)
            return 0;

        var balance =
            await _context.AttendanceRecords
                .AsNoTracking()
                .Where(x =>
                    x.EmployeeId == attendance.EmployeeId &&
                    x.Date >= firstDayOfMonth &&
                    x.Date <= previousDay &&
                    x.Status != AttendanceStatus.Weekend &&
                    x.Status != AttendanceStatus.Holiday)
                .SumAsync(
                    x => x.WorkedMinutes - x.ExpectedMinutes,
                    cancellationToken);

        return balance;
    }

}