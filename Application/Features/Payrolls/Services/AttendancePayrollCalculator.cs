using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Features.Payrolls.Interfaces;
using MicroERP.Domin.Entities.Payrolls;
using MicroERP.Domin.Entities.Policies;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.Payrolls.Services;

public class AttendancePayrollCalculator
    : IAttendancePayrollCalculator
{
    private readonly IApplicationDbContext _context;


    public AttendancePayrollCalculator(
        IApplicationDbContext context)
    {
        _context = context;
    }



    public async Task<List<PayrollItem>> CalculateAsync(
      int employeeId,
      DateOnly startDate,
      DateOnly endDate,
      decimal basicSalary,
      CancellationToken cancellationToken)
    {
        var items = new List<PayrollItem>();


        var policy =
            await _context.PayrollPolicys
                .AsNoTracking()
                .FirstAsync(
                    x => x.IsDefault,
                    cancellationToken);


        var attendanceRecords =
            await _context.AttendanceRecords
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    x.Date >= startDate &&
                    x.Date <= endDate &&
                    x.Status != AttendanceStatus.Holiday &&
                    x.Status != AttendanceStatus.Weekend)
                .ToListAsync(cancellationToken);


        if (!attendanceRecords.Any())
            return items;


        var workingMinutes =
            policy.WorkingDays *
            policy.WorkingMinutesPerDay;


        var minuteValue =
            basicSalary / workingMinutes;


        // ==========================
        // Absence
        // ==========================

        await AddAbsenceDeductionAsync(
            items,
            employeeId,
            startDate,
            endDate,
            basicSalary,
            policy,
            cancellationToken);


        // ==========================
        // Partial Attendance
        // ==========================

        await AddPartialAttendanceDeductionAsync(
            items,
            employeeId,
            startDate,
            endDate,
            basicSalary,
            policy,
            cancellationToken);


        // ==========================
        // Lost Time
        // ==========================

        var expectedMinutes =
            attendanceRecords.Sum(x => x.ExpectedMinutes);


        var workedMinutes =
            attendanceRecords.Sum(x => x.WorkedMinutes);


        var netLostMinutes =
            Math.Max(
                0,
                expectedMinutes - workedMinutes);


        var deductibleMinutes =
            Math.Max(
                0,
                netLostMinutes - policy.MonthlyFreeMinutes);


        if (deductibleMinutes > 0)
        {
            var deduction =
                deductibleMinutes * minuteValue;


            var component =
                await _context.SalaryComponents
                    .FirstOrDefaultAsync(
                        x => x.Code == "LOST_TIME",
                        cancellationToken);


            if (component != null)
            {
                items.Add(new PayrollItem
                {
                    SalaryComponentId =
                        component.Id,

                    Amount =
                        deduction,

                    ItemName =
                        component.NameAr,

                    Description =
                        $"Lost time deduction ({deductibleMinutes} minutes)",

                    Source =
                        PayrollItemSource.Attendance,

                    Type =
                        SalaryComponentType.Deduction
                });
            }
        }


        return items;
    }

    private async Task AddAbsenceDeductionAsync(
     List<PayrollItem> items,
     int employeeId,
     DateOnly startDate,
     DateOnly endDate,
     decimal basicSalary,
     PayrollPolicy policy,
     CancellationToken cancellationToken)
    {
        var absentDays =
            await _context.AttendanceRecords
                .CountAsync(x =>
                    x.EmployeeId == employeeId &&
                    x.Date >= startDate &&
                    x.Date <= endDate &&
                    x.Status == AttendanceStatus.Absent,
                    cancellationToken);

        if (absentDays == 0)
            return;

        var dailySalary =
            basicSalary / policy.WorkingDays;

        var deduction =
            dailySalary *
            absentDays *
            policy.AbsentDeductionFactor;

        var component =
            await _context.SalaryComponents
                .FirstOrDefaultAsync(
                    x => x.Code == "ABSENT",
                    cancellationToken);

        if (component == null)
            return;

        items.Add(new PayrollItem
        {
            SalaryComponentId = component.Id,

            Amount = deduction,

            ItemName = component.NameAr,

            Description =
                $"Absence deduction ({absentDays} day(s))",

            Source = PayrollItemSource.Attendance,

            Type = SalaryComponentType.Deduction
        });
    }

    private async Task AddPartialAttendanceDeductionAsync(
       List<PayrollItem> items,
       int employeeId,
       DateOnly startDate,
       DateOnly endDate,
       decimal basicSalary,
       PayrollPolicy policy,
       CancellationToken cancellationToken)
    {
        var records =
    await _context.AttendanceRecords
        .Where(x =>
            x.EmployeeId == employeeId &&
            x.Date >= startDate &&
            x.Date <= endDate &&
            x.Status == AttendanceStatus.PartialAttendance)
        .ToListAsync(cancellationToken);

        if (!records.Any())
            return;

        var dailySalary =
            basicSalary / policy.WorkingDays;

        decimal deduction = 0;

        foreach (var record in records)
        {
            var percentage =
                record.HasPermission
                    ? policy.PartialAttendanceWithPermissionPercentage
                    : policy.PartialAttendanceWithoutPermissionPercentage;

            deduction +=
                dailySalary * (percentage / 100m);
        }

        var component =
            await _context.SalaryComponents
                .FirstOrDefaultAsync(
                    x => x.Code == "PARTIAL_ATTENDANCE",
                    cancellationToken);

        if (component == null)
            return;

        items.Add(new PayrollItem
        {
            SalaryComponentId = component.Id,
            Amount = deduction,
            ItemName = component.NameAr,
            Description = $"Partial attendance deduction ({records.Count} day(s))",
            Source = PayrollItemSource.Attendance,
            Type = SalaryComponentType.Deduction
        });
    }
}