using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using MicroERP.Application.Features.EmployeeAttendance.AttendanceLogs.Calculators;
using MicroERP.Application.Features.EmployeeAttendance.AttendanceLogs.Interfaces;
using Microsoft.EntityFrameworkCore;
using static HRPermissions;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Services;

public class AttendanceRecalculateService
    : IAttendanceRecalculateService
{
    private readonly IApplicationDbContext _context;
    private readonly IAttendanceCalculator _calculator;
    private readonly IAttendanceOvertimeService _overtimeService;

    public AttendanceRecalculateService(
        IApplicationDbContext context,
        IAttendanceCalculator calculator,
        IAttendanceOvertimeService overtimeService)
    {
        _context = context;
        _calculator = calculator;
        _overtimeService = overtimeService;
    }



    public async Task<Result<bool>> RecalculateDayAsync(
      DateOnly date,
      CancellationToken cancellationToken = default)
    {
        var records =
            await _context.AttendanceRecords
                .Where(x => x.Date == date)
                .ToListAsync(cancellationToken);



        if (!records.Any())
        {
            return Result<bool>
                .Failure("No attendance records found");
        }


        foreach (var record in records)
        {
            await _overtimeService.CreateHolidayWeekendOvertimeAsync(
           record,
           cancellationToken);
            await _calculator.CalculateAsync(
                record,
                cancellationToken);
        }



     



        await _context.SaveChangesAsync(cancellationToken);



        return Result<bool>.Succeeded(true);
    }





    public async Task<Result<bool>> RecalculateMonthAsync(
       int year,
       int month,
       CancellationToken cancellationToken = default)
    {
        var startDate =
            new DateOnly(
                year,
                month,
                1);



        var endDate =
            startDate
            .AddMonths(1)
            .AddDays(-1);



        var records =
            await _context.AttendanceRecords
            .Where(x =>
                x.Date >= startDate &&
                x.Date <= endDate)
            .ToListAsync(cancellationToken);



        if (!records.Any())
        {
            return Result<bool>
                .Failure(
                "No attendance records found");
        }



        foreach (var record in records)
        {
            await _calculator.CalculateAsync(
                record,
                cancellationToken);

            await _overtimeService.CreateHolidayWeekendOvertimeAsync(
             record,
             cancellationToken);
        }



        await _context.SaveChangesAsync(
            cancellationToken);



        return Result<bool>
            .Succeeded(true);
    }





    public async Task<Result<bool>> RecalculateEmployeeAsync(
        int employeeId,
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        var startDate =
            new DateOnly(
                year,
                month,
                1);



        var endDate =
            startDate
            .AddMonths(1)
            .AddDays(-1);



        var records =
            await _context.AttendanceRecords
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    x.Date >= startDate &&
                    x.Date <= endDate)
                .ToListAsync(cancellationToken);



        if (!records.Any())
        {
            return Result<bool>
                .Failure("No attendance records found");
        }



        foreach (var record in records)
        {

            await _calculator.CalculateAsync(
                record,
                cancellationToken);

            await _overtimeService.CreateHolidayWeekendOvertimeAsync(
          record,
          cancellationToken);
        }



        await _context.SaveChangesAsync(
            cancellationToken);



        return Result<bool>
            .Succeeded(true);
    }





    public async Task<Result<bool>> RecalculateAllAsync(
     int year,
     int month,
     CancellationToken cancellationToken = default)
    {
        var startDate =
            new DateOnly(
                year,
                month,
                1);



        var endDate =
            startDate
            .AddMonths(1)
            .AddDays(-1);



        var records =
            await _context.AttendanceRecords
                .Where(x =>
                    x.Date >= startDate &&
                    x.Date <= endDate)
                .ToListAsync(cancellationToken);



        if (!records.Any())
        {
            return Result<bool>
                .Failure("No attendance records found");
        }



        foreach (var record in records)
        {
          

            await _overtimeService.CreateHolidayWeekendOvertimeAsync(
             record,
             cancellationToken);

            await _calculator.CalculateAsync(
                record,
                cancellationToken);
        }



        await _context.SaveChangesAsync(
            cancellationToken);



        return Result<bool>
            .Succeeded(true);
    }
}