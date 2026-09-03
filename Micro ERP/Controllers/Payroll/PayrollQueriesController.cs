using MicroERP.Application.Features.Payrolls.DTOs;
using MicroERP.Application.Features.Payrolls.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.Payroll;


[Route("api/payroll-queries")]
[ApiController]
[Authorize]
public class PayrollQueriesController : ControllerBase
{
    private readonly IPayrollQueries _payrollQueries;


    public PayrollQueriesController(
        IPayrollQueries payrollQueries)
    {
        _payrollQueries = payrollQueries;
    }



    // Get all payroll periods
    [HttpGet("periods")]
    [Authorize(
        Policy = HRPermissions.Payroll.View)]
    public async Task<IActionResult> GetPeriods(
        CancellationToken cancellationToken)
    {
        var result =
            await _payrollQueries.GetPeriodsAsync(
                cancellationToken);


        if (!result.Success)
            return BadRequest(result);


        return Ok(result);
    }



    // Get payrolls by period
    [HttpGet("period/{periodId:int}")]
    [Authorize(
        Policy = HRPermissions.Payroll.View)]
    public async Task<IActionResult> GetByPeriod(int periodId,
        CancellationToken cancellationToken)
    {
        var result =
            await _payrollQueries.GetByPeriodAsync(
                periodId,
                cancellationToken);


        if (!result.Success)
            return BadRequest(result);


        return Ok(result);
    }



    // Get payroll details
    [HttpGet("{payrollId:int}")]
    [Authorize(Policy = HRPermissions.Payroll.View)]
    public async Task<IActionResult> GetDetails(int payrollId,
        CancellationToken cancellationToken)
    {
        var result =
            await _payrollQueries.GetDetailsAsync(
                payrollId,
                cancellationToken);


        if (!result.Success)
            return BadRequest(result);


        return Ok(result);
    }



    [HttpGet("employee/{employeeId:int}")]
    [Authorize(Policy = HRPermissions.Payroll.View)]
    public async Task<IActionResult> GetByEmployee(
    int employeeId,
    CancellationToken cancellationToken)
    {
        var result =
            await _payrollQueries.GetByEmployeeAsync(
                employeeId,
                cancellationToken);


        return Ok(result);
    }


    [HttpGet("paid")]
    [Authorize(Policy = HRPermissions.Payroll.View)]
    public async Task<IActionResult> GetPaid(
    CancellationToken cancellationToken)
    {
        var result =
            await _payrollQueries.GetPaidPayrollsAsync(
                cancellationToken);


        return Ok(result);
    }


    [HttpGet("{id:int}/payslip")]
    [Authorize(Policy = HRPermissions.Payroll.View)]
    public async Task<IActionResult> GetPayslip(int id,
    CancellationToken cancellationToken)
    {
        var result =
            await _payrollQueries.GetPayslipAsync(
                id,
                cancellationToken);

        return Ok(result);
    }


    [HttpGet]
    [Authorize(Policy = HRPermissions.Payroll.View)]
    public async Task<IActionResult> GetPaged(
    [FromQuery] PayrollFilterDto filter,
    CancellationToken cancellationToken)
    {
        var result =
            await _payrollQueries.GetPagedAsync(
                filter,
                cancellationToken);


        return Ok(result);
    }
}