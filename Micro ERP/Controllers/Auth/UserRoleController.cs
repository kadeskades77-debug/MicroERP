using MicroERP.Application.Authorization.Permissions;
using MicroERP.Application.Features.Authorization.Roles.DTOs;
using MicroERP.Application.Features.Authorization.Roles.Interfaces;
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

    // ==================== USER ROLES ====================

    [HttpGet("user/{userId}")]
    [Authorize(Policy = RolePermissionPermissions.UserRole.View)]
    public async Task<IActionResult> GetUserRoles(string userId,
        CancellationToken cancellationToken)
    {
        return HandleResult(
            await _service.GetUserRolesAsync(
                userId,
                cancellationToken));
    }

    // ==================== AVAILABLE ROLES ====================

    [HttpGet("roles")]
    [Authorize(Policy = RolePermissionPermissions.UserRole.View)]
    public async Task<IActionResult> GetRoles(
        CancellationToken cancellationToken)
    {
        return HandleResult(
            await _service.GetAvailableRolesAsync(
                cancellationToken));
    }

    // ==================== ASSIGN ROLES ====================

    [HttpPost("{userId}/assign")]
    [Authorize(Policy = RolePermissionPermissions.UserRole.AssignUsers)]
    public async Task<IActionResult> Assign(string userId,
        AssignUserRolesDto dto,
        CancellationToken cancellationToken)
    {
        return HandleResult(
            await _service.AssignRolesAsync(
                userId,
                dto,
                cancellationToken));
    }

    // ==================== REPLACE ROLES ====================

    [HttpPut("replace")]
    [Authorize(Policy = RolePermissionPermissions.UserRole.Replace)]
    public async Task<IActionResult> ReplaceRoles(
        UpdateUserRolesDto dto,
        CancellationToken cancellationToken)
    {
        return HandleResult(
            await _service.ReplaceRolesAsync(
                dto,
                cancellationToken));
    }

    // ==================== REMOVE ROLES ====================

    [HttpPost("{userId}/remove")]
    [Authorize(Policy = RolePermissionPermissions.UserRole.Delete)]
    public async Task<IActionResult> Remove(string userId,
        AssignUserRolesDto dto,
        CancellationToken cancellationToken)
    {
        return HandleResult(
            await _service.RemoveRolesAsync(
                userId,
                dto,
                cancellationToken));
    }
}