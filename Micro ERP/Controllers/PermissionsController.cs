using MicroERP.Application.Authorization.Permissions;
using MicroERP.Application.Features.Permissions.DTOs;
using MicroERP.Application.Features.Permissions.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers;

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
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(
            await _permissionService.GetAllAsync());
    }


    //================ GET Available Permissions =================

    [HttpGet("available")]
    [Authorize(Policy = RolePermissionPermissions.Permission.View)]
    public async Task<IActionResult> GetAvailable()
    {
        var result = await _permissionService.GetAvailablePermissionsAsync();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }


    //================ GET BY ID =================

    [HttpGet("{id:int}")]
    [Authorize(Policy = RolePermissionPermissions.Permission.View)]
    public async Task<IActionResult> GetById(int id)
    {
        return HandleResult(
            await _permissionService.GetByIdAsync(id));
    }



    //================ CREATE =================

    [HttpPost]
    [Authorize(Policy = RolePermissionPermissions.Permission.Create)]
    public async Task<IActionResult> Create(
        CreatePermissionDto dto)
    {
        return HandleResult(
            await _permissionService.CreateAsync(dto));
    }



    //================ UPDATE =================

    [HttpPut("{id:int}")]
    [Authorize(Policy = RolePermissionPermissions.Permission.Update)]
    public async Task<IActionResult> Update(
        int id,
        UpdatePermissionDto dto)
    {
        return HandleResult(
            await _permissionService.UpdateAsync(id, dto));
    }



    //================ DELETE =================

    [HttpDelete("{id:int}")]
    [Authorize(Policy = RolePermissionPermissions.Permission.Delete)]
    public async Task<IActionResult> Delete(int id)
    {
        return HandleResult(
            await _permissionService.DeleteAsync(id));
    }
}