using MicroERP.Application.Features.Leaves.EmployeeLeaves.DTOs;

namespace MicroERP.Application.Features.Leaves.EmployeeLeaves.Interfaces
{
    public interface IEmployeeLeaveReportQueries
    {
        Task<List<LeaveReportDto>> GetReportAsync(
            LeaveReportFilterDto filter,
            CancellationToken cancellationToken = default);

        Task<LeaveSummaryDto> GetSummaryAsync(
            LeaveReportFilterDto filter,
            CancellationToken cancellationToken = default);
    }
}
