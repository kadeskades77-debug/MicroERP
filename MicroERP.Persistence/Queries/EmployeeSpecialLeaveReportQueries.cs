using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Features.Leaves.EmployeeSpecialLeaves.DTOs;
using MicroERP.Application.Features.Leaves.EmployeeSpecialLeaves.Interfaces;
using MicroERP.Domin.Entities.EmployeeLeaves;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Persistence.Queries;

public class EmployeeSpecialLeaveReportQueries
    : IEmployeeSpecialLeaveReportQueries
{
    private readonly IApplicationDbContext _context;


    public EmployeeSpecialLeaveReportQueries(
        IApplicationDbContext context)
    {
        _context = context;
    }



    public async Task<List<SpecialLeaveReportDto>> GetReportAsync(
        SpecialLeaveReportFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var query =
            ApplyFilters(filter);


        return await query
            .OrderByDescending(x => x.StartDate)

            .Select(x => new SpecialLeaveReportDto
            {
                LeaveId = x.Id,


                EmployeeId =
                    x.EmployeeId,


                EmployeeName =
                    x.Employee.User.FullName,


                Department =
                    x.Employee.Department.NameAr,


                Type =
                    x.Type,


                StartDate =
                    x.StartDate,


                EndDate =
                    x.EndDate,


                TotalDays =
                    x.TotalDays,


                Status =
                    x.Status,


                Reason =
                    x.Reason,


                ApprovedOn =
                    x.ApprovedOn

            })

            .ToListAsync(
                cancellationToken);
    }





    public async Task<SpecialLeaveSummaryDto> GetSummaryAsync(
        SpecialLeaveReportFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var query =
            ApplyFilters(filter);



        return new SpecialLeaveSummaryDto
        {
            TotalRequests =
                await query.CountAsync(
                    cancellationToken),


            Approved =
                await query.CountAsync(
                    x =>
                    x.Status ==
                    SpecialLeaveStatus.Approved,
                    cancellationToken),


            Pending =
                await query.CountAsync(
                    x =>
                    x.Status ==
                    SpecialLeaveStatus.Pending,
                    cancellationToken),


            Rejected =
                await query.CountAsync(
                    x =>
                    x.Status ==
                    SpecialLeaveStatus.Rejected,
                    cancellationToken),


            Cancelled =
                await query.CountAsync(
                    x =>
                    x.Status ==
                    SpecialLeaveStatus.Cancelled,
                    cancellationToken),


            TotalDays =
                await query.SumAsync(
                    x => x.TotalDays,
                    cancellationToken)
        };
    }





    private IQueryable<EmployeeSpecialLeave> ApplyFilters(
        SpecialLeaveReportFilterDto filter)
    {
        var query =
            _context.EmployeeSpecialLeaves
            .AsNoTracking()
            .Include(x => x.Employee)
                .ThenInclude(x => x.Department)
            .Include(x => x.Employee)
                .ThenInclude(x => x.User)
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



        if (filter.Type.HasValue)
        {
            query = query.Where(x =>
                x.Type ==
                filter.Type.Value);
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