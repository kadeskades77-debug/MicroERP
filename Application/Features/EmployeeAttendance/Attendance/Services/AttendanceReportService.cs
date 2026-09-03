using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Report;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using MicroERP.Domin.Entities.EmployeeAttendance;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Services;

public class AttendanceReportService
    : IAttendanceReportQueries
{
    private readonly IApplicationDbContext _context;


    public AttendanceReportService(
        IApplicationDbContext context)
    {
        _context = context;
    }



    public async Task<Result<List<AttendanceReportDto>>>
      GetAttendanceReportAsync(
      AttendanceReportFilterDto filter,
      CancellationToken cancellationToken = default)
    {
        var query =
            BuildAttendanceQuery();


        query =
            ApplyEmployeeFilter(
                query,
                filter.EmployeeId);


        query =
            ApplyDateFilter(
                query,
                filter.DateFrom,
                filter.DateTo);



        if (filter.Status.HasValue)
        {
            query =
                query.Where(x =>
                    x.Status ==
                    filter.Status.Value);
        }



        var result =
            await query
            .OrderByDescending(x => x.Date)
            .Select(x => new AttendanceReportDto
            {
                EmployeeId =
                    x.EmployeeId,

                EmployeeName =
                    x.Employee.User.FullName,

                DepartmentName =
                    x.Employee.Department.NameAr,

                WorkScheduleName =
                    x.Employee.WorkSchedule!.Name,

                Date =
                    x.Date,

                Status =
                    x.Status,

                ExpectedMinutes =
                    x.ExpectedMinutes,

                WorkedMinutes =
                    x.WorkedMinutes,

                LateMinutes =
                    x.LateMinutes,

                EarlyLeaveMinutes =
                    x.EarlyLeaveMinutes,

                LostTimeMinutes =
                    x.LostTimeMinutes,

                IsMissingCheckIn =
                    x.IsMissingCheckIn,

                IsMissingCheckOut =
                    x.IsMissingCheckOut,

                TransactionsCount =
                    x.Transactions.Count,

                Notes =
                    x.Notes
            })
            .ToListAsync(cancellationToken);



        return Result<List<AttendanceReportDto>>
            .Succeeded(result);
    }

    public async Task<Result<List<LateAttendanceReportDto>>>
      GetLateReportAsync(
     LateAttendanceFilterDto filter,
     CancellationToken cancellationToken = default)
    {
        var query =
            BuildAttendanceQuery()
            .Where(x => x.LateMinutes > 0);



        query =
            ApplyEmployeeFilter(
                query,
                filter.EmployeeId);



        query =
            ApplyDateFilter(
                query,
                filter.DateFrom,
                filter.DateTo);



        if (filter.MinimumLateMinutes.HasValue)
        {
            query =
                query.Where(x =>
                    x.LateMinutes >=
                    filter.MinimumLateMinutes.Value);
        }



        var result =
            await query

            .GroupBy(x => new
            {
                x.EmployeeId,

                EmployeeName =
                    x.Employee.User.FullName
            })


            .Select(x => new LateAttendanceReportDto
            {
                EmployeeId =
                    x.Key.EmployeeId,


                EmployeeName =
                    x.Key.EmployeeName,


                LateDays =
                    x.Count(),


                TotalLateMinutes =
                    x.Sum(a =>
                        a.LateMinutes),


                AverageLateMinutes =
                    (int)x.Average(a =>
                        a.LateMinutes)
            })


            .OrderByDescending(x =>
                x.TotalLateMinutes)


            .ToListAsync(
                cancellationToken);



        return Result<List<LateAttendanceReportDto>>
            .Succeeded(result);
    }

    public async Task<Result<List<AbsentAttendanceReportDto>>>
      GetAbsentReportAsync(
     AbsentAttendanceFilterDto filter,
     CancellationToken cancellationToken = default)
    {
        var query =
            BuildAttendanceQuery()
            .Where(x =>
                x.Status ==
                AttendanceStatus.Absent);



        query =
            ApplyEmployeeFilter(
                query,
                filter.EmployeeId);



        query =
            ApplyDateFilter(
                query,
                filter.DateFrom,
                filter.DateTo);



        var penalty =
            await GetAbsentPenaltyAsync(
                cancellationToken);



        var result =
            await query

            .GroupBy(x => new
            {
                x.EmployeeId,

                EmployeeName =
                    x.Employee.User.FullName
            })


            .Select(x => new AbsentAttendanceReportDto
            {
                EmployeeId =
                    x.Key.EmployeeId,


                EmployeeName =
                    x.Key.EmployeeName,


                AbsentDays =
                    x.Count(),


                TotalDays =
                    x.Count(),


                PenaltyPoints =
                    x.Count() *
                    penalty
            })


            .ToListAsync(
                cancellationToken);



        foreach (var item in result)
        {
            item.AbsencePercentage =
                item.TotalDays == 0
                ? 0
                :
                Math.Round(
                    (decimal)item.AbsentDays /
                    item.TotalDays *
                    100,
                    2);
        }



        return Result<List<AbsentAttendanceReportDto>>
            .Succeeded(result);
    }

    public async Task<Result<List<TopLateEmployeeDto>>>
   GetTopLateEmployeesAsync(
       DateOnly? from = null,
       DateOnly? to = null,
       int top = 10,
       CancellationToken cancellationToken = default)
    {
        EnsurePeriod(
            ref from,
            ref to);



        var query =
            BuildAttendanceQuery()
            .Where(x =>
                x.Date >= from &&
                x.Date <= to &&
                x.LateMinutes > 0);



        var result =
            await query

            .GroupBy(x => new
            {
                x.EmployeeId,

                EmployeeName =
                    x.Employee.User.FullName,

                DepartmentName =
                    x.Employee.Department.NameAr
            })


            .Select(x => new TopLateEmployeeDto
            {
                EmployeeId =
                    x.Key.EmployeeId,


                EmployeeName =
                    x.Key.EmployeeName,


                DepartmentName =
                    x.Key.DepartmentName,


                LateDays =
                    x.Count(),


                TotalLateMinutes =
                    x.Sum(a =>
                        a.LateMinutes),


                TotalLostMinutes =
                    x.Sum(a =>
                        a.LostTimeMinutes),


                AverageLateMinutes =
                    (int)x.Average(a =>
                        a.LateMinutes),


                AverageLostMinutes =
                    (int)x.Average(a =>
                        a.LostTimeMinutes)
            })


            .OrderByDescending(x =>
                x.TotalLateMinutes)


            .ThenByDescending(x =>
                x.TotalLostMinutes)


            .ThenByDescending(x =>
                x.LateDays)


            .Take(top)


            .ToListAsync(
                cancellationToken);



        return Result<List<TopLateEmployeeDto>>
            .Succeeded(result);
    }

    public async Task<Result<List<TopAbsentEmployeeDto>>>
  GetTopAbsentEmployeesAsync(
      DateOnly? from = null,
      DateOnly? to = null,
      int top = 10,
      CancellationToken cancellationToken = default)
    {
        EnsurePeriod(
            ref from,
            ref to);



        var penalty =
            await GetAbsentPenaltyAsync(
                cancellationToken);



        var query =
            BuildAttendanceQuery()
            .Where(x =>
                x.Date >= from &&
                x.Date <= to &&
                x.Status ==
                AttendanceStatus.Absent);



        var result =
            await query

            .GroupBy(x => new
            {
                x.EmployeeId,

                EmployeeName =
                    x.Employee.User.FullName,

                DepartmentName =
                    x.Employee.Department.NameAr
            })


            .Select(x => new TopAbsentEmployeeDto
            {
                EmployeeId =
                    x.Key.EmployeeId,


                EmployeeName =
                    x.Key.EmployeeName,


                DepartmentName =
                    x.Key.DepartmentName,


                AbsentDays =
                    x.Count(),


                PenaltyPoints =
                    x.Count() *
                    penalty
            })


            .OrderByDescending(x =>
                x.AbsentDays)


            .ThenByDescending(x =>
                x.PenaltyPoints)


            .Take(top)


            .ToListAsync(
                cancellationToken);



        return Result<List<TopAbsentEmployeeDto>>
            .Succeeded(result);
    }

    //==================== Helpers ===================

    private IQueryable<AttendanceRecord> BuildAttendanceQuery()
    {
        return _context.AttendanceRecords
            .AsNoTracking()
            .AsQueryable();
    }



    private IQueryable<AttendanceRecord> ApplyEmployeeFilter(
        IQueryable<AttendanceRecord> query,
        int? employeeId)
    {
        if (employeeId.HasValue)
        {
            query = query.Where(x =>
                x.EmployeeId == employeeId.Value);
        }

        return query;
    }



    private IQueryable<AttendanceRecord> ApplyDateFilter(
        IQueryable<AttendanceRecord> query,
        DateOnly? from,
        DateOnly? to)
    {
        if (from.HasValue)
        {
            query = query.Where(x =>
                x.Date >= from.Value);
        }


        if (to.HasValue)
        {
            query = query.Where(x =>
                x.Date <= to.Value);
        }


        return query;
    }



    private (DateOnly From, DateOnly To) GetDefaultPeriod()
    {
        var today =
            DateOnly.FromDateTime(
                DateTime.Today);


        return
        (
            new DateOnly(
                today.Year,
                today.Month,
                1),

            today
        );
    }



    private async Task<int> GetAbsentPenaltyAsync(
        CancellationToken cancellationToken)
    {
        return await _context.AttendancePolicies
            .AsNoTracking()
            .Where(x => x.IsDefault)
            .Select(x =>
                x.AbsentPenaltyPoints)
            .FirstOrDefaultAsync(
                cancellationToken);
    }

    private void EnsurePeriod(
    ref DateOnly? from,
    ref DateOnly? to)
    {
        if (from.HasValue &&
            to.HasValue)
            return;


        var period =
            GetDefaultPeriod();


        from ??= period.From;

        to ??= period.To;
    }
}