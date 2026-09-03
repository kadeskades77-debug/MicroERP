using MicroERP.Application.Features.Payrolls.SalaryComponents.DTOs;
using MicroERP.Application.Features.Payrolls.SalaryComponents.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.Payroll;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SalaryComponentsController : ControllerBase
{
    private readonly ISalaryComponentService _service;
    private readonly ISalaryComponentQueries _queries;

    public SalaryComponentsController(
        ISalaryComponentService service,
        ISalaryComponentQueries queries)
    {
        _service = service;
        _queries = queries;
    }

    [HttpGet]
    [Authorize(Policy = HRPermissions.Payroll.View)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _queries.GetAllAsync(cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = HRPermissions.Payroll.View)]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _queries.GetByIdAsync(
            id,
            cancellationToken);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = HRPermissions.Payroll.Create)]
    public async Task<IActionResult> Create(
        CreateSalaryComponentDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(
            dto,
            cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = HRPermissions.Payroll.Create)]
    public async Task<IActionResult> Update(int id,
        UpdateSalaryComponentDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(
            id,
            dto,
            cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = HRPermissions.Payroll.Create)]
    public async Task<IActionResult> Delete(int id,
        CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(
            id,
            cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}