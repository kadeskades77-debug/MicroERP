
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

    public PermissionGroupsController(
        IPermissionGroupService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Policy = RolePermissionPermissions.PermissionGroup.View)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        return HandleResult(
            await _service.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = RolePermissionPermissions.PermissionGroup.View)]
    public async Task<IActionResult> GetById(int id,
        CancellationToken cancellationToken)
    {
        return HandleResult(
            await _service.GetByIdAsync(
                id,
                cancellationToken));
    }

    [HttpPost]
    [Authorize(Policy = RolePermissionPermissions.PermissionGroup.Create)]
    public async Task<IActionResult> Create(
        CreatePermissionGroupDto dto,
        CancellationToken cancellationToken)
    {
        return HandleResult(
            await _service.CreateAsync(
                dto,
                cancellationToken));
    }

    [HttpPost("{id}/permissions")]
    [Authorize(Policy = RolePermissionPermissions.PermissionGroup.Update)]
    public async Task<IActionResult> AddPermissions(int id,
        AddPermissionsToGroupDto dto,
        CancellationToken cancellationToken)
    {
        return HandleResult(
            await _service.AddPermissionsToGroupAsync(
                id,
                dto,
                cancellationToken));
    }

    [HttpDelete("{id}/permissions")]
    [Authorize(Policy = RolePermissionPermissions.PermissionGroup.Update)]
    public async Task<IActionResult> RemovePermissions(int id, RemovePermissionsFromGroupDto dto, 
        CancellationToken cancellationToken)
    {
        var result =
            await _service.RemovePermissionsFromGroupAsync(
                id,
                dto, cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{id}/allpermissions")]
    [Authorize(Policy = RolePermissionPermissions.PermissionGroup.Update)]
    public async Task<IActionResult> RemoveAllPermissions(int id,
        CancellationToken cancellationToken)
    {
        return HandleResult(
            await _service.RemoveAllPermissionsFromGroupAsync(
                id,
                cancellationToken));
    }

    [HttpPost("move-permissions")]
    [Authorize(Policy = RolePermissionPermissions.PermissionGroup.Update)]
    public async Task<IActionResult> MovePermissions(
        MovePermissionsBetweenGroupsDto dto,
        CancellationToken cancellationToken)
    {
        return HandleResult(
            await _service.MovePermissionsBetweenGroupsAsync(
                dto,
                cancellationToken));
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = RolePermissionPermissions.PermissionGroup.Update)]
    public async Task<IActionResult> Update(int id,
        UpdatePermissionGroupDto dto,
        CancellationToken cancellationToken)
    {
        return HandleResult(
            await _service.UpdateAsync(
                id,
                dto,
                cancellationToken));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = RolePermissionPermissions.PermissionGroup.Delete)]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        return HandleResult(
            await _service.DeleteAsync(
                id,
                cancellationToken));
    }

    [HttpGet("lookup")]
    public async Task<IActionResult> Lookup(
        CancellationToken cancellationToken)
    {
        return Ok(
            await _service.GetLookupAsync(
                cancellationToken));
    }
}