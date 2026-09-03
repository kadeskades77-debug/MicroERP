using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroERP.Application.Authorization.Permissions;
using MicroERP.Application.Features.Ticketing.DTOs;
using MicroERP.Application.Features.Ticketing.Interfaces;

namespace Micro_ERP.Controllers.Ticketing;

[ApiController]
[Route("api/ticketing/tickets")]
public class TicketController : ControllerBase
{
    private readonly ITicketService _service;

    public TicketController(ITicketService service)
    {
        _service = service;
    }

    [HttpPost]
    [Authorize(Policy = TicketPermissions.Ticket.Create)]
    public async Task<IActionResult> Create(
        CreateTicketDto dto,
        CancellationToken ct)
    {
        var result = await _service.CreateAsync(dto, ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = TicketPermissions.Ticket.View)]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet]
    [Authorize(Policy = TicketPermissions.Ticket.View)]
    public async Task<IActionResult> GetAll(
        [FromQuery] TicketFilterDto filter,
        CancellationToken ct)
    {
        var result = await _service.GetAllAsync(filter, ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id:int}")]
    [Authorize(Policy = TicketPermissions.Ticket.Update)]
    public async Task<IActionResult> Update(
        int id,
        UpdateTicketDto dto,
        CancellationToken ct)
    {
        var result = await _service.UpdateAsync(id, dto, ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("status")]
    [Authorize(Policy = TicketPermissions.Ticket.ChangeStatus)]
    public async Task<IActionResult> ChangeStatus(
        ChangeTicketStatusDto dto,
        CancellationToken ct)
    {
        var result = await _service.ChangeStatusAsync(dto, ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("priority")]
    [Authorize(Policy = TicketPermissions.Ticket.ChangePriority)]
    public async Task<IActionResult> ChangePriority(
        ChangeTicketPriorityDto dto,
        CancellationToken ct)
    {
        var result = await _service.ChangePriorityAsync(dto, ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // =========================================================
    // Start Ticket
    // =========================================================

    [HttpPost("start")]
    [Authorize(Policy = TicketPermissions.Ticket.ChangeStatus)]
    public async Task<IActionResult> Start(
        TicketActionDto dto,
        CancellationToken ct)
    {
        var result =
            await _service.StartAsync(
                dto,
                ct);

        return result.Success
            ? Ok(result)
            : BadRequest(result);
    }

    [HttpPost("resolve")]
    [Authorize(Policy = TicketPermissions.Ticket.Resolve)]
    public async Task<IActionResult> Resolve(
       TicketActionDto dto,
        CancellationToken ct)
    {
        var result = await _service.ResolveAsync(dto, ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("close")]
    [Authorize(Policy = TicketPermissions.Ticket.Close)]
    public async Task<IActionResult> Close(
     TicketActionDto dto,
        CancellationToken ct)
    {
        var result = await _service.CloseAsync(dto, ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("reopen")]
    [Authorize(Policy = TicketPermissions.Ticket.Reopen)]
    public async Task<IActionResult> Reopen(
       TicketActionDto dto,
        CancellationToken ct)
    {
        var result = await _service.ReopenAsync(dto, ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = TicketPermissions.Ticket.Delete)]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken ct)
    {
        var result = await _service.DeleteAsync(id, ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
