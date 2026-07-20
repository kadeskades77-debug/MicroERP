using MicroERP.Application.Authorization.Permissions;
using MicroERP.Application.Features.Roles.DTOs;
using MicroERP.Application.Features.Roles.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers;

[Route("api/[controller]")]
[Authorize]
public class RolesController : BaseApiController
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    [Authorize(Policy = RolePermissionPermissions.Role.View)]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await _roleService.GetAllAsync());
    }

    [HttpGet("{id}")]
    [Authorize(Policy = RolePermissionPermissions.Role.View)]
    public async Task<IActionResult> GetById(string id)
    {
        return HandleResult(await _roleService.GetByIdAsync(id));
    }

    [HttpPost]
    [Authorize(Policy = RolePermissionPermissions.Role.Create)]
    public async Task<IActionResult> Create(CreateRoleDto dto)
    {
        return HandleResult(await _roleService.CreateAsync(dto));
    }

    [HttpPut("{id}")]
    [Authorize(Policy = RolePermissionPermissions.Role.Update)]
    public async Task<IActionResult> Update(string id,UpdateRoleDto dto)
    {
        return HandleResult(await _roleService.UpdateAsync(id, dto));
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = RolePermissionPermissions.Role.Delete)]
    public async Task<IActionResult> Delete(string id)
    {
        return HandleResult(await _roleService.DeleteAsync(id));
    }

    [HttpGet("lookup")]
    public async Task<IActionResult> Lookup()
    {
        return Ok(await _roleService.GetLookupAsync());
    }
}