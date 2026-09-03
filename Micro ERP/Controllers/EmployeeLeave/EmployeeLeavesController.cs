using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Features.Leaves.EmployeeLeaves.DTOs;
using MicroERP.Application.Features.Leaves.EmployeeLeaves.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.EmployeeLeave;

[Route("api")]
[ApiController]
public class EmployeeLeavesController : ControllerBase
{
    private readonly IEmployeeLeaveService _service;
    private readonly ICurrentUserService _currentUser;
    public EmployeeLeavesController(
        IEmployeeLeaveService service, ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }



    // Get all leaves for employee
    [HttpGet("employees/{employeeId}/leaves")]
    [Authorize(Policy = HRPermissions.EmployeeLeave.View)]
    public async Task<IActionResult> GetByEmployee(int employeeId,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetByEmployeeAsync(
            employeeId,
            cancellationToken);

        return Ok(result);
    }



    // Create employee leave request
    [HttpPost("employees/leaves")]
    [Authorize(Policy = HRPermissions.EmployeeLeave.Create)]
    public async Task<IActionResult> Create(CreateLeaveDto dto,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest("User not authenticated.");
        }

        var result = await _service.CreateAsync(
            userId,
            dto,
            cancellationToken);

        return Ok(result);
    }



    // Get leave by id
    [HttpGet("employee-leaves/{id}")]
    [Authorize(Policy = HRPermissions.EmployeeLeave.View)]
    public async Task<IActionResult> GetById(int id,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(
            id,
            cancellationToken);

        return Ok(result);
    }



    // Update leave request
    [HttpPut("employee-leaves/{id}")]
    [Authorize(Policy = HRPermissions.EmployeeLeave.Update)]
    public async Task<IActionResult> Update(int id,UpdateLeaveDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(
            id,
            dto,
            cancellationToken);

        return Ok(result);
    }



    // Delete leave request
    [HttpDelete("employee-leaves/{id}")]
    [Authorize(Policy = HRPermissions.EmployeeLeave.Delete)]
    public async Task<IActionResult> Delete(int id,
        CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(
            id,
            cancellationToken);

        return Ok(result);
    }



    // Approve leave request
    [HttpPost("employee-leaves/{id}/approve")]
    [Authorize(Policy = HRPermissions.EmployeeLeave.Approve)]
    public async Task<IActionResult> Approve(int id,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(
            System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;


        var result = await _service.ApproveAsync(
            id,
            userId!,
            cancellationToken);


        return Ok(result);
    }



    // Reject leave request
    [HttpPost("employee-leaves/{id}/reject")]
    [Authorize(Policy = HRPermissions.EmployeeLeave.Reject)]
    public async Task<IActionResult> Reject(int id,RejectLeaveDto dto,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(
            System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;


        var result = await _service.RejectAsync(
            id,
            userId!,
            dto,
            cancellationToken);


        return Ok(result);
    }

    // Cancel approved leave
    [HttpPost("employee-leaves/{employeeId}/{id}/cancel")]
    [Authorize(Policy = HRPermissions.EmployeeLeave.Cancel)]
    public async Task<IActionResult> Cancel(int employeeId,int id,
        CancellationToken cancellationToken)
    {
        var userId =
            User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)
            ?.Value;


        var result = await _service.CancelAsync(
            id,
            userId!,
            cancellationToken);


        return Ok(result);
    }
}