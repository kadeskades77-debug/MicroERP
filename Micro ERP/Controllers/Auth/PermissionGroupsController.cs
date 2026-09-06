
using MicroERP.Application.Authorization.Permissions;
using MicroERP.Application.Features.Authorization.PermissionGroups.DTOs;
using MicroERP.Application.Features.Authorization.PermissionGroups.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public class PermissionGroupsController : BaseApiController
{
    private readonly IPermissionGroupService _service;

    public PermissionGroupsController(IPermissionGroupService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Policy = RolePermissionPermissions.PermissionGroup.View)]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await _service.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = RolePermissionPermissions.PermissionGroup.View)]
    public async Task<IActionResult> GetById(int id)
    {
        return HandleResult(await _service.GetByIdAsync(id));
    }

    [HttpPost]
    [Authorize(Policy = RolePermissionPermissions.PermissionGroup.Create)]
    public async Task<IActionResult> Create(CreatePermissionGroupDto dto)
    {
        return HandleResult(await _service.CreateAsync(dto));
    }

    [HttpPost("{id}/permissions")]
    [Authorize(Policy = RolePermissionPermissions.PermissionGroup.Update)]
    public async Task<IActionResult> AddPermissions(int id,AddPermissionsToGroupDto dto)
    {
        var result =
            await _service.AddPermissionsToGroupAsync(
                id,
                dto);

        return Ok(result);
    }

    [HttpDelete("{id}/permissions")]
    [Authorize(Policy = RolePermissionPermissions.PermissionGroup.Update)]
    public async Task<IActionResult> RemovePermissions(int id, RemovePermissionsFromGroupDto dto)
    {
        var result =
            await _service.RemovePermissionsFromGroupAsync(
                id,
                dto);

        return Ok(result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = RolePermissionPermissions.PermissionGroup.Update)]
    public async Task<IActionResult> Update(int id, UpdatePermissionGroupDto dto)
    {
        return HandleResult(await _service.UpdateAsync(id, dto));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = RolePermissionPermissions.PermissionGroup.Delete)]
    public async Task<IActionResult> Delete(int id)
    {
        return HandleResult(await _service.DeleteAsync(id));
    }

    [HttpGet("lookup")]
    public async Task<IActionResult> Lookup()
    {
        return Ok(await _service.GetLookupAsync());
    }
}