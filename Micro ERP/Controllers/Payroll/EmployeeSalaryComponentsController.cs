using MicroERP.Application.Features.Payrolls.DTOs;
using MicroERP.Application.Features.Payrolls.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.Payroll;


[Route("api/[controller]")]
[ApiController]
[Authorize]
public class EmployeeSalaryComponentsController : ControllerBase
{
    private readonly IEmployeeSalaryComponentService _service;


    public EmployeeSalaryComponentsController(
        IEmployeeSalaryComponentService service)
    {
        _service = service;
    }



    [HttpPost]
    [Authorize(
        Policy = HRPermissions.Payroll.Create)]
    public async Task<IActionResult> Create(
        CreateEmployeeSalaryComponentDto dto,
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
}