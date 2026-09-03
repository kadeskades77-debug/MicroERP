using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Payrolls.Adjustments.DTOs;
using MicroERP.Application.Features.Payrolls.Adjustments.Interfaces;
using MicroERP.Domin.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.Payroll;


[Route("api/payroll-adjustments")]
[ApiController]
[Authorize]
public class PayrollAdjustmentsController : ControllerBase
{
    private readonly IPayrollAdjustmentService _service;

    private readonly IPayrollAdjustmentQueries _queries;


    public PayrollAdjustmentsController(
        IPayrollAdjustmentService service,
        IPayrollAdjustmentQueries queries)
    {
        _service = service;
        _queries = queries;
    }



    [HttpPost]
    [Authorize(
        Policy = HRPermissions.Payroll.Create)]
    public async Task<IActionResult> Create(
        CreatePayrollAdjustmentDto dto,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.CreateAsync(
                dto,
                cancellationToken);


        if (!result.Success)
            return BadRequest(result);


        return Ok(result);
    }

    // =========================================================
    // Create Adjustment - All Employees
    // =========================================================

    [HttpPost("all-employees")]
    [Authorize(
       Policy = HRPermissions.Payroll.Create)]
    public async Task<IActionResult> CreateForAllEmployees(
       [FromBody] CreatePayrollAdjustmentForAllEmployeesDto dto,
       CancellationToken cancellationToken)
    {
        var result =
            await _service.CreateForAllEmployeesAsync(
                dto,
                cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }


    [HttpPut("{id:int}")]
    [Authorize(
        Policy = HRPermissions.Payroll.Update)]
    public async Task<IActionResult> Update(int id,
        UpdatePayrollAdjustmentDto dto,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.UpdateAsync(
                id,
                dto,
                cancellationToken);


        if (!result.Success)
            return BadRequest(result);


        return Ok(result);
    }





    [HttpDelete("{id:int}")]
    [Authorize(
        Policy = HRPermissions.Payroll.Delete)]
    public async Task<IActionResult> Delete(int id,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.DeleteAsync(
                id,
                cancellationToken);


        if (!result.Success)
            return BadRequest(result);


        return Ok(result);
    }





    [HttpGet]
    [Authorize(
        Policy = HRPermissions.Payroll.View)]
    public async Task<IActionResult> GetAll(
        [FromQuery] PayrollAdjustmentFilterDto filter,
        [FromQuery] PagedRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetAllAsync(
                filter,
                request,
                cancellationToken);


        return Ok(result);
    }





    [HttpGet("{id:int}")]
    [Authorize(
        Policy = HRPermissions.Payroll.View)]
    public async Task<IActionResult> GetById(int id,
        CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetByIdAsync(
                id,
                cancellationToken);


        if (!result.Success)
            return NotFound(result);


        return Ok(result);
    }
}