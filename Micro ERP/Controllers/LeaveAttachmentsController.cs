using MicroERP.Application.Features.Documents.LeaveDocuments.DTOS;
using MicroERP.Application.Features.Documents.LeaveDocuments.Interfaces;
using MicroERP.Domin.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LeaveAttachmentsController : ControllerBase
{
    private readonly ILeaveAttachmentService _service;


    public LeaveAttachmentsController(
        ILeaveAttachmentService service)
    {
        _service = service;
    }



    [HttpPost("employee-leave/{leaveId:int}")]
    [Authorize(Policy = HRPermissions.EmployeeLeave.Create)]
    public async Task<IActionResult> UploadEmployeeLeaveAttachment(int leaveId,
        [FromForm] UploadLeaveAttachmentDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.UploadEmployeeLeaveAsync(
            leaveId,
            dto,
            cancellationToken);


        if (!result.Success)
            return BadRequest(result);


        return Ok(result);
    }





    [HttpPost("special-leave/{leaveId:int}")]
    [Authorize(Policy = HRPermissions.EmployeeLeave.Create)]
    public async Task<IActionResult> UploadSpecialLeaveAttachment(int leaveId,
        [FromForm] UploadLeaveAttachmentDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.UploadEmployeeSpecialLeaveAsync(
            leaveId,
            dto,
            cancellationToken);


        if (!result.Success)
            return BadRequest(result);


        return Ok(result);
    }



 


    [HttpGet("{leaveId:int}")]
    [Authorize(Policy = HRPermissions.EmployeeLeave.View)]
    public async Task<IActionResult> GetByLeave(
        int leaveId,
        [FromQuery] LeaveCategory category,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetByLeaveAsync(
            leaveId,
            category,
            cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }





    [HttpDelete("{id:int}")]
    [Authorize(Policy = HRPermissions.EmployeeLeave.Delete)]
    public async Task<IActionResult> Delete(int id,
        CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(
            id,
            cancellationToken);


        if (!result.Success)
            return BadRequest(result);


        return Ok(result);
    }
}