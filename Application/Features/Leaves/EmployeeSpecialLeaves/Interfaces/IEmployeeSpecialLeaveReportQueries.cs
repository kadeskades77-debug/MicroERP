using MicroERP.Application.Features.Leaves.EmployeeSpecialLeaves.DTOs;

namespace MicroERP.Application.Features.Leaves.EmployeeSpecialLeaves.Interfaces;

public interface IEmployeeSpecialLeaveReportQueries
{
    Task<List<SpecialLeaveReportDto>> GetReportAsync(
        SpecialLeaveReportFilterDto filter,
        CancellationToken cancellationToken = default);


    Task<SpecialLeaveSummaryDto> GetSummaryAsync(
        SpecialLeaveReportFilterDto filter,
        CancellationToken cancellationToken = default);
}