using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroERP.Application.Authorization.Permissions;
using MicroERP.Application.Features.Ticketing.DTOs;
using MicroERP.Application.Features.Ticketing.Interfaces;

namespace Micro_ERP.Controllers.Ticketing;

[ApiController]
[Route("api/ticketing/assignments")]
public class TicketAssignmentController : ControllerBase
{
    private readonly ITicketAssignmentService _service;

    public TicketAssignmentController(ITicketAssignmentService service)
    {
        _service = service;
    }

    [HttpPost]
    [Authorize(Policy = TicketPermissions.Assignment.Assign)]
    public async Task<IActionResult> Assign(
        AssignTicketDto dto,
        CancellationToken ct)
    {
        var result = await _service.AssignAsync(dto, ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete]
    [Authorize(Policy = TicketPermissions.Assignment.Unassign)]
    public async Task<IActionResult> Unassign(
        UnassignTicketDto dto,
        CancellationToken ct)
    {
        var result = await _service.UnassignAsync(dto, ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("ticket/{ticketId:int}")]
    [Authorize(Policy = TicketPermissions.Assignment.View)]
    public async Task<IActionResult> GetByTicketId(
        int ticketId,
        CancellationToken ct)
    {
        var result = await _service.GetHistoryAsync(ticketId, ct);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
