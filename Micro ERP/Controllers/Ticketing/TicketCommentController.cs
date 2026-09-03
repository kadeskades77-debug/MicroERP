using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroERP.Application.Authorization.Permissions;
using MicroERP.Application.Features.Ticketing.DTOs;
using MicroERP.Application.Features.Ticketing.Interfaces;

namespace Micro_ERP.Controllers.Ticketing;

[ApiController]
[Route("api/ticketing/comments")]
public class TicketCommentController : ControllerBase
{
    private readonly ITicketCommentService _service;

    public TicketCommentController(ITicketCommentService service)
    {
        _service = service;
    }

    [HttpPost]
    [Authorize(Policy = TicketPermissions.Comment.Create)]
    public async Task<IActionResult> Add(
        AddTicketCommentDto dto,
        CancellationToken ct)
    {
        var result = await _service.CreateAsync(dto, ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }


    [HttpPut("{id:int}")]
    [Authorize(Policy = TicketPermissions.Comment.Create)]
    public async Task<IActionResult> UpdateAsync(
        int id,
        UpdateTicketCommentDto dto,
        CancellationToken ct)
    {
        var result = await _service.UpdateAsync(id,dto, ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("ticket/{ticketId:int}")]
    [Authorize(Policy = TicketPermissions.Comment.View)]
    public async Task<IActionResult> GetByTicketId(
        int ticketId,
        CancellationToken ct)
    {
        var result = await _service.GetByTicketIdAsync(ticketId, ct);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = TicketPermissions.Comment.Delete)]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken ct)
    {
        var result = await _service.DeleteAsync(id, ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
