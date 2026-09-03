using MicroERP.Application.Features.EmployeeAttendance.AttendanceTransactions.DTOs;
using MicroERP.Application.Features.EmployeeAttendance.AttendanceTransactions.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.Attendance;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AttendanceTransactionsController : ControllerBase
{
    private readonly IAttendanceTransactionService _service;

    public AttendanceTransactionsController(
        IAttendanceTransactionService service)
    {
        _service = service;
    }


    [HttpPost]
    [Authorize(Policy = HRPermissions.Attendance.Create)]
    public async Task<IActionResult> Create(
        CreateAttendanceTransactionDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(
            dto,
            cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }


    [HttpGet("attendance/{attendanceRecordId:int}")]
    [Authorize(Policy = HRPermissions.Attendance.View)]
    public async Task<IActionResult> GetByAttendance(
        int attendanceRecordId,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetByAttendanceAsync(
            attendanceRecordId,
            cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}