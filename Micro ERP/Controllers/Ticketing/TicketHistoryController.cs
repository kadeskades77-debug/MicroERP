using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroERP.Application.Authorization.Permissions;
using MicroERP.Application.Features.Ticketing.Interfaces;

namespace Micro_ERP.Controllers.Ticketing;

[ApiController]
[Route("api/ticketing/history")]
public class TicketHistoryController : ControllerBase
{
    private readonly ITicketHistoryService _service;

    public TicketHistoryController(ITicketHistoryService service)
    {
        _service = service;
    }

    [HttpGet("ticket/{ticketId:int}")]
    [Authorize(Policy = TicketPermissions.History.View)]
    public async Task<IActionResult> GetByTicketId(
        int ticketId,
        CancellationToken ct)
    {
        var result = await _service.GetByTicketIdAsync(ticketId, ct);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
