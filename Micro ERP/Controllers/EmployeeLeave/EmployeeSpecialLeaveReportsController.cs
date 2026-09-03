using MicroERP.Application.Features.Leaves.EmployeeSpecialLeaves.DTOs;
using MicroERP.Application.Features.Leaves.EmployeeSpecialLeaves.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.EmployeeLeave;


[Route("api/special-leave-reports")]
[ApiController]
[Authorize(Policy = HRPermissions.EmployeeSpecialLeave.View)]
public class EmployeeSpecialLeaveReportsController
    : ControllerBase
{
    private readonly IEmployeeSpecialLeaveReportQueries _queries;


    public EmployeeSpecialLeaveReportsController(
        IEmployeeSpecialLeaveReportQueries queries)
    {
        _queries = queries;
    }



    // تقرير تفصيلي
    [HttpGet]
    public async Task<IActionResult> GetReport(
        [FromQuery] SpecialLeaveReportFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetReportAsync(
                filter,
                cancellationToken);


        return Ok(result);
    }





    // ملخص التقرير
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(
        [FromQuery] SpecialLeaveReportFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetSummaryAsync(
                filter,
                cancellationToken);


        return Ok(result);
    }
}