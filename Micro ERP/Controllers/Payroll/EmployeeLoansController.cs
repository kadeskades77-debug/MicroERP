using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Payrolls.EmployeeLoans.DTOs;
using MicroERP.Application.Features.Payrolls.EmployeeLoans.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Micro_ERP.Controllers.Payroll;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeeLoansController : ControllerBase
{
    private readonly IEmployeeLoanService _service;

    public EmployeeLoansController(
        IEmployeeLoanService service)
    {
        _service = service;
    }


    // =========================================================
    // Get All
    // =========================================================

    [HttpGet]
    [Authorize(Policy = HRPermissions.PayrollLoans.View)]
    public async Task<IActionResult> GetAll(
        [FromQuery] EmployeeLoanFilterDto filter,
        [FromQuery] PagedRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.GetAllAsync(
                filter,
                request,
                cancellationToken);

        return result.Success
            ? Ok(result)
            : BadRequest(result);
    }


    // =========================================================
    // Get By Id
    // =========================================================

    [HttpGet("{id:int}")]
    [Authorize(Policy = HRPermissions.PayrollLoans.View)]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.GetByIdAsync(
                id,
                cancellationToken);

        return result.Success
            ? Ok(result)
            : NotFound(result);
    }


    // =========================================================
    // Create Loan
    // =========================================================

    [HttpPost]
    [Authorize(Policy = HRPermissions.PayrollLoans.Create)]
    public async Task<IActionResult> Create(
        [FromBody] CreateEmployeeLoanDto dto,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.CreateAsync(
                dto,
                cancellationToken);

        return result.Success
            ? Ok(result)
            : BadRequest(result);
    }


    // =========================================================
    // Update Loan
    // =========================================================

    [HttpPut("{id:int}")]
    [Authorize(Policy = HRPermissions.PayrollLoans.Update)]
    public async Task<IActionResult> Update(int id,
        [FromBody] UpdateEmployeeLoanDto dto,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.UpdateAsync(
                id,
                dto,
                cancellationToken);

        return result.Success
            ? Ok(result)
            : BadRequest(result);
    }



    // =========================================================
    // Suspend Loan
    // =========================================================

    [HttpPost("{id:int}/suspend")]
    [Authorize(Policy = HRPermissions.PayrollLoans.Suspend)]
    public async Task<IActionResult> Suspend(
       int id,
       [FromBody] SuspendEmployeeLoanDto dto,
       CancellationToken cancellationToken)
    {
        var userId =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(
                Result.Failure(
                    "User ID was not found."));
        }

        var result =
            await _service.SuspendAsync(
                id,
                userId,
                dto.Reason,
                cancellationToken);

        return result.Success
            ? Ok(result)
            : BadRequest(result);
    }


    // =========================================================
    // Resume Loan
    // =========================================================

    [HttpPost("{id:int}/resume")]
    [Authorize(Policy = HRPermissions.PayrollLoans.Resume)]
    public async Task<IActionResult> Resume(
        int id,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.ResumeAsync(
                id,
                cancellationToken);

        return result.Success
            ? Ok(result)
            : BadRequest(result);
    }


    // =========================================================
    // Cancel Loan
    // =========================================================

    [HttpPost("{id:int}/cancel")]
    [Authorize(Policy = HRPermissions.PayrollLoans.Cancel)]
    public async Task<IActionResult> Cancel(
     int id,
     CancelEmployeeLoanDto dto,
     CancellationToken cancellationToken)
    {
        var userId =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var result =
            await _service.CancelAsync(
                id,
                userId,
                dto.Reason,
                cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }


 
}