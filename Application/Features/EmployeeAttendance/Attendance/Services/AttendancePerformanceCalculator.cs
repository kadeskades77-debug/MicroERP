using MicroERP.Application.Common.Interfaces;
using MicroERP.Domin.Entities.EmployeeAttendance;
using MicroERP.Domin.Entities.EmployeeLeaves;
using MicroERP.Domin.Entities.Policies;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Services;

public class AttendancePerformanceCalculator
{
    private readonly IApplicationDbContext _context;

    public AttendancePerformanceCalculator(IApplicationDbContext context)
    {
        _context = context;
    }

       public async Task<AttendancePerformanceResult> CalculateAsync(
        IEnumerable<AttendanceRecord> records,
        IEnumerable<EmployeeLeave> leaves,
        DateOnly periodStart,
        DateOnly periodEnd,
        AttendancePolicy policy)
    {
        var result =
            new AttendancePerformanceResult();


        decimal score =
            policy.MaximumPerformanceScore;


        // =========================================================
        // Attendance Records
        // =========================================================

        var attendanceRecords =
            records
                .Where(record =>
                    record.Date >= periodStart &&
                    record.Date <= periodEnd&&
                    record.Status != AttendanceStatus.Holiday &&
                    record.Status != AttendanceStatus.Weekend)
                .ToList();


        // =========================================================
        // Approved Leaves
        // =========================================================

        var approvedLeaves =
            leaves
                .Where(leave =>
                    leave.Status == LeaveStatus.Approved &&

                    leave.StartDate <= periodEnd &&

                    leave.EndDate >= periodStart)
                .ToList();


        // =========================================================
        // Leave Statistics
        // =========================================================

        foreach (var leave in approvedLeaves)
        {
            var leaveStart =
             leave.StartDate < periodStart
             ? periodStart
             : leave.StartDate;

            var leaveEnd =
                leave.EndDate > periodEnd
                    ? periodEnd
                    : leave.EndDate;

            if (leaveStart > leaveEnd)
                continue;


            var daysInPeriod =
     await CalculateLeaveDaysAsync(
         leave.EmployeeId,
         leaveStart,
         leaveEnd);


            switch (leave.LeaveType)
            {
                // =====================================================
                // Annual Leave
                // =====================================================

                case LeaveType.Annual:

                    result.AnnualLeaveDays +=
                        daysInPeriod;

                    break;


                // =====================================================
                // Sick Leave
                // =====================================================

                case LeaveType.Sick:

                    result.SickLeaveDays +=
                        daysInPeriod;

                    break;


                // =====================================================
                // Emergency Leave
                // =====================================================
                //
                // إحصائية فقط.
                // لا يوجد أي خصم.
                // =====================================================

                case LeaveType.Emergency:

                    result.EmergencyLeaveDays +=
                        daysInPeriod;

                    break;


                // =====================================================
                // Unpaid Leave
                // =====================================================

                case LeaveType.Unpaid:

                    result.UnpaidLeaveDays +=
                        daysInPeriod;

                    break;
            }
        }


        // =========================================================
        // Annual Leave Penalty
        // =========================================================
        //
        // مثال:
        //
        // MonthlyAnnualLeaveLimit = 5
        // AnnualLeaveDays = 7
        //
        // Exceeded = 2
        // =========================================================

        if (result.AnnualLeaveDays >
            policy.MonthlyAnnualLeaveLimit)
        {
            var exceededDays =
                result.AnnualLeaveDays -
                policy.MonthlyAnnualLeaveLimit;


            ApplyPenalty(
                ref score,
                result,
                exceededDays *
                policy.AnnualLeavePenaltyPoints);
        }


        // =========================================================
        // Sick Leave Penalty
        // =========================================================
     

        if (result.SickLeaveDays >
            policy.MonthlySickLeaveLimit)
        {
            var exceededDays =
                result.SickLeaveDays -
                policy.MonthlySickLeaveLimit;


            ApplyPenalty(
                ref score,
                result,
                exceededDays *
                policy.SickLeavePenaltyPoints);
        }


        // =========================================================
        // Unpaid Leave Penalty
        // =========================================================
        //
        // كل يوم إجازة غير مدفوعة يؤثر على التقييم.
        // =========================================================

        if (result.UnpaidLeaveDays > 0 &&
            policy.UnpaidLeavePenaltyPoints > 0)
        {
            var penalty =
                result.UnpaidLeaveDays *
                policy.UnpaidLeavePenaltyPoints;


            ApplyPenalty(
                ref score,
                result,
                penalty);
        }


        // =========================================================
        // Daily Attendance Calculations
        // =========================================================

        foreach (var record in attendanceRecords)
        {
            // =====================================================
            // Weekend / Holiday / Leave
            // =====================================================

            if (record.Status == AttendanceStatus.Weekend ||
                record.Status == AttendanceStatus.Holiday)
            {
                continue;
            }


            // =====================================================
            // Daily Status
            // =====================================================

            switch (record.Status)
            {
                case AttendanceStatus.Present:

                    result.PresentDays++;

                    break;


                case AttendanceStatus.Absent:

                    result.AbsentDays++;

                    ApplyPenalty(
                        ref score,
                        result,
                        policy.AbsentPenaltyPoints);

                    break;


                case AttendanceStatus.PartialAttendance:

                    result.PartialAttendanceDays++;

                    ApplyPenalty(
                        ref score,
                        result,
                        policy.PartialAttendancePenaltyPoints);

                    break;


                case AttendanceStatus.MissingCheckIn:

                    result.MissingCheckInCount++;

                    ApplyPenalty(
                        ref score,
                        result,
                        policy.MissingCheckInPenaltyPoints);

                    break;


                case AttendanceStatus.MissingCheckOut:

                    result.MissingCheckOutCount++;

                    ApplyPenalty(
                        ref score,
                        result,
                        policy.MissingCheckOutPenaltyPoints);

                    break;
            }


            // =====================================================
            // Aggregate Late Minutes
            // =====================================================

            result.TotalLateMinutes +=
                record.LateMinutes;


            // =====================================================
            // Aggregate Early Leave Minutes
            // =====================================================

            result.TotalEarlyLeaveMinutes +=
                record.EarlyLeaveMinutes;


            // =====================================================
            // Early Leave Days
            // =====================================================

            if (record.EarlyLeaveMinutes > 0)
            {
                result.EarlyLeaveDays++;
            }
        }


        // =========================================================
        // FINAL LATE MINUTES
        // =========================================================
      

        var lateMinutes =
            Math.Max(
                0,
                result.TotalLateMinutes -
                policy.LateGraceMinutes);
        Console.WriteLine(
    $"Chargeable Late Minutes = {lateMinutes}");


        if (lateMinutes > 0 &&
            policy.LateMinutesPerPenaltyPoint > 0)
        {
            var penalty =
                (decimal)lateMinutes /
                policy.LateMinutesPerPenaltyPoint;

            ApplyPenalty(
                ref score,
                result,
                penalty);
        }


        // =========================================================
        // FINAL LOST TIME
        // =========================================================

        var totalExpectedMinutes =
            attendanceRecords
                .Sum(x => x.ExpectedMinutes);

        
        var totalWorkedMinutes =
            attendanceRecords
                .Sum(x => x.WorkedMinutes);


        result.TotalLostTimeMinutes =
            Math.Max(
                0,
                totalExpectedMinutes -
                totalWorkedMinutes);


        // =========================================================
        // LOST TIME PENALTY
        // =========================================================

        var lostMinutes =
            Math.Max(
                0,
                result.TotalLostTimeMinutes -
                policy.LostTimeGraceMinutes);


        if (lostMinutes > 0 &&
            policy.LostMinutesPerPenaltyPoint > 0)
        {
            var penalty =
                (decimal)lostMinutes /
                policy.LostMinutesPerPenaltyPoint;


            ApplyPenalty(
                ref score,
                result,
                penalty);
        }


        // =========================================================
        // EARLY LEAVE PENALTY
        // =========================================================

        if (result.EarlyLeaveDays >
            policy.MonthlyEarlyLeaveLimit)
        {
            var exceededDays =
                result.EarlyLeaveDays -
                policy.MonthlyEarlyLeaveLimit;


            ApplyPenalty(
                ref score,
                result,
                exceededDays *
                policy.EarlyLeavePenaltyPoints);
        }


        // =========================================================
        // Final Score
        // =========================================================

        result.FinalScore =
       (int)Math.Round(
           Math.Clamp(
               score,
               policy.MinimumPerformanceScore,
               policy.MaximumPerformanceScore),
           MidpointRounding.AwayFromZero);


        return result;
    }


    

    private static void ApplyPenalty(
        ref decimal score,
        AttendancePerformanceResult result,
        decimal penalty)
    {
        score -= penalty;

        result.TotalPenaltyPoints += penalty;
    }

    private async Task<int> CalculateLeaveDaysAsync(
        int employeeId,
        DateOnly startDate,
        DateOnly endDate)
    {
        var holidays =
            await _context.Holidays
                .Where(x =>
                    x.StartDate <= endDate &&
                    x.EndDate >= startDate)
                .ToListAsync();

        var specialLeaves =
            await _context.EmployeeSpecialLeaves
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    x.Status == SpecialLeaveStatus.Approved &&
                    x.StartDate <= endDate &&
                    x.EndDate >= startDate)
                .ToListAsync();

        var totalDays = 0;

        for (var date = startDate;
             date <= endDate;
             date = date.AddDays(1))
        {
            // استبعاد الجمعة
            if (date.DayOfWeek == DayOfWeek.Friday)
                continue;

            // استبعاد العطل الرسمية
            if (holidays.Any(x =>
                    x.StartDate <= date &&
                    x.EndDate >= date))
            {
                continue;
            }

            // استبعاد الإجازات الخاصة
            if (specialLeaves.Any(x =>
                    x.StartDate <= date &&
                    x.EndDate >= date))
            {
                continue;
            }

            totalDays++;
        }

        return totalDays;
    }
}