using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using MicroERP.Application.Features.EmployeeAttendance.AttendanceLogs.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.Attendance;

[ApiController]
[Route("api/attendance-test")]
public class AttendanceTestController : ControllerBase
{
    private readonly IAttendanceLogProcessor _processor;
    private readonly IAttendanceRecalculateService _service;


    public AttendanceTestController(
        IAttendanceLogProcessor processor, IAttendanceRecalculateService service)
    {
        _processor = processor;
        _service = service;
    }


    [HttpPost("process")]
    public async Task<IActionResult> Process(
        CancellationToken cancellationToken)
    {
        await _processor.ProcessPendingLogsAsync(
            cancellationToken);

        return Ok("Attendance logs processed successfully");
    }

    [HttpPost("day")]
    public async Task<IActionResult> RecalculateDay(
      DateOnly date,
      CancellationToken cancellationToken)
    {
        var result =
            await _service.RecalculateDayAsync(
                date,
                cancellationToken);


        return Ok(result);
    }

    [HttpPost("recalculate-month")]
    public async Task<IActionResult> RecalculateMonth(
    [FromQuery] int year,
    [FromQuery] int month,
    CancellationToken cancellationToken)
    {
        await _processor.ProcessPendingLogsAsync(
           cancellationToken);
        var result =
            await _service
                .RecalculateMonthAsync(
                    year,
                    month,
                    cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}