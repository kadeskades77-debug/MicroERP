using MicroERP.Application.Authorization.Permissions;
using MicroERP.Application.Features.Authorization.RolePermissionGroup.DTOs;
using MicroERP.Application.Features.Authorization.RolePermissionGroup.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public class RolePermissionGroupsController : BaseApiController
{
    private readonly IRolePermissionGroupService _service;

    public RolePermissionGroupsController(
        IRolePermissionGroupService service)
    {
        _service = service;
    }


    //================ GET ROLE GROUPS =================

    [HttpGet("role/{roleId}")]
    [Authorize(
        Policy = RolePermissionPermissions.Role.View)]
    public async Task<IActionResult> GetRolePermissionGroups(
        string roleId,
        CancellationToken cancellationToken)
    {
        return HandleResult(
            await _service.GetRolePermissionGroupsAsync(
                roleId,
                cancellationToken));
    }


    //================ ADD =================

    [HttpPost("add")]
    [Authorize(
        Policy = RolePermissionPermissions.Role.Update)]
    public async Task<IActionResult> Add(
        AddRolePermissionGroupsDto dto,
        CancellationToken cancellationToken)
    {
        return HandleResult(
            await _service.AddPermissionGroupsAsync(
                dto,
                cancellationToken));
    }


    //================ REPLACE =================

    [HttpPut("replace")]
    [Authorize(
        Policy = RolePermissionPermissions.Role.Update)]
    public async Task<IActionResult> Replace(
        UpdateRolePermissionGroupsDto dto,
        CancellationToken cancellationToken)
    {
        return HandleResult(
            await _service.ReplacePermissionGroupsAsync(
                dto,
                cancellationToken));
    }


    //================ REMOVE ONE =================

    [HttpDelete("remove")]
    [Authorize(
        Policy = RolePermissionPermissions.Role.Update)]
    public async Task<IActionResult> Remove(
        RemoveRolePermissionGroupDto dto,
        CancellationToken cancellationToken)
    {
        return HandleResult(
            await _service.RemovePermissionGroupAsync(
                dto,
                cancellationToken));
    }


    //================ REMOVE ALL =================

    [HttpDelete("role/{roleId}")]
    [Authorize(
        Policy = RolePermissionPermissions.Role.Delete)]
    public async Task<IActionResult> RemoveAll(
        string roleId,
        CancellationToken cancellationToken)
    {
        return HandleResult(
            await _service.RemoveAllPermissionGroupsAsync(
                roleId,
                cancellationToken));
    }
}