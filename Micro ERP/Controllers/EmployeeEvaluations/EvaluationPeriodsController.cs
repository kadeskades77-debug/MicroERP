using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeEvaluations;
using MicroERP.Application.Features.EmployeeEvaluations.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.EmployeeEvaluations;

[ApiController]
[Authorize]
[Route("api/employee-evaluations/periods")]
public class EvaluationPeriodsController : ControllerBase
{
    private readonly IEvaluationPeriodService _service;

    public EvaluationPeriodsController(
        IEvaluationPeriodService service)
    {
        _service = service;
    }


    // =========================================================
    // Create
    // =========================================================

    [HttpPost]
    [Authorize(
        Policy = HRPermissions.EmployeeEvaluations.Create)]
    public async Task<ActionResult<Result<EvaluationPeriodDto>>> Create(
        CreateEvaluationPeriodDto dto,
        CancellationToken ct)
    {
        var result =
            await _service.CreateAsync(
                dto,
                ct);

        return Ok(result);
    }


    // =========================================================
    // Get By Id
    // =========================================================

    [HttpGet("{id:int}")]
    [Authorize(
        Policy = HRPermissions.EmployeeEvaluations.View)]
    public async Task<ActionResult<Result<EvaluationPeriodDto>>> GetById(
        int id,
        CancellationToken ct)
    {
        var result =
            await _service.GetByIdAsync(
                id,
                ct);

        return Ok(result);
    }


    // =========================================================
    // Get All
    // =========================================================

    [HttpGet]
    [Authorize(
        Policy = HRPermissions.EmployeeEvaluations.View)]
    public async Task<ActionResult<Result<List<EvaluationPeriodDto>>>> GetAll(
        CancellationToken ct)
    {
        var result =
            await _service.GetAllAsync(
                ct);

        return Ok(result);
    }


    // =========================================================
    // Update
    // =========================================================

    [HttpPut("{id:int}")]
    [Authorize(
        Policy = HRPermissions.EmployeeEvaluations.Update)]
    public async Task<ActionResult<Result<EvaluationPeriodDto>>> Update(
        int id,
        UpdateEvaluationPeriodDto dto,
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


    // =========================================================
    // Open
    // =========================================================

    [HttpPost("{id:int}/open")]
    [Authorize(
        Policy = HRPermissions.EmployeeEvaluations.Update)]
    public async Task<ActionResult<Result<bool>>> Open(
        int id,
        CancellationToken ct)
    {
        var result =
            await _service.OpenAsync(
                id,
                ct);

        return Ok(result);
    }


    // =========================================================
    // Close
    // =========================================================

    [HttpPost("{id:int}/close")]
    [Authorize(
        Policy = HRPermissions.EmployeeEvaluations.Update)]
    public async Task<ActionResult<Result<bool>>> Close(
        int id,
        CancellationToken ct)
    {
        var result =
            await _service.CloseAsync(
                id,
                ct);

        return Ok(result);
    }
}