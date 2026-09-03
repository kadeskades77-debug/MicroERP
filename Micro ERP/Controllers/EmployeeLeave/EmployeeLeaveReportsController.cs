using MicroERP.Application.Features.Leaves.EmployeeLeaves.DTOs;
using MicroERP.Application.Features.Leaves.EmployeeLeaves.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.EmployeeLeave;

[Route("api/leave-reports")]
[ApiController]
[Authorize]
[Authorize(Policy = HRPermissions.EmployeeLeave.View)]
public class EmployeeLeaveReportsController : ControllerBase
{
    private readonly IEmployeeLeaveReportQueries _queries;

    public EmployeeLeaveReportsController(
        IEmployeeLeaveReportQueries queries)
    {
        _queries = queries;
    }

    /// <summary>
    /// تفاصيل الإجازات
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetReport(
        [FromQuery] LeaveReportFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetReportAsync(
                filter,
                cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// ملخص الإجازات
    /// </summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(
        [FromQuery] LeaveReportFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetSummaryAsync(
                filter,
                cancellationToken);

        return Ok(result);
    }
}