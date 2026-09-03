using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.Attendance;

[Route("api/hr/attendance-logs")]
[ApiController]
public class AttendanceLogController : ControllerBase
{
    private readonly IAttendanceLogService _service;


    public AttendanceLogController(
        IAttendanceLogService service)
    {
        _service = service;
    }



    [HttpPost("receive")]
    [Authorize(Policy = HRPermissions.AttendanceDevice.ReceiveLogs)]
    public async Task<IActionResult> Receive(
        CreateAttendanceLogDto dto,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.ReceiveAsync(
                dto,
                cancellationToken);


        return Ok(result);
    }
}