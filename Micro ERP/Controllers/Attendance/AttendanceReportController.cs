using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Report;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.Attendance;

[Route("api/[controller]")]
[ApiController]
public class AttendanceReportController : ControllerBase
{
    private readonly IAttendanceReportQueries _service;


    public AttendanceReportController(
        IAttendanceReportQueries service)
    {
        _service = service;
    }



    [HttpGet]
    [Authorize(Policy = HRPermissions.Attendance.View)]
    public async Task<IActionResult> Get(
        [FromQuery] AttendanceReportFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.GetAttendanceReportAsync(
                filter,
                cancellationToken);



        if (!result.Success)
            return BadRequest(result);



        return Ok(result);
    }

    [HttpGet("late")]
    [Authorize(Policy = HRPermissions.Attendance.View)]
    public async Task<IActionResult> GetLateReport(
    [FromQuery] LateAttendanceFilterDto filter,
    CancellationToken cancellationToken)
    {
        var result =
            await _service.GetLateReportAsync(
                filter,
                cancellationToken);


        if (!result.Success)
            return BadRequest(result);


        return Ok(result);
    }

    [HttpGet("absent")]
    [Authorize(Policy = HRPermissions.Attendance.View)]
    public async Task<IActionResult> GetAbsentReport(
    [FromQuery] AbsentAttendanceFilterDto filter,
    CancellationToken cancellationToken)
    {
        var result =
            await _service.GetAbsentReportAsync(
                filter,
                cancellationToken);


        if (!result.Success)
            return BadRequest(result);


        return Ok(result);
    }

    [HttpGet("top-late")]
    [Authorize(Policy = HRPermissions.Attendance.View)]
    public async Task<IActionResult> GetTopLateEmployees(
  [FromQuery] DateOnly? from,
  [FromQuery] DateOnly? to,
  [FromQuery] int top = 10,
  CancellationToken cancellationToken = default)
    {
        var result =
            await _service.GetTopLateEmployeesAsync(
                from,
                to,
                top,
                cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("top-absent")]
    [Authorize(Policy = HRPermissions.Attendance.View)]
    public async Task<IActionResult> GetTopAbsentEmployees(
    [FromQuery] DateOnly? from,
    [FromQuery] DateOnly? to,
    [FromQuery] int top = 10,
    CancellationToken cancellationToken = default)
    {
        var result =
            await _service.GetTopAbsentEmployeesAsync(
                from,
                to,
                top,
                cancellationToken);

        return Ok(result);
    }
}