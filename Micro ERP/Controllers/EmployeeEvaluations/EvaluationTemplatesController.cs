using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeEvaluations;
using MicroERP.Application.Features.EmployeeEvaluations.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.EmployeeEvaluations;

[ApiController]
[Route("api/employee-evaluations/templates")]
public class EvaluationTemplatesController : ControllerBase
{
    private readonly IEvaluationTemplateService _service;

    public EvaluationTemplatesController(
        IEvaluationTemplateService service)
    {
        _service = service;
    }


    // =========================================================
    // Create
    // =========================================================

    [HttpPost]
    [Authorize(
        Policy = HRPermissions.EmployeeEvaluations.Create)]
    public async Task<ActionResult<Result<EvaluationTemplateDto>>> Create(
        CreateEvaluationTemplateDto dto,
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
    public async Task<ActionResult<Result<EvaluationTemplateDto>>> GetById(int id,
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
    public async Task<ActionResult<Result<List<EvaluationTemplateDto>>>> GetAll(
        CancellationToken ct = default)
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
    public async Task<ActionResult<Result<EvaluationTemplateDto>>> Update(
        int id,
        UpdateEvaluationTemplateDto dto,
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
}