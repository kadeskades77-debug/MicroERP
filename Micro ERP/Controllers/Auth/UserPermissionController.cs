using MicroERP.Application.Authorization.Permissions;
using MicroERP.Application.Features.Authorization.UserPermissions.DTOs;
using MicroERP.Application.Features.Authorization.UserPermissions.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Micro_ERP.Controllers.Auth;


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
    public async Task<IActionResult> Get(
      string userId,
      CancellationToken cancellationToken)
    {
        return HandleResult(
            await _service.GetUserPermissionGroupsAsync(
                userId,
                cancellationToken));
    }


    [HttpPut]
    [Authorize(Policy = RolePermissionPermissions.UserPermissionGroup.Replace)]
    public async Task<IActionResult> Replace(
        UpdateUserPermissionGroupsDto dto,
        CancellationToken cancellationToken)
    {
        return HandleResult(
            await _service.ReplacePermissionGroupsAsync(
                dto,
                cancellationToken));
    }


    [HttpPost]
    [Authorize(Policy = RolePermissionPermissions.UserPermissionGroup.AssignUsers)]
    public async Task<IActionResult> Add(
        AddUserPermissionGroupsDto dto,
        CancellationToken cancellationToken)
    {
        return HandleResult(
            await _service.AddPermissionGroupsAsync(
                dto,
                cancellationToken));
    }


    [HttpDelete("{userId}/groups/{key}")]
    [Authorize(Policy = RolePermissionPermissions.UserPermissionGroup.Delete)]
    public async Task<IActionResult> RemovePermissionGroup(
        string userId,
        string key,
        CancellationToken cancellationToken)
    {
        var result = await _service
            .RemovePermissionGroupFromUserAsync(
                new RemoveUserPermissionGroupDto
                {
                    UserId = userId,
                    PermissionGroupKey = key
                },
                cancellationToken);

        return HandleResult(result);
    }
}