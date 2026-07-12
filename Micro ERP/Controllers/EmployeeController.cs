using Micro_ERP.Controllers;
using MicroERP.Application.Authorization.Permissions;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Employees.DTOs;
using MicroERP.Application.Features.Employees.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroERP.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EmployeeController : BaseApiController
{
    private readonly IEmployeeService _employeeService;

    public EmployeeController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    [Authorize(Policy = HRPermissions.Employee.View)]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await _employeeService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = HRPermissions.Employee.View)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _employeeService.GetByIdAsync(id);

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = HRPermissions.Employee.Create)]
    public async Task<IActionResult> Create(CreateEmployeeDto dto)
    {
        var result = await _employeeService.CreateAsync(dto);

        return CreatedAtAction(
             nameof(GetById),
             new { id = result.EmployeeId },
             result);
    }

    [HttpPut("{id}/transfer")]
    [Authorize(Policy = HRPermissions.Employee.TransferDepartment)]
    public async Task<IActionResult> Transfer(int id,TransferEmployeeDto dto)
    {
        var result = await _employeeService
            .TransferEmployeeAsync(id, dto);

        return result.Success
            ? Ok(result)
            : BadRequest(result);
    }

    [HttpPut("{id}/salary")]
    [Authorize(Policy = HRPermissions.Employee.UpdateSalary)]
    public async Task<IActionResult> UpdateSalary(int id,UpdateEmployeeSalaryDto dto)
    {
        var result = await _employeeService
            .UpdateSalaryAsync(id, dto);

        return result.Success
            ? Ok(result)
            : BadRequest(result);
    }

    [HttpPatch("{id:int}")]
    [Authorize(Policy = HRPermissions.Employee.Update)]
    public async Task<IActionResult> Update(int id,UpdateEmployeeDto dto)
    {
        var result = await _employeeService.UpdateAsync(id, dto);

        return result.Success
            ? Ok(result)
            : BadRequest(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = HRPermissions.Employee.Delete)]
    public async Task<IActionResult> Delete(int id)
    {
        await _employeeService.DeleteAsync(id);

        return Ok(new Result
        {
            Success = true,
            Message = "Employee Deleted successfully."
        });
    }

    [HttpPatch("{id}/restore")]
    [Authorize(Policy = HRPermissions.Employee.Delete)]
    public async Task<IActionResult> Restore(int id)
    {
        await _employeeService.RestoreAsync(id);

        return Ok(new Result
        {
            Success = true,
            Message = "Employee restored successfully."
        });
    }

    [HttpPatch("{id:int}/activate")]
    [Authorize(Policy = HRPermissions.Employee.Update)]
    public async Task<IActionResult> Activate(int id)
    {
        await _employeeService.ActivateAsync(id);

        return Ok(new Result
        {
            Success = true,
            Message = "Employee Activated successfully."
        });
    }

    [HttpPatch("{id:int}/deactivate")]
    [Authorize(Policy = HRPermissions.Employee.Update)]
    public async Task<IActionResult> Deactivate(int id)
    {
        await _employeeService.DeactivateAsync(id);

        return Ok(new Result
        {
            Success = true,
            Message = "Employee Deactivated successfully."
        });
    }
}