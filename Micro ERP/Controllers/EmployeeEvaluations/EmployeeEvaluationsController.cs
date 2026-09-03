using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeEvaluations;
using MicroERP.Application.Features.EmployeeEvaluations.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.EmployeeEvaluations;

[ApiController]
[Route("api/employee-evaluations")]
public class EmployeeEvaluationsController : ControllerBase
{
    private readonly IEmployeeEvaluationService _service;

    public EmployeeEvaluationsController(
        IEmployeeEvaluationService service)
    {
        _service = service;
    }


    // =========================================================
    // Create
    // =========================================================

    [HttpPost]
    [Authorize(
        Policy = HRPermissions.EmployeeEvaluations.Create)]
    public async Task<ActionResult<Result<EmployeeEvaluationDto>>> Create(
        CreateEmployeeEvaluationDto dto,
        CancellationToken ct)
    {
        var result =
            await _service.CreateAsync(
                dto,
                ct);

        return Ok(result);
    }


    // =========================================================
    // Update
    // =========================================================

    [HttpPut("{id:int}")]
    [Authorize(
        Policy = HRPermissions.EmployeeEvaluations.Update)]
    public async Task<ActionResult<Result<EmployeeEvaluationDto>>> Update(
        int id,
        UpdateEmployeeEvaluationDto dto,
        CancellationToken ct)
    {
        var result =
            await _service.UpdateAsync(
                id,
                dto,
                ct);

        return Ok(result);
    }


    // =========================================================
    // Submit
    // =========================================================

    [HttpPost("{id:int}/submit")]
    [Authorize(
        Policy = HRPermissions.EmployeeEvaluations.Submit)]
    public async Task<ActionResult<Result<bool>>> Submit(
        int id,
        CancellationToken ct)
    {
        var result =
            await _service.SubmitAsync(
                id,
                ct);

        return Ok(result);
    }


    // =========================================================
    // Approve
    // =========================================================

    [HttpPost("{id:int}/approve")]
    [Authorize(
        Policy = HRPermissions.EmployeeEvaluations.Approve)]
    public async Task<ActionResult<Result<bool>>> Approve(
        int id,
        CancellationToken ct)
    {
        var result =
            await _service.ApproveAsync(
                id,
                ct);

        return Ok(result);
    }


    // =========================================================
    // Reject
    // =========================================================

    [HttpPost("{id:int}/reject")]
    [Authorize(
        Policy = HRPermissions.EmployeeEvaluations.Reject)]
    public async Task<ActionResult<Result<bool>>> Reject(
        int id,
        [FromQuery] string? reason,
        CancellationToken ct)
    {
        var result =
            await _service.RejectAsync(
                id,
                reason,
                ct);

        return Ok(result);
    }


    // =========================================================
    // Delete
    // =========================================================

    [HttpDelete("{id:int}")]
    [Authorize(
        Policy = HRPermissions.EmployeeEvaluations.Delete)]
    public async Task<ActionResult<Result<bool>>> Delete(
        int id,
        CancellationToken ct)
    {
        var result =
            await _service.DeleteAsync(
                id,
                ct);

        return Ok(result);
    }
}