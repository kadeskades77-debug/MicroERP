using MicroERP.Application.Authorization.Permissions;
using MicroERP.Application.Features.Authentication.Roles.DTOs;
using MicroERP.Application.Features.Authentication.Roles.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.Auth;

[Route("api/[controller]")]
[Authorize]
public class RolesController : BaseApiController
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    //================ GET ALL =================

    [HttpGet]
    [Authorize(Policy = RolePermissionPermissions.Role.View)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        return HandleResult(
            await _roleService.GetAllAsync(
                cancellationToken));
    }

    //================ GET BY ID =================

    [HttpGet("{id}")]
    [Authorize(Policy = RolePermissionPermissions.Role.View)]
    public async Task<IActionResult> GetById(
        string id,
        CancellationToken cancellationToken)
    {
        return HandleResult(
            await _roleService.GetByIdAsync(
                id,
                cancellationToken));
    }

    //================ CREATE =================

    [HttpPost]
    [Authorize(Policy = RolePermissionPermissions.Role.Create)]
    public async Task<IActionResult> Create(
        CreateRoleDto dto,
        CancellationToken cancellationToken)
    {
        return HandleResult(
            await _roleService.CreateAsync(
                dto,
                cancellationToken));
    }

    //================ UPDATE =================

    [HttpPut("{id}")]
    [Authorize(Policy = RolePermissionPermissions.Role.Update)]
    public async Task<IActionResult> Update(
        string id,
        UpdateRoleDto dto,
        CancellationToken cancellationToken)
    {
        return HandleResult(
            await _roleService.UpdateAsync(
                id,
                dto,
                cancellationToken));
    }

    //================ DELETE =================

    [HttpDelete("{id}")]
    [Authorize(Policy = RolePermissionPermissions.Role.Delete)]
    public async Task<IActionResult> Delete(
        string id,
        CancellationToken cancellationToken)
    {
        return HandleResult(
            await _roleService.DeleteAsync(
                id,
                cancellationToken));
    }

    //================ LOOKUP =================

    [HttpGet("lookup")]
    public async Task<IActionResult> Lookup(
        CancellationToken cancellationToken)
    {
        return Ok(
            await _roleService.GetLookupAsync(
                cancellationToken));
    }
}