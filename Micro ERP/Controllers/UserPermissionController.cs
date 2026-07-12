using MicroERP.Application.Authorization.Permissions;
using MicroERP.Application.Features.UserPermissions.DTOs;
using MicroERP.Application.Features.UserPermissions.Interfaces;
using MicroERP.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Micro_ERP.Controllers;


[Route("api/user-permissions")]
[ApiController]
public class UserPermissionController : BaseApiController
{
    private readonly IUserPermissionAssignmentService _service;


    public UserPermissionController(
        IUserPermissionAssignmentService service)
    {
        _service = service;
    }



    [HttpGet("{userId}")]
    [Authorize(Policy = RolePermissionPermissions.UserPermissionGroup.View)]
    public async Task<IActionResult> Get(string userId)
    {
        return HandleResult(
            await _service.GetUserPermissionGroupsAsync(userId));
    }



    [HttpPut]
    [Authorize(Policy = RolePermissionPermissions.UserPermissionGroup.Replace)]
    public async Task<IActionResult> Replace(
        UpdateUserPermissionGroupsDto dto)
    {
        return HandleResult(
            await _service.ReplacePermissionGroupsAsync(dto));
    }

    [HttpPost("{userId}/groups/{key}")]
    [Authorize(Policy = RolePermissionPermissions.UserPermissionGroup.AssignUsers)]
    public async Task<IActionResult> AssignUsers(string userId,string key)
    {
        return HandleResult(
            await _service.AddPermissionGroupAsync(userId,
                key));
    }



    [HttpDelete("{userId}/groups/{key}")]
    [Authorize(Policy = RolePermissionPermissions.UserPermissionGroup.Delete)]
    public async Task<IActionResult> RemovePermissionGroup(
    string userId,
    string key)
    {
        var result = await _service
            .RemovePermissionGroupFromUserAsync(
                new RemoveUserPermissionGroupDto
                {
                    UserId = userId,
                    PermissionGroupKey = key
                });

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}