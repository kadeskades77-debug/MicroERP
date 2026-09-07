using MicroERP.Application.Authorization.Permissions;
using MicroERP.Application.Features.Authorization.Permissions.DTOs;
using MicroERP.Application.Features.Authorization.Permissions.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public class PermissionsController : BaseApiController
{
    private readonly IPermissionService _permissionService;

    public PermissionsController(
        IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }


    //================ GET ALL =================

    [HttpGet]
    [Authorize(Policy = RolePermissionPermissions.Permission.View)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        return HandleResult(
            await _permissionService.GetAllAsync(
                cancellationToken));
    }


    //================ GET AVAILABLE PERMISSIONS =================

    [HttpGet("available/{groupId:int}")]
    [Authorize(Policy = RolePermissionPermissions.Permission.View)]
    public async Task<IActionResult> GetAvailable(
     int groupId,
     CancellationToken cancellationToken)
    {
        return HandleResult(
            await _permissionService
                .GetAvailablePermissionsAsync(
                    groupId,
                    cancellationToken));
    }


    //================ GET BY ID =================

    [HttpGet("{id:int}")]
    [Authorize(Policy = RolePermissionPermissions.Permission.View)]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        return HandleResult(
            await _permissionService.GetByIdAsync(
                id,
                cancellationToken));
    }


    //================ UPDATE =================

    [HttpPut("{id:int}")]
    [Authorize(Policy = RolePermissionPermissions.Permission.Update)]
    public async Task<IActionResult> Update(int id,
        UpdatePermissionDto dto,
        CancellationToken cancellationToken)
    {
        return HandleResult(
            await _permissionService.UpdateAsync(
                id,
                dto,
                cancellationToken));
    }


    //================ LOOKUP =================

    [HttpGet("lookup")]
    [Authorize(Policy = RolePermissionPermissions.Permission.View)]
    public async Task<IActionResult> Lookup(
        CancellationToken cancellationToken)
    {
        return Ok(
            await _permissionService.GetLookupAsync(
                cancellationToken));
    }
}