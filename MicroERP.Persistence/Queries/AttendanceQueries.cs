using MicroERP.Application.Common.Extensions;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Dashboard;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Report;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using MicroERP.Domin.Entities.EmployeeAttendance;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MicroERP.Persistence.Queries;

public class AttendanceQueries : IAttendanceQueries
{
    private readonly IApplicationDbContext _context;


    public AttendanceQueries(
        IApplicationDbContext context)
    {
        _context = context;
    }



    public async Task<Result<AttendanceDto>>
     GetByIdAsync(
     int id,
     CancellationToken cancellationToken = default)
    {
        var attendance =
            await BuildAttendanceQuery()

            .Where(x =>
                x.Id == id)

            .Select(BuildAttendanceDto())

            .FirstOrDefaultAsync(
                cancellationToken);



        if (attendance == null)
        {
            return Result<AttendanceDto>
                .Failure(
                    "Attendance record not found.");
        }



        return Result<AttendanceDto>
            .Succeeded(attendance);
    }





    public async Task<Result<PagedResult<AttendanceDto>>> GetAllAsync(
     AttendanceFilterDto filter,
     PagedRequest request,
     CancellationToken cancellationToken = default)
    {
        var query =
            BuildAttendanceQuery();

        query =
            ApplyFilters(
                query,
                filter);

        var totalCount =
            await query.CountAsync(
                cancellationToken);

        var items =
            await query

            .OrderByDescending(x => x.Date)
            .ThenBy(x => x.Employee.User.FullName)

            .Skip((request.PageNumber - 1) * request.PageSize)

            .Take(request.PageSize)

            .Select(BuildAttendanceDto())

            .ToListAsync(
                cancellationToken);

        return Result<PagedResult<AttendanceDto>>
          .Succeeded(
              new PagedResult<AttendanceDto>(
                  items,
                  totalCount,
                  request.PageNumber,
                  request.PageSize));
    }





    public async Task<Result<List<AttendanceDto>>>
    GetDailyAttendanceAsync(
    DateOnly date,
    CancellationToken cancellationToken = default)
    {
        var result =
            await BuildAttendanceQuery()

            .Where(x =>
                x.Date == date)

            .OrderBy(x =>
                x.Employee.Department.NameAr)

            .ThenBy(x =>
                x.Employee.User.FullName)

            .Select(BuildAttendanceDto())

            .ToListAsync(
                cancellationToken);

        return Result<List<AttendanceDto>>
            .Succeeded(result);
    }





    public async Task<Result<List<AttendanceDto>>>
     GetEmployeeAttendanceAsync(
        AttendanceFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        if (!filter.EmployeeId.HasValue)
        {
            return Result<List<AttendanceDto>>
                .Failure(
                    "EmployeeId is required.");
        }

        var query =
            BuildAttendanceQuery();

        query =
            ApplyFilters(
                query,
                filter);

        var result =
            await query

            .OrderByDescending(x => x.Date)

            .Select(BuildAttendanceDto())

            .ToListAsync(
                cancellationToken);

        return Result<List<AttendanceDto>>
            .Succeeded(result);
    }



    public async Task<Result<AttendanceSummaryDto>>
    GetEmployeeMonthlySummaryAsync(
       int employeeId,
       int month,
       int year,
       CancellationToken cancellationToken = default)
    {
        var query =
            BuildEmployeeMonthlyQuery(
                employeeId,
                year,
                month);

        var records =
            await query.ToListAsync(
                cancellationToken);

        if (!records.Any())
        {
            return Result<AttendanceSummaryDto>
                .Failure(
                    "No attendance records found.");
        }

        var employeeName =
            await _context.Employees
                .Where(x => x.Id == employeeId)
                .Select(x => x.User.FullName)
                .FirstAsync(cancellationToken);

        var summary =
            new AttendanceSummaryDto
            {
                EmployeeId = employeeId,

                EmployeeName = employeeName,

                Year = year,

                Month = month,

                WorkingDays =
    records.Count(x =>
        x.ExpectedMinutes > 0),

                PresentDays =
                    records.Count(x =>
                        x.Status == AttendanceStatus.Present),

                AbsentDays =
                    records.Count(x =>
                        x.Status == AttendanceStatus.Absent),

                LeaveDays =
                    records.Count(x =>
                        x.Status == AttendanceStatus.OnLeave),

                WeekendDays =
                    records.Count(x =>
                        x.Status == AttendanceStatus.Weekend),

                WorkedMinutes =
                    records.Sum(x =>
                        x.WorkedMinutes),

                ExpectedMinutes =
                    records.Sum(x =>
                        x.ExpectedMinutes),

                LateMinutes =
                    records.Sum(x =>
                        x.LateMinutes),

                EarlyLeaveMinutes =
                    records.Sum(x =>
                        x.EarlyLeaveMinutes),

                LostTimeMinutes =
                    records.Sum(x =>
                        x.LostTimeMinutes)
            };

        return Result<AttendanceSummaryDto>
            .Succeeded(summary);
    }


    //=========================Dashboard================


    public async Task<Result<AttendanceDashboardDto>> GetDashboardAsync(
        DateOnly? date = null,
        CancellationToken cancellationToken = default)
    {
        date ??= DateOnly.FromDateTime(DateTime.Today);


        var totalEmployees =
            await _context.Employees
            .CountAsync(
                x => x.IsActive,
                cancellationToken);



        var attendances =
            await ApplyDateRange(
                BuildAttendanceQuery(),
                date,
                date)
            .ToListAsync(cancellationToken);



        var present =
            CountStatus(
                attendances,
                AttendanceStatus.Present);


        var absent =
            CountStatus(
                attendances,
                AttendanceStatus.Absent);



        var dto = new AttendanceDashboardDto
        {
            TotalEmployees = totalEmployees,

            ExpectedEmployees =
                attendances.Count(x =>
                    x.ExpectedMinutes > 0),


            PresentEmployees = present,

            AbsentEmployees = absent,


            OnLeaveEmployees =
                CountStatus(
                    attendances,
                    AttendanceStatus.OnLeave),


            HolidayEmployees =
                CountStatus(
                    attendances,
                    AttendanceStatus.Holiday),


            WeekendEmployees =
                CountStatus(
                    attendances,
                    AttendanceStatus.Weekend),


            PartialAttendanceEmployees =
                CountStatus(
                    attendances,
                    AttendanceStatus.PartialAttendance),



            MissingCheckInEmployees =
                CountStatus(
                    attendances,
                    AttendanceStatus.MissingCheckIn),



            MissingCheckOutEmployees =
                CountStatus(
                    attendances,
                    AttendanceStatus.MissingCheckOut),



            LateEmployees =
                attendances.Count(x =>
                    x.LateMinutes > 0),



            EarlyLeaveEmployees =
                attendances.Count(x =>
                    x.EarlyLeaveMinutes > 0),



            LostTimeEmployees =
                attendances.Count(x =>
                    x.LostTimeMinutes > 0),



            TotalWorkedMinutes =
                attendances.Sum(x =>
                    x.WorkedMinutes),



            TotalExpectedMinutes =
                attendances.Sum(x =>
                    x.ExpectedMinutes),



            TotalLateMinutes =
                attendances.Sum(x =>
                    x.LateMinutes),



            TotalLostMinutes =
                attendances.Sum(x =>
                    x.LostTimeMinutes),



            AverageLateMinutes =
                CalculateAverage(
                    attendances.Sum(x =>
                        x.LateMinutes),
                    attendances.Count(x =>
                        x.LateMinutes > 0)),



            AverageLostMinutes =
                CalculateAverage(
                    attendances.Sum(x =>
                        x.LostTimeMinutes),
                    attendances.Count(x =>
                        x.LostTimeMinutes > 0)),



            AttendanceRate =
                CalculateRate(
                    present,
                    totalEmployees),



            AbsenceRate =
                CalculateRate(
                    absent,
                    totalEmployees)
        };


        return Result<AttendanceDashboardDto>
            .Succeeded(dto);
    }

    public async Task<Result<List<AttendanceTrendDto>>> GetAttendanceTrendAsync(
     DateOnly? from = null,
     DateOnly? to = null,
     CancellationToken cancellationToken = default)
    {
        var query =
         ApplyDateRange(
        BuildAttendanceQuery(),
        from,
        to);


        var result =
            await query
            .GroupBy(x => x.Date)
            .Select(x => new AttendanceTrendDto
            {
                Date = x.Key,


                Present =
                    x.Count(a =>
                        a.Status == AttendanceStatus.Present),


                Absent =
                    x.Count(a =>
                        a.Status == AttendanceStatus.Absent),


                OnLeave =
                    x.Count(a =>
                        a.Status == AttendanceStatus.OnLeave),


                Holiday =
                    x.Count(a =>
                        a.Status == AttendanceStatus.Holiday),


                Weekend =
                    x.Count(a =>
                        a.Status == AttendanceStatus.Weekend),


                PartialAttendance =
                    x.Count(a =>
                        a.Status == AttendanceStatus.PartialAttendance),


                MissingCheckIn =
                    x.Count(a =>
                        a.Status == AttendanceStatus.MissingCheckIn),


                MissingCheckOut =
                    x.Count(a =>
                        a.Status == AttendanceStatus.MissingCheckOut)

            })
            .OrderBy(x => x.Date)
            .ToListAsync(cancellationToken);



        return Result<List<AttendanceTrendDto>>
            .Succeeded(result);
    }

    public async Task<Result<List<DepartmentAttendanceDashboardDto>>>
     GetDepartmentDashboardAsync(
     DateOnly? date = null,
     CancellationToken cancellationToken = default)
    {
        date ??= DateOnly.FromDateTime(DateTime.Today);



        var records =
      await ApplyDateRange(
        BuildAttendanceQuery(),
        date,
        date)
    .Include(x => x.Employee)
    .ThenInclude(x => x.Department)
    .ToListAsync(cancellationToken);



        var result =
            records
            .GroupBy(x => new
            {
                x.Employee.DepartmentId,
                DepartmentName =
                    x.Employee.Department.NameAr
            })
            .Select(x => new DepartmentAttendanceDashboardDto
            {
                DepartmentId =
                    x.Key.DepartmentId,

                DepartmentName =
                    x.Key.DepartmentName,


                TotalEmployees =
                    x.Select(a => a.EmployeeId)
                    .Distinct()
                    .Count(),


                ExpectedEmployees =
                    x.Count(a =>
                        a.ExpectedMinutes > 0),


                PresentEmployees =
                    CountStatus(
                        x,
                        AttendanceStatus.Present),


                AbsentEmployees =
                    CountStatus(
                        x,
                        AttendanceStatus.Absent),


                OnLeaveEmployees =
                    CountStatus(
                        x,
                        AttendanceStatus.OnLeave),


                WeekendEmployees =
                    CountStatus(
                        x,
                        AttendanceStatus.Weekend),


                HolidayEmployees =
                    CountStatus(
                        x,
                        AttendanceStatus.Holiday),


                PartialAttendanceEmployees =
                    CountStatus(
                        x,
                        AttendanceStatus.PartialAttendance),


                LateEmployees =
                    x.Count(a =>
                        a.LateMinutes > 0),


                EarlyLeaveEmployees =
                    x.Count(a =>
                        a.EarlyLeaveMinutes > 0),


                LostTimeEmployees =
                    x.Count(a =>
                        a.LostTimeMinutes > 0),


                TotalWorkedMinutes =
                    x.Sum(a =>
                        a.WorkedMinutes),


                TotalLateMinutes =
                    x.Sum(a =>
                        a.LateMinutes),


                TotalLostMinutes =
                    x.Sum(a =>
                        a.LostTimeMinutes)

            })
            .ToList();



        foreach (var item in result)
        {
            item.AttendanceRate =
                CalculateRate(
                    item.PresentEmployees,
                    item.TotalEmployees);


            item.AbsenceRate =
                CalculateRate(
                    item.AbsentEmployees,
                    item.TotalEmployees);


            item.AverageLateMinutes =
                CalculateAverage(
                    item.TotalLateMinutes,
                    item.LateEmployees);


            item.AverageLostMinutes =
                CalculateAverage(
                    item.TotalLostMinutes,
                    item.LostTimeEmployees);
        }



        return Result<List<DepartmentAttendanceDashboardDto>>
            .Succeeded(result);
    }


    public async Task<Result<List<TopPerformanceEmployeeDto>>>
    GetTopPerformanceEmployeesAsync(
    int year,
    int month,
    int top = 10,
    CancellationToken cancellationToken = default)
    {
        var result =
            await GetPerformanceEmployeesAsync(
                year,
                month,
                top,
                true,
                cancellationToken);

        return Result<List<TopPerformanceEmployeeDto>>
            .Succeeded(result);
    }

    public async Task<Result<List<TopPerformanceEmployeeDto>>>
    GetLowPerformanceEmployeesAsync(
        int year,
        int month,
        int top = 10,
        CancellationToken cancellationToken = default)
    {
        var result =
            await GetPerformanceEmployeesAsync(
                year,
                month,
                top,
                false,
                cancellationToken);

        return Result<List<TopPerformanceEmployeeDto>>
            .Succeeded(result);
    }

    //=========== Helpers ================

    private IQueryable<AttendanceRecord> BuildAttendanceQuery()
    {
        return _context.AttendanceRecords
            .AsNoTracking();
    }


    private static Expression<Func<AttendanceRecord, AttendanceDto>>
    BuildAttendanceDto()
    {
        return x => new AttendanceDto
        {
            Id = x.Id,

            EmployeeId = x.EmployeeId,

            EmployeeName = x.Employee.User.FullName,

            DepartmentName =
                x.Employee.Department.NameAr,

            WorkScheduleName =
                x.Employee.WorkSchedule != null
                    ? x.Employee.WorkSchedule.Name
                    : null,

            Date = x.Date,

            CheckIn =
                x.Transactions
                    .Where(t =>
                        t.Type ==
                        AttendanceTransactionType.CheckIn)
                    .OrderBy(t =>
                        t.TransactionTime)
                    .Select(t =>
                        t.TransactionTime)
                    .FirstOrDefault(),

            CheckOut =
                x.Transactions
                    .Where(t =>
                        t.Type ==
                        AttendanceTransactionType.CheckOut)
                    .OrderByDescending(t =>
                        t.TransactionTime)
                    .Select(t =>
                        t.TransactionTime)
                    .FirstOrDefault(),

            WorkedMinutes =
                x.WorkedMinutes,

            ExpectedMinutes =
                x.ExpectedMinutes,

            LateMinutes =
                x.LateMinutes,

            LostTimeMinutes =
                x.LostTimeMinutes,

            EarlyLeaveMinutes =
                x.EarlyLeaveMinutes,

            IsMissingCheckIn =
                x.IsMissingCheckIn,

            IsMissingCheckOut =
                x.IsMissingCheckOut,

            TransactionsCount =
                x.Transactions.Count,

            Status =
                x.Status,

            Notes =
                x.Notes,

            CreatedOn =
                x.CreatedOn
        };
    }

    private (DateOnly Start, DateOnly End)
      GetDefaultPeriod()
    {
        var year = DateTime.Today.Year;

        return
        (
            new DateOnly(year, 1, 1),
            new DateOnly(year, 12, 31)
        );
    }

    private IQueryable<AttendanceRecord> ApplyDefaultYear(
    IQueryable<AttendanceRecord> query,
    DateOnly? from,
    DateOnly? to)
    {
        if (from.HasValue || to.HasValue)
            return query;

        var period =
            GetDefaultPeriod();

        return query.Where(x =>
            x.Date >= period.Start &&
            x.Date <= period.End);
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

    private IQueryable<AttendanceRecord> ApplyEmployeeFilter(
    IQueryable<AttendanceRecord> query,
    int? employeeId)
    {
        if (!employeeId.HasValue)
            return query;

        return query.Where(x =>
            x.EmployeeId == employeeId.Value);
    }

    private IQueryable<AttendanceRecord> ApplyDepartmentFilter(
    IQueryable<AttendanceRecord> query,
    int? departmentId)
    {
        if (!departmentId.HasValue)
            return query;

        return query.Where(x =>
            x.Employee.DepartmentId == departmentId.Value);
    }

    private IQueryable<AttendanceRecord> ApplyStatusFilter(
    IQueryable<AttendanceRecord> query,
    AttendanceStatus? status)
    {
        if (!status.HasValue)
            return query;

        return query.Where(x =>
            x.Status == status.Value);
    }

    private IQueryable<AttendanceRecord> ApplyFilters(
    IQueryable<AttendanceRecord> query,
    AttendanceFilterDto filter)
    {
        query = ApplyDefaultYear(
            query,
            filter.DateFrom,
            filter.DateTo);

        query = ApplyDateFilter(
            query,
            filter.DateFrom,
            filter.DateTo);

        query = ApplyEmployeeFilter(
            query,
            filter.EmployeeId);

        query = ApplyDepartmentFilter(
            query,
            filter.DepartmentId);

        query = ApplyStatusFilter(
            query,
            filter.Status);

        return query;
    }

    private IQueryable<AttendanceRecord> BuildEmployeeMonthlyQuery(
    int employeeId,
    int year,
    int month)
    {
        return BuildAttendanceQuery()
            .Where(x =>
                x.EmployeeId == employeeId &&
                x.Date.Year == year &&
                x.Date.Month == month);
    }

    private static decimal CalculateRate(
    int numerator,
    int denominator)
    {
        if (denominator == 0)
            return 0;

        return Math.Round(
            (decimal)numerator /
            denominator * 100,
            2);
    }

    private IQueryable<AttendanceRecord> ApplyDateRange(
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

    private static int CountStatus(
    IEnumerable<AttendanceRecord> records,
    AttendanceStatus status)
    {
        return records.Count(x => x.Status == status);
    }


    private async Task<List<TopPerformanceEmployeeDto>>
       GetPerformanceEmployeesAsync(
       int year,
       int month,
       int top,
       bool descending,
       CancellationToken cancellationToken)
    {
        var firstDay =
            new DateOnly(year, month, 1);


        var lastDay =
            firstDay
            .AddMonths(1)
            .AddDays(-1);



        // بيانات الحضور الشهرية
        var records =
            await BuildAttendanceQuery()
            .Where(x =>
                x.Date >= firstDay &&
                x.Date <= lastDay)
            .GroupBy(x => new
            {
                x.EmployeeId,

                EmployeeName =
                    x.Employee.User.FullName,

                DepartmentName =
                    x.Employee.Department.NameAr
            })
            .Select(x => new TopPerformanceEmployeeDto
            {
                EmployeeId =
                    x.Key.EmployeeId,


                EmployeeName =
                    x.Key.EmployeeName,


                DepartmentName =
                    x.Key.DepartmentName,



                WorkingDays =
                    x.Count(r =>
                        r.ExpectedMinutes > 0),



                PresentDays =
                    x.Count(r =>
                        r.Status == AttendanceStatus.Present),



                AbsentDays =
                    x.Count(r =>
                        r.Status == AttendanceStatus.Absent),



                LeaveDays =
                    x.Count(r =>
                        r.Status == AttendanceStatus.OnLeave),



                PartialAttendanceDays =
                    x.Count(r =>
                        r.Status ==
                        AttendanceStatus.PartialAttendance),



                WorkedMinutes =
                    x.Sum(r =>
                        r.WorkedMinutes),



                ExpectedMinutes =
                    x.Sum(r =>
                        r.ExpectedMinutes),



                TotalLateMinutes =
                    x.Sum(r =>
                        r.LateMinutes),



                TotalLostMinutes =
                    x.Sum(r =>
                        r.LostTimeMinutes),



                TotalEarlyLeaveMinutes =
                    x.Sum(r =>
                        r.EarlyLeaveMinutes),



                EarlyLeaveDays =
                    x.Count(r =>
                        r.EarlyLeaveMinutes > 0)

            })
            .ToListAsync(cancellationToken);



        if (!records.Any())
            return new List<TopPerformanceEmployeeDto>();



        var employeeIds =
            records
            .Select(x => x.EmployeeId)
            .ToList();



        // الأداء المحفوظ مسبقاً
        var performances =
            await _context.AttendancePerformances
            .AsNoTracking()
            .Where(x =>
                x.Year == year &&
                x.Month == month &&
                employeeIds.Contains(x.EmployeeId))
            .ToDictionaryAsync(
                x => x.EmployeeId,
                cancellationToken);



        // الإجازات دفعة واحدة
        var leaves =
            await _context.EmployeeLeaves
            .AsNoTracking()
            .Where(x =>
                employeeIds.Contains(x.EmployeeId) &&
                x.Status == LeaveStatus.Approved &&
                x.StartDate <= lastDay &&
                x.EndDate >= firstDay)
            .GroupBy(x => x.EmployeeId)
            .Select(x => new
            {
                EmployeeId =
                    x.Key,


                AnnualLeaveDays =
                    x.Sum(l => l.AnnualDays),


                SickLeaveDays =
                    x.Sum(l => l.SickDays),


                UnpaidLeaveDays =
                    x.Sum(l => l.UnpaidDays)

            })
            .ToDictionaryAsync(
                x => x.EmployeeId,
                cancellationToken);



        foreach (var employee in records)
        {
            // ربط نتيجة الأداء
            if (performances.TryGetValue(
                employee.EmployeeId,
                out var performance))
            {
                employee.AttendanceScore =
                    performance.AttendanceScore;


                employee.TotalPenaltyPoints =
                    performance.TotalPenaltyPoints;
            }



            // ربط الإجازات
            if (leaves.TryGetValue(
                employee.EmployeeId,
                out var leave))
            {
                employee.AnnualLeaveDays =
                    leave.AnnualLeaveDays;


                employee.SickLeaveDays =
                    leave.SickLeaveDays;


                employee.UnpaidLeaveDays =
                    leave.UnpaidLeaveDays;
            }



            employee.AttendanceRate =
                CalculateRate(
                    employee.PresentDays,
                    employee.WorkingDays);



            employee.PunctualityRate =
                CalculateRate(
                    employee.ExpectedMinutes -
                    employee.TotalLateMinutes -
                    employee.TotalLostMinutes -
                    employee.TotalEarlyLeaveMinutes,

                    employee.ExpectedMinutes);
        }



        return descending

            ? records
                .OrderByDescending(x =>
                    x.AttendanceScore)

                .ThenByDescending(x =>
                    x.AttendanceRate)

                .ThenBy(x =>
                    x.TotalPenaltyPoints)

                .Take(top)
                .ToList()


            : records
                .OrderBy(x =>
                    x.AttendanceScore)

                .ThenBy(x =>
                    x.AttendanceRate)

                .ThenByDescending(x =>
                    x.TotalPenaltyPoints)

                .Take(top)
                .ToList();
    }



    private static decimal CalculateAverage(
        int total,
        int count)
    {
        if (count == 0)
            return 0;

        return Math.Round(
            (decimal)total / count,
            2);
    }
}