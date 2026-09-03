using MicroERP.Application.Authorization.Permissions;
using MicroERP.Application.Features.Authentication.Roles.DTOs;
using MicroERP.Application.Features.Authentication.Roles.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Micro_ERP.Controllers.Auth;


[Route("api/[controller]")]
[ApiController]
public class UserRolesController : BaseApiController
{
    private readonly IUserRoleService _service;


    public UserRolesController(IUserRoleService service)
    {
        _service = service;
    }


    [HttpGet("user/{userId}")]
    [Authorize(Policy = RolePermissionPermissions.UserRole.View)]
    public async Task<IActionResult> GetUserRoles(string userId)
    {
        return HandleResult(
            await _service.GetUserRolesAsync(userId));
    }

    [HttpGet("roles")]
    [Authorize(Policy = RolePermissionPermissions.UserRole.View)]
    public async Task<IActionResult> GetRoles()
    {
        return HandleResult(
            await _service.GetAvailableRolesAsync());
    }

    [HttpPost("{userId}/assign")]
    [Authorize(Policy = RolePermissionPermissions.UserRole.AssignUsers)]
    public async Task<IActionResult> Assign(string userId,AssignUserRolesDto dto)
    {
        return HandleResult(
            await _service.AssignRolesAsync(
                userId,
                dto));
    }

    [HttpPut("replace")]
    [Authorize(Policy = RolePermissionPermissions.UserRole.Replace)]
    public async Task<IActionResult> ReplaceRoles(UpdateUserRolesDto dto)
    {
        return HandleResult(
            await _service.ReplaceRolesAsync(dto));
    }

    [HttpPost("{userId}/remove")]
    [Authorize(Policy = RolePermissionPermissions.UserRole.Delete)]
    public async Task<IActionResult> Remove(string userId,AssignUserRolesDto dto)
    {
        return HandleResult(
            await _service.RemoveRolesAsync(
                userId,
                dto));
    }
}