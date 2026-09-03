using MicroERP.Application.Features.Leaves.EmployeeLeaveBalances.DTOs;
using MicroERP.Application.Features.Leaves.EmployeeLeaveBalances.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.EmployeeLeave;


[Route("api/leave-balance-reports")]
[ApiController]
[Authorize(Policy = HRPermissions.EmployeeLeave.View)]
public class LeaveBalanceReportsController
    : ControllerBase
{
    private readonly ILeaveBalanceReportQueries _queries;


    public LeaveBalanceReportsController(
        ILeaveBalanceReportQueries queries)
    {
        _queries = queries;
    }





    // تقرير الأرصدة
    [HttpGet]
    public async Task<IActionResult> GetReport(
        [FromQuery] LeaveBalanceReportFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetReportAsync(
                filter,
                cancellationToken);


        return Ok(result);
    }





    // ملخص الأرصدة
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(
        [FromQuery] LeaveBalanceReportFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetSummaryAsync(
                filter,
                cancellationToken);


        return Ok(result);
    }
}