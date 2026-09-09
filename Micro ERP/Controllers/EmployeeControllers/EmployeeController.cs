using Micro_ERP.Controllers;
using MicroERP.Application.Features.Employees.DTOs;
using MicroERP.Application.Features.Employees.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Micro_ERP.Controllers.EmployeeControllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeeController : BaseApiController
{
    private readonly IEmployeeService _employeeService;

    public EmployeeController(
        IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }


    // =========================================================
    // Get All
    // =========================================================

    [HttpGet]
    [Authorize(Policy = HRPermissions.Employee.View)]
    public async Task<IActionResult> GetAll(
        [FromQuery] EmployeeFilterDto? filter = null)
    {
        var result =
            await _employeeService.GetAllAsync(filter);

        return HandleResult(result);
    }


    // =========================================================
    // Get Deleted
    // =========================================================

    [HttpGet("deleted")]
    [Authorize(Policy = HRPermissions.Employee.View)]
    public async Task<IActionResult> GetDeleted()
    {
        var result =
            await _employeeService.GetDeletedAsync();

        return HandleResult(result);
    }

    // =========================================================
    // Get By Id
    // =========================================================

    [HttpGet("{id:int}")]
    [Authorize(Policy = HRPermissions.Employee.View)]
    public async Task<IActionResult> GetById(int id,
        CancellationToken cancellationToken)
    {
        var result =
            await _employeeService.GetByIdAsync(
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
        CreateEmployeeDto dto,
        CancellationToken cancellationToken)
    {
        var result =
            await _employeeService.CreateAsync(
                dto,
                cancellationToken);

        if (!result.Success)
            return HandleResult(result);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Data!.EmployeeId },
            result);
    }


    // =========================================================
    // Transfer Department
    // =========================================================

    [HttpPut("{id:int}/transfer")]
    [Authorize(Policy = HRPermissions.EmployeeDepartment.Transfer)]
    public async Task<IActionResult> Transfer(int id,
        TransferEmployeeDto dto,
        CancellationToken cancellationToken)
    {
        var result =
            await _employeeService.TransferEmployeeAsync(
                id,
                dto,
                cancellationToken);

        return HandleResult(result);
    }


    // =========================================================
    // Update Salary
    // =========================================================

    [HttpPut("{id:int}/salary")]
    [Authorize(Policy = HRPermissions.EmployeeSalary.Update)]
    public async Task<IActionResult> UpdateSalary(int id,
        UpdateEmployeeSalaryDto dto,
        CancellationToken cancellationToken)
    {
        var result =
            await _employeeService.UpdateSalaryAsync(
                id,
                dto,
                cancellationToken);

        return HandleResult(result);
    }


    // =========================================================
    // Update Employee
    // =========================================================

    [HttpPatch("{id:int}")]
    [Authorize(Policy = HRPermissions.Employee.Update)]
    public async Task<IActionResult> Update(int id,
        UpdateEmployeeDto dto,
        CancellationToken cancellationToken)
    {
        var result =
            await _employeeService.UpdateAsync(
                id,
                dto,
                cancellationToken);

        return HandleResult(result);
    }


    // =========================================================
    // Delete Employee
    // =========================================================

    [HttpDelete("{id:int}")]
    [Authorize(Policy = HRPermissions.Employee.Delete)]
    public async Task<IActionResult> Delete(int id,
        CancellationToken cancellationToken)
    {
        var result =
            await _employeeService.DeleteAsync(
                id,
                cancellationToken);

        return HandleResult(result);
    }


    // =========================================================
    // Restore Employee
    // =========================================================

    [HttpPatch("{id:int}/restore")]
    [Authorize(Policy = HRPermissions.Employee.Delete)]
    public async Task<IActionResult> Restore(int id,
        CancellationToken cancellationToken)
    {
        var result =
            await _employeeService.RestoreAsync(
                id,
                cancellationToken);

        return HandleResult(result);
    }




    // =========================================================
    // ChangeStatus Employee
    // =========================================================


    [HttpPatch("{id:int}/status")]
    [Authorize(Policy = HRPermissions.Employee.Update)]
    public async Task<IActionResult> ChangeStatus(
    int id,
    ChangeEmployeeStatusDto dto,
    CancellationToken cancellationToken)
    {
        var result =
            await _employeeService.ChangeStatusAsync(
                id,
                dto,
                cancellationToken);

        return HandleResult(result);
    }



    // =========================================================
    // Assign Work Schedule To Employee
    // =========================================================

    [HttpPut("{id:int}/work-schedule")]
    [Authorize(Policy = HRPermissions.Employee.Update)]
    public async Task<IActionResult> AssignWorkSchedule(
        int id,
        int workScheduleId,
        CancellationToken cancellationToken)
    {
        var result =
            await _employeeService
                .AssignWorkScheduleToEmployeeAsync(
                    id,
                    workScheduleId,
                    cancellationToken);

        return HandleResult(result);
    }


    // =========================================================
    // Assign Work Schedule To Department
    // =========================================================

    [HttpPut("department/{departmentId:int}/work-schedule")]
    [Authorize(Policy = HRPermissions.Employee.Update)]
    public async Task<IActionResult> AssignWorkScheduleToDepartment(
        int departmentId,
        int workScheduleId,
        CancellationToken cancellationToken)
    {
        var result =
            await _employeeService
                .AssignWorkScheduleToDepartmentAsync(
                    departmentId,
                    workScheduleId,
                    cancellationToken);

        return HandleResult(result);
    }


    // =========================================================
    // Employee Lookup
    // =========================================================

    [HttpGet("lookup")]
    [Authorize(Policy = HRPermissions.Employee.View)]
    public async Task<IActionResult> Lookup(
        CancellationToken cancellationToken)
    {
        var result =
            await _employeeService.GetLookupAsync(
                cancellationToken);

        return HandleResult(result);
    }
}

