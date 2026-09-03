using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroERP.Application.Authorization.Permissions;
using MicroERP.Application.Features.Ticketing.DTOs;
using MicroERP.Application.Features.Ticketing.Interfaces;

namespace Micro_ERP.Controllers.Ticketing;

[ApiController]
[Route("api/ticketing/categories")]
public class TicketCategoryController : ControllerBase
{
    private readonly ITicketCategoryService _service;

    public TicketCategoryController(ITicketCategoryService service)
    {
        _service = service;
    }

    [HttpPost]
    [Authorize(Policy = TicketPermissions.Category.Create)]
    public async Task<IActionResult> Create(
        CreateTicketCategoryDto dto,
        CancellationToken ct)
    {
        var result = await _service.CreateAsync(dto, ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = TicketPermissions.Category.View)]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet]
    [Authorize(Policy = TicketPermissions.Category.View)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _service.GetAllAsync(ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id:int}")]
    [Authorize(Policy = TicketPermissions.Category.Update)]
    public async Task<IActionResult> Update(
        int id,
        UpdateTicketCategoryDto dto,
        CancellationToken ct)
    {
        var result = await _service.UpdateAsync(id, dto, ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = TicketPermissions.Category.Delete)]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken ct)
    {
        var result = await _service.DeleteAsync(id, ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
