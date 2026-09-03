using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.Attendance;

[Route("api/attendance-corrections")]
[ApiController]
public class AttendanceCorrectionController : ControllerBase
{
    private readonly IAttendanceCorrectionService _service;


    public AttendanceCorrectionController(
        IAttendanceCorrectionService service)
    {
        _service = service;
    }



    // إنشاء طلب تصحيح
    [HttpPost]
    [Authorize(Policy = HRPermissions.AttendanceCorrection.Create)]
    public async Task<IActionResult> Create(
        CreateAttendanceCorrectionDto dto,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.CreateAsync(
                dto,
                cancellationToken);

        return Ok(result);
    }



    // عرض طلبات التصحيح المعلقة
    [HttpGet("pending")]
    [Authorize(Policy = HRPermissions.AttendanceCorrection.View)]
    public async Task<IActionResult> GetPending(
        CancellationToken cancellationToken)
    {
        var result =
            await _service.GetPendingAsync(
                cancellationToken);

        return Ok(result);
    }



    // اعتماد التصحيح
    [HttpPost("{id}/approve")]
    [Authorize(Policy = HRPermissions.AttendanceCorrection.Approve)]
    public async Task<IActionResult> Approve(
        int id,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.ApproveAsync(
                id,
                cancellationToken);

        return Ok(result);
    }



    // رفض التصحيح
    [HttpPost("{id}/reject")]
    [Authorize(Policy = HRPermissions.AttendanceCorrection.Reject)]
    public async Task<IActionResult> Reject(
        int id,
        RejectAttendanceCorrectionDto dto,
        CancellationToken cancellationToken)
    {
        if (id != dto.CorrectionId)
        {
            return BadRequest(
                "Correction id mismatch");
        }


        var result =
            await _service.RejectAsync(
                dto.CorrectionId,
                dto.RejectionReason,
                cancellationToken);


        return Ok(result);
    }
}