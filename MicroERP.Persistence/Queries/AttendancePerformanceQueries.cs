using MicroERP.Application.Common.Extensions;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Dashboard;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using MicroERP.Domin.Entities.EmployeeAttendance;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Persistence.Queries
{
    public class AttendancePerformanceQueries: IAttendancePerformanceQueries
    {
        private readonly IApplicationDbContext _context;

        public AttendancePerformanceQueries(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<AttendancePerformanceDto>>
         GetByEmployeeAndMonthAsync(
         int employeeId,
         int year,
         int month,
         CancellationToken cancellationToken = default)
        {
            var result =
                await SelectDto(BuildQuery())
                .FirstOrDefaultAsync(
                    x =>
                        x.EmployeeId == employeeId &&
                        x.Year == year &&
                        x.Month == month,
                    cancellationToken);


            if (result == null)
            {
                return Result<AttendancePerformanceDto>
                    .Failure(
                        "Attendance performance not found.");
            }


            return Result<AttendancePerformanceDto>
                .Succeeded(result);
        }
       
        public async Task<Result<List<AttendancePerformanceDto>>>
         GetEmployeeHistoryAsync(
         int employeeId,
         CancellationToken cancellationToken = default)
        {
            var result =
                await _context.AttendancePerformances
                .AsNoTracking()
                .Where(x =>
                    x.EmployeeId == employeeId)

                .OrderByDescending(x =>
                    x.Year)

                .ThenByDescending(x =>
                    x.Month)

                .Select(x => new AttendancePerformanceDto
                {
                    Id = x.Id,

                    EmployeeId =
                        x.EmployeeId,

                    EmployeeName =
                        x.Employee.User.FullName,


                    Year =
                        x.Year,

                    Month =
                        x.Month,


                    AttendanceScore =
                        x.AttendanceScore,


                    PresentDays =
                        x.PresentDays,

                    AbsentDays =
                        x.AbsentDays,

                    PartialAttendanceDays =
                        x.PartialAttendanceDays,


                    MissingCheckInCount =
                        x.MissingCheckInCount,

                    MissingCheckOutCount =
                        x.MissingCheckOutCount,


                    TotalLateMinutes =
                        x.TotalLateMinutes,

                    TotalLostTimeMinutes =
                        x.TotalLostTimeMinutes,

                    TotalEarlyLeaveMinutes =
                        x.TotalEarlyLeaveMinutes,


                    EarlyLeaveDays =
                        x.EarlyLeaveDays,


                    AnnualLeaveDays =
                        x.AnnualLeaveDays,

                    SickLeaveDays =
                        x.SickLeaveDays,

                    UnpaidLeaveDays =
                        x.UnpaidLeaveDays,


                    TotalPenaltyPoints =
                        x.TotalPenaltyPoints,


                    CreatedOn =
                        x.CreatedOn
                })

                .ToListAsync(cancellationToken);



            return Result<List<AttendancePerformanceDto>>
                .Succeeded(result);
        }

        public async Task<Result<PagedResult<AttendancePerformanceDto>>>
       GetPagedAsync(
       AttendancePerformanceFilterDto filter,
       PagedRequest request,
       CancellationToken cancellationToken = default)
        {
            var query =
                BuildQuery();


            query =
                ApplyFilter(
                    query,
                    filter);

        
            var result =
                query
                .OrderByDescending(x =>
                    x.Year)

                .ThenByDescending(x =>
                    x.Month)

                .Select(x => new AttendancePerformanceDto
                {
                    Id =
                        x.Id,

                    EmployeeId =
                        x.EmployeeId,

                    EmployeeName =
                        x.Employee.User.FullName,


                    Year =
                        x.Year,

                    Month =
                        x.Month,


                    AttendanceScore =
                        x.AttendanceScore,


                    PresentDays =
                        x.PresentDays,

                    AbsentDays =
                        x.AbsentDays,

                    PartialAttendanceDays =
                        x.PartialAttendanceDays,


                    MissingCheckInCount =
                        x.MissingCheckInCount,

                    MissingCheckOutCount =
                        x.MissingCheckOutCount,


                    TotalLateMinutes =
                        x.TotalLateMinutes,

                    TotalLostTimeMinutes =
                        x.TotalLostTimeMinutes,

                    TotalEarlyLeaveMinutes =
                        x.TotalEarlyLeaveMinutes,


                    EarlyLeaveDays =
                        x.EarlyLeaveDays,


                    AnnualLeaveDays =
                        x.AnnualLeaveDays,

                    SickLeaveDays =
                        x.SickLeaveDays,

                    UnpaidLeaveDays =
                        x.UnpaidLeaveDays,


                    TotalPenaltyPoints =
                        x.TotalPenaltyPoints,


                    CreatedOn =
                        x.CreatedOn
                });



            var paged =
                await result.ToPagedResultAsync(
                    request,
                    cancellationToken);



            return Result<PagedResult<AttendancePerformanceDto>>
                .Succeeded(paged);
        }


        public async Task<Result<AttendancePerformanceDashboardDto>>
        GetDashboardAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default)
        {
            var query =
                _context.AttendancePerformances
                .AsNoTracking()
                .Where(x =>
                    x.Year == year &&
                    x.Month == month);



            var data =
                await query
                .GroupBy(x => 1)
                .Select(x => new AttendancePerformanceDashboardDto
                {
                    EvaluatedEmployees =
                        x.Count(),


                    AverageScore =
                        Math.Round(
                            x.Average(p =>
                                (decimal)p.AttendanceScore),
                            2),


                    HighestScore =
                        x.Max(p =>
                            p.AttendanceScore),


                    LowestScore =
                        x.Min(p =>
                            p.AttendanceScore),


                    TotalPenaltyPoints =
                        x.Sum(p =>
                            p.TotalPenaltyPoints),


                    TotalAbsentDays =
                        x.Sum(p =>
                            p.AbsentDays),


                    TotalLateMinutes =
                        x.Sum(p =>
                            p.TotalLateMinutes),


                    TotalLostTimeMinutes =
                        x.Sum(p =>
                            p.TotalLostTimeMinutes),


                    ExcellentCount =
                        x.Count(p =>
                            p.AttendanceScore >= 90),


                    GoodCount =
                        x.Count(p =>
                            p.AttendanceScore >= 75 &&
                            p.AttendanceScore < 90),


                    AverageCount =
                        x.Count(p =>
                            p.AttendanceScore >= 60 &&
                            p.AttendanceScore < 75),


                    WeakCount =
                        x.Count(p =>
                            p.AttendanceScore < 60)
                })
                .FirstOrDefaultAsync(cancellationToken);



            data ??= new AttendancePerformanceDashboardDto();



            data.TotalEmployees =
                await _context.Employees
                .CountAsync(
                    x => x.IsActive,
                    cancellationToken);



            return Result<AttendancePerformanceDashboardDto>
                .Succeeded(data);
        }

        public async Task<Result<AttendancePerformanceStatisticsDto>>
          GetMonthlyStatisticsAsync(
          int year,
          int month,
          CancellationToken cancellationToken = default)
        {
            var statistics =
                await BuildQuery()
                .Where(x =>
                    x.Year == year &&
                    x.Month == month)
                .GroupBy(x => 1)
                .Select(x => new AttendancePerformanceStatisticsDto
                {
                    TotalEmployees =
                        x.Count(),

                    AverageScore =
                        Math.Round(
                            x.Average(y =>
                                (decimal)y.AttendanceScore),
                            2),

                    HighestScore =
                        x.Max(y =>
                            y.AttendanceScore),

                    LowestScore =
                        x.Min(y =>
                            y.AttendanceScore),

                    ExcellentEmployees =
                        x.Count(y =>
                            y.AttendanceScore >= 90),

                    GoodEmployees =
                        x.Count(y =>
                            y.AttendanceScore >= 80 &&
                            y.AttendanceScore < 90),

                    AverageEmployees =
                        x.Count(y =>
                            y.AttendanceScore >= 70 &&
                            y.AttendanceScore < 80),

                    WeakEmployees =
                        x.Count(y =>
                            y.AttendanceScore < 70)
                })
                .FirstOrDefaultAsync(cancellationToken);



            return Result<AttendancePerformanceStatisticsDto>
                .Succeeded(
                    statistics ??
                    new AttendancePerformanceStatisticsDto());
        }

        public async Task<Result<List<TopPerformanceEmployeeDto>>>
      GetTopEmployeesAsync(
      int year,
      int month,
      int top = 10,
      CancellationToken cancellationToken = default)
        {
            var result =
                await ApplyTopPerformanceOrder(
                    SelectPerformanceEmployeeDto(
                        BuildMonthlyQuery(year, month)))
                .Take(top)
                .ToListAsync(cancellationToken);


            return Result<List<TopPerformanceEmployeeDto>>
                .Succeeded(result);
        }




        public async Task<Result<List<TopPerformanceEmployeeDto>>>
            GetLowestEmployeesAsync(
            int year,
            int month,
            int top = 10,
            CancellationToken cancellationToken = default)
        {
            var result =
                await ApplyLowestPerformanceOrder(
                 SelectPerformanceEmployeeDto(
                   BuildMonthlyQuery(year, month)))
                .Take(top)
                .ToListAsync(cancellationToken);


            return Result<List<TopPerformanceEmployeeDto>>
                .Succeeded(result);
        }

        public async Task<Result<List<AttendancePerformanceSummaryDto>>>
        GetEmployeePerformanceSummaryAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default)
        {
            var result =
                await BuildMonthlyQuery(year, month)
                .Select(x => new AttendancePerformanceSummaryDto
                {
                    EmployeeId =
                        x.EmployeeId,

                    EmployeeName =
                        x.Employee.User.FullName,


                    AttendanceScore =
                        x.AttendanceScore,


                    PresentDays =
                        x.PresentDays,


                    AbsentDays =
                        x.AbsentDays,


                    PartialAttendanceDays =
                        x.PartialAttendanceDays,


                    AnnualLeaveDays =
                        x.AnnualLeaveDays,


                    SickLeaveDays =
                        x.SickLeaveDays,


                    UnpaidLeaveDays =
                        x.UnpaidLeaveDays,


                    TotalLateMinutes =
                        x.TotalLateMinutes,


                    TotalLostTimeMinutes =
                        x.TotalLostTimeMinutes,


                    TotalEarlyLeaveMinutes =
                        x.TotalEarlyLeaveMinutes,


                    EarlyLeaveDays =
                        x.EarlyLeaveDays,


                    TotalPenaltyPoints =
                        x.TotalPenaltyPoints
                })
                .OrderByDescending(x =>
                    x.AttendanceScore)

                .ToListAsync(cancellationToken);


            return Result<List<AttendancePerformanceSummaryDto>>
                .Succeeded(result);
        }

        public async Task<Result<List<DepartmentPerformanceSummaryDto>>>
        GetDepartmentPerformanceSummaryAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default)
        {
            var result =
                await BuildMonthlyQuery(year, month)

                .GroupBy(x => new
                {
                    x.Employee.DepartmentId,

                    DepartmentName =
                        x.Employee.Department.NameAr
                })

                .Select(x => new DepartmentPerformanceSummaryDto
                {
                    DepartmentId =
                        x.Key.DepartmentId,

                    DepartmentName =
                        x.Key.DepartmentName,


                    EmployeesCount =
                        x.Count(),


                    AverageScore =
                        Math.Round(
                            x.Average(p =>
                                (decimal)p.AttendanceScore),
                            2),


                    TotalPresentDays =
                        x.Sum(p =>
                            p.PresentDays),


                    TotalAbsentDays =
                        x.Sum(p =>
                            p.AbsentDays),


                    TotalPartialAttendanceDays =
                        x.Sum(p =>
                            p.PartialAttendanceDays),



                    TotalAnnualLeaveDays =
                        x.Sum(p =>
                            p.AnnualLeaveDays),


                    TotalSickLeaveDays =
                        x.Sum(p =>
                            p.SickLeaveDays),


                    TotalUnpaidLeaveDays =
                        x.Sum(p =>
                            p.UnpaidLeaveDays),



                    TotalLateMinutes =
                        x.Sum(p =>
                            p.TotalLateMinutes),


                    TotalLostTimeMinutes =
                        x.Sum(p =>
                            p.TotalLostTimeMinutes),


                    TotalEarlyLeaveMinutes =
                        x.Sum(p =>
                            p.TotalEarlyLeaveMinutes),


                    TotalEarlyLeaveDays =
                        x.Sum(p =>
                            p.EarlyLeaveDays),


                    TotalPenaltyPoints =
                        x.Sum(p =>
                            p.TotalPenaltyPoints)
                })

                .OrderByDescending(x =>
                    x.AverageScore)

                .ToListAsync(cancellationToken);



            return Result<List<DepartmentPerformanceSummaryDto>>
                .Succeeded(result);
        }

        private IQueryable<AttendancePerformance> BuildQuery()
        {
            return _context.AttendancePerformances
                .AsNoTracking();
        }

        private IQueryable<AttendancePerformance> ApplyFilter(
        IQueryable<AttendancePerformance> query,
        AttendancePerformanceFilterDto filter)
        {
            if (filter.EmployeeId.HasValue)
            {
                query =
                    query.Where(x =>
                        x.EmployeeId == filter.EmployeeId.Value);
            }


            if (filter.Year.HasValue)
            {
                query =
                    query.Where(x =>
                        x.Year == filter.Year.Value);
            }


            if (filter.Month.HasValue)
            {
                query =
                    query.Where(x =>
                        x.Month == filter.Month.Value);
            }


            if (filter.MinimumScore.HasValue)
            {
                query =
                    query.Where(x =>
                        x.AttendanceScore >= filter.MinimumScore.Value);
            }


            if (filter.MaximumScore.HasValue)
            {
                query =
                    query.Where(x =>
                        x.AttendanceScore <= filter.MaximumScore.Value);
            }


            return query;
        }

        private static IQueryable<AttendancePerformanceDto> SelectDto(
            IQueryable<AttendancePerformance> query)
        {
            return query.Select(x => new AttendancePerformanceDto
            {
                Id = x.Id,

                EmployeeId = x.EmployeeId,

                EmployeeName = x.Employee.User.FullName,

                Year = x.Year,

                Month = x.Month,

                AttendanceScore = x.AttendanceScore,

                PresentDays = x.PresentDays,

                AbsentDays = x.AbsentDays,

                PartialAttendanceDays = x.PartialAttendanceDays,

                EarlyLeaveDays = x.EarlyLeaveDays,

                AnnualLeaveDays = x.AnnualLeaveDays,

                SickLeaveDays = x.SickLeaveDays,

                UnpaidLeaveDays = x.UnpaidLeaveDays,

                MissingCheckInCount = x.MissingCheckInCount,

                MissingCheckOutCount = x.MissingCheckOutCount,

                TotalLateMinutes = x.TotalLateMinutes,

                TotalLostTimeMinutes = x.TotalLostTimeMinutes,

                TotalEarlyLeaveMinutes = x.TotalEarlyLeaveMinutes,

                TotalPenaltyPoints = x.TotalPenaltyPoints,

                CreatedOn = x.CreatedOn
            });
        }

        private IQueryable<AttendancePerformance> BuildMonthlyQuery(
         int year,
         int month)
        {
            return BuildQuery()
                .Where(x =>
                    x.Year == year &&
                    x.Month == month);
        }

        private static IQueryable<TopPerformanceEmployeeDto>
       SelectPerformanceEmployeeDto(
       IQueryable<AttendancePerformance> query)
        {
            return query
                .Select(x => new TopPerformanceEmployeeDto
                {
                    EmployeeId =
                        x.EmployeeId,

                    EmployeeName =
                        x.Employee.User.FullName,

                    DepartmentName =
                        x.Employee.Department.NameAr,


                    AttendanceScore =
                        x.AttendanceScore,


                    PresentDays =
                        x.PresentDays,

                    AbsentDays =
                        x.AbsentDays,

                    PartialAttendanceDays =
                        x.PartialAttendanceDays,


                    AnnualLeaveDays =
                        x.AnnualLeaveDays,

                    SickLeaveDays =
                        x.SickLeaveDays,

                    UnpaidLeaveDays =
                        x.UnpaidLeaveDays,


                    EarlyLeaveDays =
                        x.EarlyLeaveDays,


                    TotalLateMinutes =
                        x.TotalLateMinutes,

                    TotalLostMinutes =
                        x.TotalLostTimeMinutes,

                    TotalEarlyLeaveMinutes =
                        x.TotalEarlyLeaveMinutes,


                    TotalPenaltyPoints =
                        x.TotalPenaltyPoints
                });
        }

        private IQueryable<TopPerformanceEmployeeDto>
        ApplyTopPerformanceOrder(
        IQueryable<TopPerformanceEmployeeDto> query)
        {
            return query
                .OrderByDescending(x =>
                    x.AttendanceScore)

                .ThenByDescending(x =>
                    x.PresentDays)

                .ThenBy(x =>
                    x.TotalPenaltyPoints);
        }

        private IQueryable<TopPerformanceEmployeeDto>
        ApplyLowestPerformanceOrder(
        IQueryable<TopPerformanceEmployeeDto> query)
        {
            return query
                .OrderBy(x =>
                    x.AttendanceScore)

                .ThenByDescending(x =>
                    x.TotalPenaltyPoints)

                .ThenBy(x =>
                    x.PresentDays);
        }
    }
}
