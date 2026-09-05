using MicroERP.Application.Features.Positions.DTOs;
using MicroERP.Application.Features.Positions.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.EmployeeControllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PositionController : BaseApiController
{
    private readonly IPositionService _positionService;

    public PositionController(
        IPositionService positionService)
    {
        _positionService = positionService;
    }

    // =========================================================
    // Get All
    // =========================================================

    [HttpGet]
    [Authorize(Policy = HRPermissions.Employee.View)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result =
            await _positionService.GetAllAsync(
                cancellationToken);

        return HandleResult(result);
    }

    // =========================================================
    // Get By Id
    // =========================================================

    [HttpGet("{id:int}")]
    [Authorize(Policy = HRPermissions.Employee.View)]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result =
            await _positionService.GetByIdAsync(
                id,
                cancellationToken);

        return HandleResult(result);
    }

    // =========================================================
    // Create
    // =========================================================

    [HttpPost]
    [Authorize(Policy = HRPermissions.Employee.Create)]
    public async Task<IActionResult> Create(
        CreatePositionDto dto,
        CancellationToken cancellationToken)
    {
        var result =
            await _positionService.CreateAsync(
                dto,
                cancellationToken);

        if (!result.Success)
            return HandleResult(result);

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = result.Data!.Id
            },
            result);
    }

    // =========================================================
    // Update
    // =========================================================

    [HttpPatch("{id:int}")]
    [Authorize(Policy = HRPermissions.Employee.Update)]
    public async Task<IActionResult> Update(
        int id,
        UpdatePositionDto dto,
        CancellationToken cancellationToken)
    {
        var result =
            await _positionService.UpdateAsync(
                id,
                dto,
                cancellationToken);

        return HandleResult(result);
    }

    // =========================================================
    // Delete
    // =========================================================

    [HttpDelete("{id:int}")]
    [Authorize(Policy = HRPermissions.Employee.Delete)]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var result =
            await _positionService.DeleteAsync(
                id,
                cancellationToken);

        return HandleResult(result);
    }

    // =========================================================
    // Restore
    // =========================================================

    [HttpPatch("{id:int}/restore")]
    [Authorize(Policy = HRPermissions.Employee.Delete)]
    public async Task<IActionResult> Restore(
        int id,
        CancellationToken cancellationToken)
    {
        var result =
            await _positionService.RestoreAsync(
                id,
                cancellationToken);

        return HandleResult(result);
    }

    [HttpPut("{positionId:int}/employees/{employeeId:int}")]
    [Authorize(Policy = HRPermissions.Employee.Update)]
    public async Task<IActionResult> AssignEmployee(
    int positionId,
    int employeeId,
    CancellationToken cancellationToken)
    {
        var result =
            await _positionService.AssignEmployeeAsync(
                positionId,
                employeeId,
                cancellationToken);

        return HandleResult(result);
    }


    [HttpDelete("{positionId:int}/employees/{employeeId:int}")]
    [Authorize(Policy = HRPermissions.Employee.Update)]
    public async Task<IActionResult> RemoveEmployee(
    int positionId,
    int employeeId,
    CancellationToken cancellationToken)
    {
        var result =
            await _positionService.RemoveEmployeeAsync(
                positionId,
                employeeId,
                cancellationToken);

        return HandleResult(result);
    }


    [HttpGet("{positionId:int}/employees")]
    [Authorize(Policy = HRPermissions.Employee.View)]
    public async Task<IActionResult> GetEmployees(
    int positionId,
    CancellationToken cancellationToken)
    {
        var result =
            await _positionService.GetEmployeesAsync(
                positionId,
                cancellationToken);

        return HandleResult(result);
    }

    // =========================================================
    // Lookup
    // =========================================================

    [HttpGet("lookup")]
    [Authorize(Policy = HRPermissions.Employee.View)]
    public async Task<IActionResult> Lookup(
        CancellationToken cancellationToken)
    {
        var result =
            await _positionService.GetLookupAsync(
                cancellationToken);

        return HandleResult(result);
    }
}