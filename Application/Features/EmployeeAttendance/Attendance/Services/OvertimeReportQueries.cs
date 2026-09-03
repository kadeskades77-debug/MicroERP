using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Overtime;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using MicroERP.Domin.Entities.EmployeeAttendance;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Services
{
    public class OvertimeReportQueries: IOvertimeReportQueries
    {
        private readonly IApplicationDbContext _context;

        public OvertimeReportQueries(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<OvertimeReportResultDto>>
         GetOvertimeReportAsync(
             OvertimeReportFilterDto filter,
             CancellationToken cancellationToken = default)
        {
            // =========================================================
            // Base Query
            // =========================================================

            IQueryable<EmployeeOvertime> query =
                _context.EmployeeOvertimes
                    .AsNoTracking();


            // =========================================================
            // Date Filter
            // =========================================================

            if (filter.FromDate.HasValue)
            {
                var fromDate =
                    filter.FromDate.Value.Date;

                query =
                    query.Where(x =>
                        x.StartDateTime >= fromDate);
            }


            if (filter.ToDate.HasValue)
            {
                // استخدام نهاية الفترة بشكل حصري
                // حتى يشمل كامل يوم ToDate
                var toDateExclusive =
                    filter.ToDate.Value.Date.AddDays(1);

                query =
                    query.Where(x =>
                        x.StartDateTime < toDateExclusive);
            }

            if (filter.EmployeeId.HasValue)
            {
                var employeeId =
                filter.EmployeeId.Value;
                query =
                   query.Where(x =>
                       x.EmployeeId == employeeId);
            }



            // =========================================================
            // Department Filter
            // =========================================================

            if (filter.DepartmentId.HasValue)
            {
                query =
                    query.Where(x =>
                        x.Employee.DepartmentId ==
                        filter.DepartmentId.Value);
            }


            // =========================================================
            // Type Filter
            // =========================================================

            if (filter.Type.HasValue)
            {
                query =
                    query.Where(x =>
                        x.Type ==
                        filter.Type.Value);
            }


            // =========================================================
            // Status Filter
            // =========================================================

            if (filter.Status.HasValue)
            {
                query =
                    query.Where(x =>
                        x.Status ==
                        filter.Status.Value);
            }


            // =========================================================
            // Source Filter
            // =========================================================

            if (filter.Source.HasValue)
            {
                query =
                    query.Where(x =>
                        x.Source ==
                        filter.Source.Value);
            }


            // =========================================================
            // Paid Filter
            // =========================================================

            if (filter.IsPaid.HasValue)
            {
                query =
                    query.Where(x =>
                        x.IsPaid ==
                        filter.IsPaid.Value);
            }


            // =========================================================
            // Details
            // =========================================================

            var overtimes =
                await query
                   .OrderBy(x => x.StartDateTime)
                     .ThenBy(x => x.Employee.User.FullName)
                    .Select(x =>
                        new OvertimeReportDto
                        {
                            Id =
                                x.Id,

                            EmployeeId =
                                x.EmployeeId,

                            EmployeeName =
                                x.Employee.User.FullName,

                            Department =
                               x.Employee.Department == null
                               ? null
                               : x.Employee.Department.NameEn,

                            StartDateTime =
                                x.StartDateTime,

                            EndDateTime =
                                x.EndDateTime,

                            TotalMinutes =
                                x.TotalMinutes,

                            TotalHours =
                                Math.Round(
                                    x.TotalMinutes / 60m,
                                    2),

                            HourlyRate =
                                x.HourlyRate,

                            Multiplier =
                                x.Multiplier,

                            Amount =
                                x.Amount,

                            Type =
                                x.Type,

                            Status =
                                x.Status,

                            Source =
                                x.Source,

                            Reason =
                                x.Reason,

                            IsPaid =
                                x.IsPaid
                        })
                    .ToListAsync(
                        cancellationToken);


            // =========================================================
            // Statistics
            // =========================================================

            var recordsCount =
                overtimes.Count;


            var employeesCount =
                overtimes
                    .Select(x =>
                        x.EmployeeId)
                    .Distinct()
                    .Count();


            var totalMinutes =
                overtimes.Sum(x =>
                    x.TotalMinutes);


            var totalHours =
                Math.Round(
                    totalMinutes / 60m,
                    2,
                    MidpointRounding.AwayFromZero);


            var totalAmount =
                Math.Round(
                    overtimes.Sum(x =>
                        x.Amount),
                    2,
                    MidpointRounding.AwayFromZero);


            var paidAmount =
                Math.Round(
                    overtimes
                        .Where(x =>
                            x.IsPaid)
                        .Sum(x =>
                            x.Amount),
                    2,
                    MidpointRounding.AwayFromZero);


            var unpaidAmount =
                Math.Round(
                    overtimes
                        .Where(x =>
                            !x.IsPaid)
                        .Sum(x =>
                            x.Amount),
                    2,
                    MidpointRounding.AwayFromZero);


            // =========================================================
            // Result
            // =========================================================

            var result =
                new OvertimeReportResultDto
                {
                    RecordsCount =
                        recordsCount,

                    EmployeesCount =
                        employeesCount,

                    TotalMinutes =
                        totalMinutes,

                    TotalHours =
                        totalHours,

                    TotalAmount =
                        totalAmount,

                    PaidAmount =
                        paidAmount,

                    UnpaidAmount =
                        unpaidAmount,

                    Overtimes =
                        overtimes
                };


            return Result<OvertimeReportResultDto>
                .Succeeded(result);
        }

         public async Task<Result<EmployeeOvertimeReportDto>>
         GetEmployeeOvertimeReportAsync(
         OvertimeReportFilterDto filter,
         CancellationToken cancellationToken = default)
        {
            // =========================================================
            // Employee Filter
            // =========================================================

            if (!filter.EmployeeId.HasValue)
            {
                return Result<EmployeeOvertimeReportDto>.Failure(
                    "Employee must be specified.");
            }

            var employeeId =
                filter.EmployeeId.Value;


            // =========================================================
            // Employee
            // =========================================================

            var employee =
                await _context.Employees
                    .AsNoTracking()
                    .Where(x =>
                        x.Id == employeeId &&
                        x.IsActive)
                    .Select(x => new
                    {
                        x.Id,

                        EmployeeName =
                            x.User.FullName,

                        Department =
                            x.Department != null
                                ? x.Department.NameEn
                                : null
                    })
                    .FirstOrDefaultAsync(
                        cancellationToken);

            if (employee == null)
            {
                return Result<EmployeeOvertimeReportDto>
                    .Failure(
                        "Employee not found or inactive.");
            }


            // =========================================================
            // Query
            // =========================================================

            var query =
                _context.EmployeeOvertimes
                    .AsNoTracking()
                    .Where(x =>
                        x.EmployeeId == employeeId);


            // =========================================================
            // Date Filters
            // =========================================================

            if (filter.FromDate.HasValue)
            {
                query =
                    query.Where(x =>
                        x.StartDateTime >=
                        filter.FromDate.Value);
            }

            if (filter.ToDate.HasValue)
            {
                query =
                    query.Where(x =>
                        x.StartDateTime <=
                        filter.ToDate.Value);
            }


            // =========================================================
            // Type
            // =========================================================

            if (filter.Type.HasValue)
            {
                query =
                    query.Where(x =>
                        x.Type == filter.Type.Value);
            }


            // =========================================================
            // Status
            // =========================================================

            if (filter.Status.HasValue)
            {
                query =
                    query.Where(x =>
                        x.Status == filter.Status.Value);
            }


            // =========================================================
            // Source
            // =========================================================

            if (filter.Source.HasValue)
            {
                query =
                    query.Where(x =>
                        x.Source == filter.Source.Value);
            }


            // =========================================================
            // Paid
            // =========================================================

            if (filter.IsPaid.HasValue)
            {
                query =
                    query.Where(x =>
                        x.IsPaid == filter.IsPaid.Value);
            }


            // =========================================================
            // Details
            // =========================================================

            var overtimes =
                await query
                    .OrderByDescending(x =>
                        x.StartDateTime)
                    .Select(x =>
                        new EmployeeOvertimeReportItemDto
                        {
                            Id =
                                x.Id,

                            StartDateTime =
                                x.StartDateTime,

                            EndDateTime =
                                x.EndDateTime,

                            TotalMinutes =
                                x.TotalMinutes,

                            TotalHours =
                                Math.Round(
                                    x.TotalMinutes / 60m,
                                    2),

                            HourlyRate =
                                x.HourlyRate,

                            Multiplier =
                                x.Multiplier,

                            Amount =
                                x.Amount,

                            Type =
                                x.Type,

                            Status =
                                x.Status,

                            Source =
                                x.Source,

                            Reason =
                                x.Reason,

                            IsPaid =
                                x.IsPaid
                        })
                    .ToListAsync(
                        cancellationToken);


            // =========================================================
            // Statistics
            // =========================================================

            var totalMinutes =
                overtimes.Sum(x =>
                    x.TotalMinutes);

            var totalAmount =
                overtimes.Sum(x =>
                    x.Amount);

            var paidAmount =
                overtimes
                    .Where(x => x.IsPaid)
                    .Sum(x => x.Amount);

            var unpaidAmount =
                overtimes
                    .Where(x => !x.IsPaid)
                    .Sum(x => x.Amount);


            // =========================================================
            // Result
            // =========================================================

            var result =
                new EmployeeOvertimeReportDto
                {
                    EmployeeId =
                        employee.Id,

                    EmployeeName =
                        employee.EmployeeName,

                    Department =
                        employee.Department,

                    Overtimes =
                        overtimes,

                    TotalMinutes =
                        totalMinutes,

                    TotalHours =
                        Math.Round(
                            totalMinutes / 60m,
                            2,
                            MidpointRounding.AwayFromZero),

                    TotalAmount =
                        Math.Round(
                            totalAmount,
                            2,
                            MidpointRounding.AwayFromZero),

                    PaidAmount =
                        Math.Round(
                            paidAmount,
                            2,
                            MidpointRounding.AwayFromZero),

                    UnpaidAmount =
                        Math.Round(
                            unpaidAmount,
                            2,
                            MidpointRounding.AwayFromZero)
                };


            return Result<EmployeeOvertimeReportDto>
                .Succeeded(result);
        }

         public async Task<Result<EmployeeOvertimeReportDto>>
         GetEmployeeOvertimeSourceReportAsync(
         OvertimeReportFilterDto filter,
         CancellationToken cancellationToken = default)
        {
            // =========================================================
            // Employee Filter
            // =========================================================

            if (!filter.EmployeeId.HasValue)
            {
                return Result<EmployeeOvertimeReportDto>.Failure(
                    "Employee must be specified.");
            }

            var employeeId =
                filter.EmployeeId.Value;

            if (!filter.Source.HasValue)
            {
                return Result<EmployeeOvertimeReportDto>.Failure(
                    "Source must be specified.");
            }

            var source =
                filter.Source.Value;

            // =========================================================
            // Employee
            // =========================================================

            var employee =
                await _context.Employees
                    .AsNoTracking()
                    .Where(x =>
                        x.Id == employeeId &&
                        x.IsActive)
                    .Select(x => new
                    {
                        x.Id,

                        EmployeeName =
                            x.User.FullName,

                        Department =
                            x.Department != null
                                ? x.Department.NameAr
                                : null
                    })
                    .FirstOrDefaultAsync(
                        cancellationToken);

            if (employee == null)
            {
                return Result<EmployeeOvertimeReportDto>
                    .Failure(
                        "Employee not found or inactive.");
            }


            // =========================================================
            // Query
            // =========================================================

            var query =
                _context.EmployeeOvertimes
                    .AsNoTracking()
                    .Where(x =>
                        x.EmployeeId == employeeId &&
                         x.Source == source);


            // =========================================================
            // Date Filters
            // =========================================================

            if (filter.FromDate.HasValue)
            {
                query =
                    query.Where(x =>
                        x.StartDateTime >=
                        filter.FromDate.Value);
            }

            if (filter.ToDate.HasValue)
            {
                query =
                    query.Where(x =>
                        x.StartDateTime <=
                        filter.ToDate.Value);
            }



            // =========================================================
            // Status
            // =========================================================

            if (filter.Status.HasValue)
            {
                query =
                    query.Where(x =>
                        x.Status == filter.Status.Value);
            }


            // =========================================================
            // Source
            // =========================================================

            if (filter.Type.HasValue)
            {
                query =
                    query.Where(x =>
                        x.Type == filter.Type.Value);
            }


            // =========================================================
            // Paid
            // =========================================================

            if (filter.IsPaid.HasValue)
            {
                query =
                    query.Where(x =>
                        x.IsPaid == filter.IsPaid.Value);
            }


            // =========================================================
            // Details
            // =========================================================

            var overtimes =
                await query
                    .OrderByDescending(x =>
                        x.StartDateTime)
                    .Select(x =>
                        new EmployeeOvertimeReportItemDto
                        {
                            Id =
                                x.Id,

                            StartDateTime =
                                x.StartDateTime,

                            EndDateTime =
                                x.EndDateTime,

                            TotalMinutes =
                                x.TotalMinutes,

                            TotalHours =
                                Math.Round(
                                    x.TotalMinutes / 60m,
                                    2),

                            HourlyRate =
                                x.HourlyRate,

                            Multiplier =
                                x.Multiplier,

                            Amount =
                                x.Amount,

                            Type =
                                x.Type,

                            Status =
                                x.Status,

                            Source =
                                x.Source,

                            Reason =
                                x.Reason,

                            IsPaid =
                                x.IsPaid
                        })
                    .ToListAsync(
                        cancellationToken);


            // =========================================================
            // Statistics
            // =========================================================

            var totalMinutes =
                overtimes.Sum(x =>
                    x.TotalMinutes);

            var totalAmount =
                overtimes.Sum(x =>
                    x.Amount);

            var paidAmount =
                overtimes
                    .Where(x => x.IsPaid)
                    .Sum(x => x.Amount);

            var unpaidAmount =
                overtimes
                    .Where(x => !x.IsPaid)
                    .Sum(x => x.Amount);


            // =========================================================
            // Result
            // =========================================================

            var result =
                new EmployeeOvertimeReportDto
                {
                    EmployeeId =
                        employee.Id,

                    EmployeeName =
                        employee.EmployeeName,

                    Department =
                        employee.Department,
                    Source=source,

                    Overtimes =
                        overtimes,

                    TotalMinutes =
                        totalMinutes,

                    TotalHours =
                        Math.Round(
                            totalMinutes / 60m,
                            2,
                            MidpointRounding.AwayFromZero),

                    TotalAmount =
                        Math.Round(
                            totalAmount,
                            2,
                            MidpointRounding.AwayFromZero),

                    PaidAmount =
                        Math.Round(
                            paidAmount,
                            2,
                            MidpointRounding.AwayFromZero),

                    UnpaidAmount =
                        Math.Round(
                            unpaidAmount,
                            2,
                            MidpointRounding.AwayFromZero)
                };


            return Result<EmployeeOvertimeReportDto>
                .Succeeded(result);
        }


         public async Task<Result<EmployeeOvertimeReportDto>>
         GetEmployeeOvertimeTypeReportAsync(
         OvertimeReportFilterDto filter,
         CancellationToken cancellationToken = default)
        {
            // =========================================================
            // Employee Filter
            // =========================================================

            if (!filter.EmployeeId.HasValue)
            {
                return Result<EmployeeOvertimeReportDto>.Failure(
                    "Employee must be specified.");
            }

            var employeeId =
                filter.EmployeeId.Value;

            if (!filter.Type.HasValue)
            {
                return Result<EmployeeOvertimeReportDto>.Failure(
                    "Type must be specified.");
            }

            var type =
                filter.Type.Value;

            // =========================================================
            // Employee
            // =========================================================

            var employee =
                await _context.Employees
                    .AsNoTracking()
                    .Where(x =>
                        x.Id == employeeId &&
                        x.IsActive)
                    .Select(x => new
                    {
                        x.Id,

                        EmployeeName =
                            x.User.FullName,

                        Department =
                            x.Department != null
                                ? x.Department.NameAr
                                : null
                    })
                    .FirstOrDefaultAsync(
                        cancellationToken);

            if (employee == null)
            {
                return Result<EmployeeOvertimeReportDto>
                    .Failure(
                        "Employee not found or inactive.");
            }


            // =========================================================
            // Query
            // =========================================================

            var query =
                _context.EmployeeOvertimes
                    .AsNoTracking()
                    .Where(x =>
                        x.EmployeeId == employeeId &&
                         x.Type == type);


            // =========================================================
            // Date Filters
            // =========================================================

            if (filter.FromDate.HasValue)
            {
                query =
                    query.Where(x =>
                        x.StartDateTime >=
                        filter.FromDate.Value);
            }

            if (filter.ToDate.HasValue)
            {
                query =
                    query.Where(x =>
                        x.StartDateTime <=
                        filter.ToDate.Value);
            }



            // =========================================================
            // Status
            // =========================================================

            if (filter.Status.HasValue)
            {
                query =
                    query.Where(x =>
                        x.Status == filter.Status.Value);
            }


            // =========================================================
            // Source
            // =========================================================

            if (filter.Source.HasValue)
            {
                query =
                    query.Where(x =>
                        x.Source == filter.Source.Value);
            }


            // =========================================================
            // Paid
            // =========================================================

            if (filter.IsPaid.HasValue)
            {
                query =
                    query.Where(x =>
                        x.IsPaid == filter.IsPaid.Value);
            }


            // =========================================================
            // Details
            // =========================================================

            var overtimes =
                await query
                    .OrderByDescending(x =>
                        x.StartDateTime)
                    .Select(x =>
                        new EmployeeOvertimeReportItemDto
                        {
                            Id =
                                x.Id,

                            StartDateTime =
                                x.StartDateTime,

                            EndDateTime =
                                x.EndDateTime,

                            TotalMinutes =
                                x.TotalMinutes,

                            TotalHours =
                                Math.Round(
                                    x.TotalMinutes / 60m,
                                    2),

                            HourlyRate =
                                x.HourlyRate,

                            Multiplier =
                                x.Multiplier,

                            Amount =
                                x.Amount,

                            Type =
                                x.Type,

                            Status =
                                x.Status,

                            Source =
                                x.Source,

                            Reason =
                                x.Reason,

                            IsPaid =
                                x.IsPaid
                        })
                    .ToListAsync(
                        cancellationToken);


            // =========================================================
            // Statistics
            // =========================================================

            var totalMinutes =
                overtimes.Sum(x =>
                    x.TotalMinutes);

            var totalAmount =
                overtimes.Sum(x =>
                    x.Amount);

            var paidAmount =
                overtimes
                    .Where(x => x.IsPaid)
                    .Sum(x => x.Amount);

            var unpaidAmount =
                overtimes
                    .Where(x => !x.IsPaid)
                    .Sum(x => x.Amount);


            // =========================================================
            // Result
            // =========================================================

            var result =
                new EmployeeOvertimeReportDto
                {
                    EmployeeId =
                        employee.Id,

                    EmployeeName =
                        employee.EmployeeName,

                    Department =
                        employee.Department,

                    Type=type,

                    Overtimes =
                        overtimes,

                    TotalMinutes =
                        totalMinutes,

                    TotalHours =
                        Math.Round(
                            totalMinutes / 60m,
                            2,
                            MidpointRounding.AwayFromZero),

                    TotalAmount =
                        Math.Round(
                            totalAmount,
                            2,
                            MidpointRounding.AwayFromZero),

                    PaidAmount =
                        Math.Round(
                            paidAmount,
                            2,
                            MidpointRounding.AwayFromZero),

                    UnpaidAmount =
                        Math.Round(
                            unpaidAmount,
                            2,
                            MidpointRounding.AwayFromZero)
                };


            return Result<EmployeeOvertimeReportDto>
                .Succeeded(result);
        }
    }
}
