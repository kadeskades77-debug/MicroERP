using MicroERP.Application.Features.Leaves.EmployeeLeaveBalances.DTOs;

namespace MicroERP.Application.Features.Leaves.EmployeeLeaveBalances.Interfaces;

public interface ILeaveBalanceReportQueries
{
    Task<List<LeaveBalanceReportDto>> GetReportAsync(
        LeaveBalanceReportFilterDto filter,
        CancellationToken cancellationToken = default);



    Task<LeaveBalanceSummaryDto> GetSummaryAsync(
        LeaveBalanceReportFilterDto filter,
        CancellationToken cancellationToken = default);
}