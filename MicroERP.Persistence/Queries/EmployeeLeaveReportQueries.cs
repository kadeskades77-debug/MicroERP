using MicroERP.Application.Features.Leaves.EmployeeLeaves.DTOs;
using MicroERP.Application.Features.Leaves.EmployeeLeaves.Interfaces;
using MicroERP.Domin.Entities.EmployeeLeaves;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Persistence.Queries
{
    public class EmployeeLeaveReportQueries
    : IEmployeeLeaveReportQueries
    {
        private readonly ApplicationDbContext _context;

        public EmployeeLeaveReportQueries(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<LeaveReportDto>> GetReportAsync(
            LeaveReportFilterDto filter,
            CancellationToken cancellationToken = default)
        {
           
            var query = ApplyFilters(filter);

            return await query
                .OrderByDescending(x => x.StartDate)
                .Select(x => new LeaveReportDto
                {
                    LeaveId = x.Id,

                    EmployeeId = x.EmployeeId,

                    EmployeeCode =
                        x.Employee.Id.ToString(),

                    EmployeeName =
                        x.Employee.User.FullName,

                    Department =
                        x.Employee.Department.NameAr,

                    LeaveType =
                        x.LeaveType,

                    Status =
                        x.Status,

                    StartDate =
                        x.StartDate,

                    EndDate =
                        x.EndDate,

                    TotalDays =
                        x.TotalDays,

                    SickDays =
                        x.SickDays,

                    EmergencyDays =
                        x.EmergencyDays,

                    AnnualDays =
                        x.AnnualDays,

                    UnpaidDays =
                        x.UnpaidDays,

                    Reason =
                        x.Reason
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<LeaveSummaryDto> GetSummaryAsync(
        LeaveReportFilterDto filter,
        CancellationToken cancellationToken = default)
        {
            var query =
                ApplyFilters(filter);


            var summary =
                new LeaveSummaryDto
                {
                    TotalRequests =
                        await query.CountAsync(
                            cancellationToken),

                    Approved =
                        await query.CountAsync(
                            x => x.Status == LeaveStatus.Approved,
                            cancellationToken),

                    Pending =
                        await query.CountAsync(
                            x => x.Status == LeaveStatus.Pending,
                            cancellationToken),

                    Rejected =
                        await query.CountAsync(
                            x => x.Status == LeaveStatus.Rejected,
                            cancellationToken),

                    Cancelled =
                        await query.CountAsync(
                            x => x.Status == LeaveStatus.Cancelled,
                            cancellationToken),

                    TotalDays =
                        await query.SumAsync(
                            x => x.TotalDays,
                            cancellationToken),

                    SickDays =
                        await query.SumAsync(
                            x => x.SickDays,
                            cancellationToken),

                    EmergencyDays =
                        await query.SumAsync(
                            x => x.EmergencyDays,
                            cancellationToken),

                    AnnualDays =
                        await query.SumAsync(
                            x => x.AnnualDays,
                            cancellationToken),

                    UnpaidDays =
                        await query.SumAsync(
                            x => x.UnpaidDays,
                            cancellationToken)
                };


            return summary;
        }


        // ==================== Helpers =======================

        private IQueryable<EmployeeLeave> ApplyFilters(
    LeaveReportFilterDto filter)
        {
            var query = _context.EmployeeLeaves
                .AsNoTracking()
                .Include(x => x.Employee)
                    .ThenInclude(x => x.Department)
                .AsQueryable();


            if (filter.EmployeeId.HasValue)
            {
                query = query.Where(x =>
                    x.EmployeeId ==
                    filter.EmployeeId.Value);
            }


            if (filter.DepartmentId.HasValue)
            {
                query = query.Where(x =>
                    x.Employee.DepartmentId ==
                    filter.DepartmentId.Value);
            }


            if (filter.LeaveType.HasValue)
            {
                query = query.Where(x =>
                    x.LeaveType ==
                    filter.LeaveType.Value);
            }


            if (filter.Status.HasValue)
            {
                query = query.Where(x =>
                    x.Status ==
                    filter.Status.Value);
            }


            if (filter.Year.HasValue)
            {
                query = query.Where(x =>
                    x.StartDate.Year ==
                    filter.Year.Value);
            }


            if (filter.FromDate.HasValue)
            {
                query = query.Where(x =>
                    x.EndDate >=
                    filter.FromDate.Value);
            }


            if (filter.ToDate.HasValue)
            {
                query = query.Where(x =>
                    x.StartDate <=
                    filter.ToDate.Value);
            }


            return query;
        }
    }
}
