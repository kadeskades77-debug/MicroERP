using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroERP.Application.Authorization.Permissions;
using MicroERP.Application.Features.Ticketing.DTOs;
using MicroERP.Application.Features.Ticketing.Interfaces;

namespace Micro_ERP.Controllers.Ticketing;

[ApiController]
[Route("api/ticketing/attachments")]
public class TicketAttachmentController : ControllerBase
{
    private readonly ITicketAttachmentService _service;

    public TicketAttachmentController(ITicketAttachmentService service)
    {
        _service = service;
    }

    [HttpPost]
    [Authorize(Policy = TicketPermissions.Attachment.Upload)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Add(
        [FromForm] AddTicketAttachmentDto dto,
        CancellationToken ct)
    {
        var result = await _service.UploadAsync(dto, ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("ticket/{ticketId:int}")]
    [Authorize(Policy = TicketPermissions.Attachment.View)]
    public async Task<IActionResult> GetByTicketId(
        int ticketId,
        CancellationToken ct)
    {
        var result = await _service.GetByTicketIdAsync(ticketId, ct);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = TicketPermissions.Attachment.Delete)]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken ct)
    {
        var result = await _service.DeleteAsync(id, ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
