using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Dashboard;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Report;
using MicroERP.Domin.Entities.EmployeeAttendance;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Services
{
    public class MyAttendanceQueries : IMyAttendanceQueries
    {
        private readonly IApplicationDbContext _context;

        public MyAttendanceQueries(
            IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<AttendanceDto>>> GetMyAttendanceAsync(
        AttendanceFilterDto filter,
        string userId,
        CancellationToken cancellationToken = default)
        {
            var employeeId =
                await GetCurrentEmployeeIdAsync(
                    userId,
                    cancellationToken);


            var result =
                await SelectAttendanceDto(
                    ApplyFilter(
                        BuildAttendanceQuery(employeeId),
                        filter))
                .OrderByDescending(x => x.Date)
                .ToListAsync(cancellationToken);


            return Result<List<AttendanceDto>>
                .Succeeded(result);
        }

        public async Task<Result<AttendanceSummaryDto>>
          GetMyMonthlySummaryAsync(
          int month,
          int year,
          string userId,
          CancellationToken cancellationToken = default)
        {
            var employeeId =
                await GetCurrentEmployeeIdAsync(
                    userId,
                    cancellationToken);


            var records =
                await GetMonthlyRecordsAsync(
                    employeeId,
                    year,
                    month,
                    cancellationToken);


            if (records.Count == 0)
            {
                return Result<AttendanceSummaryDto>
                    .Failure("No attendance records found");
            }


            var summary =
                CreateMonthlySummary(
                    employeeId,
                    await GetEmployeeNameAsync(
                        employeeId,
                        cancellationToken),
                    records);


            return Result<AttendanceSummaryDto>
                .Succeeded(summary);
        }


        public async Task<Result<AttendancePerformanceDto>>
         GetMyPerformanceAsync(
         int month,
         int year,
         string userId,
         CancellationToken cancellationToken = default)
        {
            var employeeId =
                await GetCurrentEmployeeIdAsync(
                    userId,
                    cancellationToken);


            var result =
                await SelectPerformanceDto(
                    BuildPerformanceQuery(employeeId))
                .FirstOrDefaultAsync(
                    x =>
                        x.Year == year &&
                        x.Month == month,
                    cancellationToken);


            if (result == null)
            {
                return Result<AttendancePerformanceDto>
                    .Failure("Performance not found");
            }


            return Result<AttendancePerformanceDto>
                .Succeeded(result);
        }

        public async Task<Result<AttendanceDto>>
         GetMyAttendanceByDateAsync(
         DateOnly date,
         string userId,
         CancellationToken cancellationToken = default)
        {
            var employeeId =
                await GetCurrentEmployeeIdAsync(
                    userId,
                    cancellationToken);


            var result =
                await SelectAttendanceDto(
                    BuildAttendanceQuery(employeeId))
                .FirstOrDefaultAsync(
                    x => x.Date == date,
                    cancellationToken);


            if (result == null)
            {
                return Result<AttendanceDto>
                    .Failure("Attendance record not found.");
            }


            return Result<AttendanceDto>
                .Succeeded(result);
        }

        public async Task<Result<List<AttendancePerformanceDto>>>
         GetMyPerformanceHistoryAsync(
         string userId,
         CancellationToken cancellationToken = default)
        {
            var employeeId =
                await GetCurrentEmployeeIdAsync(
                    userId,
                    cancellationToken);


            var result =
                await SelectPerformanceDto(
                    BuildPerformanceQuery(employeeId))
                .OrderByDescending(x => x.Year)
                .ThenByDescending(x => x.Month)
                .ToListAsync(cancellationToken);


            return Result<List<AttendancePerformanceDto>>
                .Succeeded(result);
        }

        //============= Helpers =====================

        protected async Task<int> GetCurrentEmployeeIdAsync(
         string userId,
         CancellationToken cancellationToken)
        {
            var employeeId =
                await _context.Employees
                .Where(x => x.UserId == userId)
                .Select(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (employeeId == 0)
            {
                throw new KeyNotFoundException(
                    "Current user is not linked to an employee.");
            }

            return employeeId;
        }

        private IQueryable<AttendanceRecord> BuildAttendanceQuery(int employeeId)
   {
        return _context.AttendanceRecords
        .AsNoTracking()
        .Where(x => x.EmployeeId == employeeId);
}

        private static IQueryable<AttendanceRecord> ApplyFilter(
         IQueryable<AttendanceRecord> query,
         AttendanceFilterDto filter)
        {
            if (!filter.DateFrom.HasValue &&
                !filter.DateTo.HasValue)
            {
                var start =
                    new DateOnly(DateTime.Now.Year, 1, 1);

                var end =
                    new DateOnly(DateTime.Now.Year, 12, 31);

                query = query.Where(x =>
                    x.Date >= start &&
                    x.Date <= end);
            }

            if (filter.DateFrom.HasValue)
            {
                query = query.Where(x =>
                    x.Date >= filter.DateFrom.Value);
            }

            if (filter.DateTo.HasValue)
            {
                query = query.Where(x =>
                    x.Date <= filter.DateTo.Value);
            }

            if (filter.Status.HasValue)
            {
                query = query.Where(x =>
                    x.Status == filter.Status.Value);
            }

            return query;
        }

        private static IQueryable<AttendanceDto> SelectAttendanceDto(
         IQueryable<AttendanceRecord> query)
        {
            return query.Select(x => new AttendanceDto
            {
                Id = x.Id,

                EmployeeId = x.EmployeeId,

                EmployeeName =
                    x.Employee.User.FullName,

                Date = x.Date,

                CheckIn = x.Transactions
                    .Where(t =>
                        t.Type == AttendanceTransactionType.CheckIn)
                    .OrderBy(t => t.TransactionTime)
                    .Select(t => t.TransactionTime)
                    .FirstOrDefault(),

                CheckOut = x.Transactions
                    .Where(t =>
                        t.Type == AttendanceTransactionType.CheckOut)
                    .OrderByDescending(t => t.TransactionTime)
                    .Select(t => t.TransactionTime)
                    .FirstOrDefault(),

                WorkedMinutes = x.WorkedMinutes,

                ExpectedMinutes = x.ExpectedMinutes,

                LostTimeMinutes = x.LostTimeMinutes,

                LateMinutes = x.LateMinutes,

                EarlyLeaveMinutes = x.EarlyLeaveMinutes,

                Status = x.Status,

                Notes = x.Notes,

                CreatedOn = x.CreatedOn
            });
        }

        private Task<string> GetEmployeeNameAsync(
         int employeeId,
         CancellationToken cancellationToken)
        {
            return _context.Employees
                .Where(x => x.Id == employeeId)
                .Select(x => x.User.FullName)
                .FirstAsync(cancellationToken);
        }

        private Task<List<AttendanceRecord>> GetMonthlyRecordsAsync(
         int employeeId,
         int year,
         int month,
         CancellationToken cancellationToken)
        {
            var (startDate, endDate) =
                GetMonthRange(year, month);

            return BuildAttendanceQuery(employeeId)
                .Where(x =>
                    x.Date >= startDate &&
                    x.Date <= endDate)
                .ToListAsync(cancellationToken);
        }

        private static (DateOnly Start, DateOnly End)
        GetMonthRange(
        int year,
        int month)
        {
            var start =
                new DateOnly(year, month, 1);

            var end =
                start.AddMonths(1)
                     .AddDays(-1);

            return (start, end);
        }

        private static AttendanceSummaryDto CreateMonthlySummary(
        int employeeId,
        string employeeName,
        List<AttendanceRecord> records)
        {
            return new AttendanceSummaryDto
            {
                EmployeeId = employeeId,

                EmployeeName = employeeName,

                TotalDays =
                    records.Count,

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

                LateMinutes =
                    records.Sum(x =>
                        x.LateMinutes),

                EarlyLeaveMinutes =
                    records.Sum(x =>
                        x.EarlyLeaveMinutes),

                WorkedMinutes =
                    records.Sum(x =>
                        x.WorkedMinutes),

                ExpectedMinutes =
                    records.Sum(x =>
                        x.ExpectedMinutes),

                LostTimeMinutes =
                    records.Sum(x =>
                        x.LostTimeMinutes)
            };
        }

        private IQueryable<AttendancePerformance>
        BuildPerformanceQuery(
        int employeeId)
        {
            return _context.AttendancePerformances
                .AsNoTracking()
                .Where(x =>
                    x.EmployeeId == employeeId);
        }

        private static IQueryable<AttendancePerformanceDto>
        SelectPerformanceDto(
        IQueryable<AttendancePerformance> query)
        {
            return query.Select(x =>
                new AttendancePerformanceDto
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

                    TotalEarlyLeaveMinutes =
                        x.TotalEarlyLeaveMinutes,

                    TotalLostTimeMinutes =
                        x.TotalLostTimeMinutes,

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

                    Notes =
                        x.Notes,

                    CreatedOn =
                        x.CreatedOn
                });
        }
    }
}
