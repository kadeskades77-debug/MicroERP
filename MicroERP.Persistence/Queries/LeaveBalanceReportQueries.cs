using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Features.Leaves.EmployeeLeaveBalances.DTOs;
using MicroERP.Application.Features.Leaves.EmployeeLeaveBalances.Interfaces;
using MicroERP.Domin.Entities.EmployeeLeaves;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Persistence.Queries;

public class LeaveBalanceReportQueries
    : ILeaveBalanceReportQueries
{
    private readonly IApplicationDbContext _context;


    public LeaveBalanceReportQueries(
        IApplicationDbContext context)
    {
        _context = context;
    }





    public async Task<List<LeaveBalanceReportDto>> GetReportAsync(
        LeaveBalanceReportFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var query =
            ApplyFilters(filter);



        return await query

            .OrderBy(x => x.EmployeeId)

            .Select(x => new LeaveBalanceReportDto
            {
                EmployeeId =
                    x.EmployeeId,


                EmployeeName =
                    x.Employee.User.FullName,


                Department =
                    x.Employee.Department.NameAr,


                LeaveType =
                    x.LeaveType,


                Year =
                    x.Year,


                TotalDays =
                    x.TotalDays,


                UsedDays =
                    x.UsedDays,


                RemainingDays =
                    x.TotalDays -
                    x.UsedDays,


                UsagePercentage =
                  x.TotalDays == 0
                               ? 0
                               :
                 Math.Round(
                     ((double)x.UsedDays /
                     x.TotalDays) * 100,
                     2)
                
            })

            .ToListAsync(
                cancellationToken);
    }





    public async Task<LeaveBalanceSummaryDto> GetSummaryAsync(
        LeaveBalanceReportFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var query =
            ApplyFilters(filter);



        var totalBalanceDays =
            await query.SumAsync(
                x => x.TotalDays,
                cancellationToken);



        var usedDays =
            await query.SumAsync(
                x => x.UsedDays,
                cancellationToken);



        return new LeaveBalanceSummaryDto
        {
            TotalEmployees =
                await query
                .Select(x => x.EmployeeId)
                .Distinct()
                .CountAsync(cancellationToken),


            TotalBalanceDays =
                totalBalanceDays,


            UsedDays =
                usedDays,


            RemainingDays =
                totalBalanceDays -
                usedDays
        };
    }





    private IQueryable<EmployeeLeaveBalance> ApplyFilters(
        LeaveBalanceReportFilterDto filter)
    {
        var query =
            _context.EmployeeLeaveBalances
            .AsNoTracking()
            .Include(x => x.Employee)
                .ThenInclude(x => x.Department)
            .Include(x => x.Employee)
                .ThenInclude(x => x.User)
            .AsQueryable();




        if (filter.EmployeeId.HasValue)
        {
            query =
                query.Where(x =>
                    x.EmployeeId ==
                    filter.EmployeeId.Value);
        }





        if (filter.DepartmentId.HasValue)
        {
            query =
                query.Where(x =>
                    x.Employee.DepartmentId ==
                    filter.DepartmentId.Value);
        }





        if (filter.LeaveType.HasValue)
        {
            query =
                query.Where(x =>
                    x.LeaveType ==
                    filter.LeaveType.Value);
        }





        if (filter.Year.HasValue)
        {
            query =
                query.Where(x =>
                    x.Year ==
                    filter.Year.Value);
        }





        return query;
    }
}