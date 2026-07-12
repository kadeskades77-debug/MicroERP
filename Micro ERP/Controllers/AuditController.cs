using MicroERP.Application.Authorization.Permissions;
using MicroERP.Application.Features.Audit.DTOs;
using MicroERP.Application.Features.Audit.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AuditController : ControllerBase
{
    private readonly IAuditService _auditService;

    public AuditController(
        IAuditService auditService)
    {
        _auditService = auditService;
    }


    //================ GET ALL =================

    [HttpGet]
    [Authorize(Policy = AuditPermissions.Audit.View)]
    public async Task<IActionResult> GetAll([FromQuery] AuditLogFilterDto dto)
    {
        var result =
            await _auditService.GetAllAsync(dto);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }


    //================ GET BY ID =================

    [HttpGet("{id:int}")]
    [Authorize(Policy = AuditPermissions.Audit.View)]
    public async Task<IActionResult> GetById(int id)
    {
        var result =
            await _auditService.GetByIdAsync(id);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }
}