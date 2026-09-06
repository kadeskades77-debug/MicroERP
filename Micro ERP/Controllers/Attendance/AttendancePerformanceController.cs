using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Dashboard;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.Attendance;


[Route("api/attendance/performance")]
[ApiController]
public class AttendancePerformanceController : ControllerBase
{
    private readonly IAttendancePerformanceService _service;
    private readonly IAttendancePerformanceQueries _queries;


    public AttendancePerformanceController(
        IAttendancePerformanceService service,
        IAttendancePerformanceQueries queries)
    {
        _service = service;
        _queries = queries;
    }



    [HttpPost("calculate")]
    [Authorize(
        Policy = HRPermissions.EmployeeEvaluation.Create)]
    public async Task<IActionResult> Calculate(
        int employeeId,
        [FromQuery] int year,
        [FromQuery] int month,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.CalculateAsync(
                employeeId,
                year,
                month,
                cancellationToken);


        return result.Success
            ? Ok(result)
            : BadRequest(result);
    }


    [HttpPost("calculate/ALL")]
    [Authorize(
        Policy = HRPermissions.EmployeeEvaluation.Create)]
    public async Task<IActionResult> CalculateAllAsync(
        [FromQuery] int year,
        [FromQuery] int month,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.CalculateAllAsync(
                year,
                month,
                cancellationToken);


        return result.Success
            ? Ok(result)
            : BadRequest(result);
    }



    [HttpGet("Month")]
    [Authorize(
        Policy = HRPermissions.EmployeeEvaluation.View)]
    public async Task<IActionResult> GetByMonth(
        int employeeId,
        int year,
        int month,
        CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetByEmployeeAndMonthAsync(
                employeeId,
                year,
                month,
                cancellationToken);


        return result.Success
            ? Ok(result)
            : NotFound(result);
    }





    [HttpGet("{employeeId:int}/history")]
    [Authorize(
        Policy = HRPermissions.EmployeeEvaluation.View)]
    public async Task<IActionResult> GetHistory(
        int employeeId,
        CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetEmployeeHistoryAsync(
                employeeId,
                cancellationToken);


        return Ok(result);
    }





    [HttpGet("top")]
    [Authorize(
        Policy = HRPermissions.EmployeeEvaluation.View)]
    public async Task<IActionResult> GetTop(
        [FromQuery] int year,
        [FromQuery] int month,
        [FromQuery] int top = 10,
        CancellationToken cancellationToken = default)
    {
        var result =
            await _queries.GetTopEmployeesAsync(
                year,
                month,
                top,
                cancellationToken);


        return Ok(result);
    }





    [HttpGet("lowest")]
    [Authorize(
        Policy = HRPermissions.EmployeeEvaluation.View)]
    public async Task<IActionResult> GetLowest(
        [FromQuery] int year,
        [FromQuery] int month,
        [FromQuery] int top = 10,
        CancellationToken cancellationToken = default)
    {
        var result =
            await _queries.GetLowestEmployeesAsync(
                year,
                month,
                top,
                cancellationToken);


        return Ok(result);
    }





    [HttpPost("paged")]
    [Authorize(
        Policy = HRPermissions.EmployeeEvaluation.View)]
    public async Task<IActionResult> GetPaged(
     [FromBody] AttendancePerformanceFilterDto filter,
     [FromQuery] PagedRequest request,
     CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetPagedAsync(
                filter,
                request,
                cancellationToken);


        return Ok(result);
    }

    [HttpGet("dashboard")]
    [Authorize(
        Policy = HRPermissions.EmployeeEvaluation.View)]
    public async Task<IActionResult> GetDashboard(
    [FromQuery] int year,
    [FromQuery] int month,
    CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetDashboardAsync(
                year,
                month,
                cancellationToken);

        return Ok(result);
    }

    [HttpGet("statistics")]
    [Authorize(
        Policy = HRPermissions.EmployeeEvaluation.View)]
    public async Task<IActionResult> GetMonthlyStatistics(
    [FromQuery] int year,
    [FromQuery] int month,
    CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetMonthlyStatisticsAsync(
                year,
                month,
                cancellationToken);

        return Ok(result);
    }

    [HttpGet("employees-summary")]
    [Authorize(
        Policy = HRPermissions.EmployeeEvaluation.View)]
    public async Task<IActionResult> GetEmployeePerformanceSummary(
    [FromQuery] int year,
    [FromQuery] int month,
    CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetEmployeePerformanceSummaryAsync(
                year,
                month,
                cancellationToken);

        return Ok(result);
    }

    [HttpGet("departments-summary")]
    [Authorize(
        Policy = HRPermissions.EmployeeEvaluation.View)]
    public async Task<IActionResult> GetDepartmentPerformanceSummary(
    [FromQuery] int year,
    [FromQuery] int month,
    CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetDepartmentPerformanceSummaryAsync(
                year,
                month,
                cancellationToken);

        return Ok(result);
    }
}