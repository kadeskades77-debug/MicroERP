using MicroERP.Application.Features.Payrolls.DTOs;
using MicroERP.Application.Features.Payrolls.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Micro_ERP.Controllers.Payroll;


[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PayrollController : ControllerBase
{
    private readonly IPayrollService _payrollService;


    public PayrollController(
        IPayrollService payrollService)
    {
        _payrollService = payrollService;
    }

    private string? GetCurrentUserId()
    {
        return User.FindFirstValue(
            ClaimTypes.NameIdentifier);
    }

    [HttpPost("period")]
    [Authorize(
        Policy = HRPermissions.Payroll.Create)]
    public async Task<IActionResult> CreatePeriod(
        CreatePayrollPeriodDto dto,
        CancellationToken cancellationToken)
    {
        var result =
            await _payrollService.CreatePeriodAsync(
                dto,
                cancellationToken);


        if (!result.Success)
            return BadRequest(result);


        return Ok(result);
    }


    [HttpPut("{periodId:int}")]
    [Authorize(Policy = HRPermissions.Payroll.Update)]
    public async Task<IActionResult> UpdatePeriod(int periodId,
    [FromBody] UpdatePayrollPeriodDto dto,
    CancellationToken cancellationToken)
    {
        var result =
            await _payrollService.UpdatePeriodAsync(
                periodId,
                dto,
                cancellationToken);


        if (!result.Success)
            return BadRequest(result);


        return Ok(result);
    }



    [HttpPost("{periodId:int}/generate")]
    [Authorize(
        Policy = HRPermissions.Payroll.Create)]
    public async Task<IActionResult> Generate(int periodId,
        CancellationToken cancellationToken)
    {
        var result =
            await _payrollService.GeneratePayrollAsync(
                periodId,
                cancellationToken);


        if (!result.Success)
            return BadRequest(result);


        return Ok(result);
    }



    [HttpPost("{payrollPeriodId:int}/approvePeriod")]
    [Authorize(
        Policy = HRPermissions.PayrollApproval.Approve)]
    public async Task<IActionResult> ApprovePeriod(int payrollPeriodId,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result =
            await _payrollService.ApprovePeriodAsync(
                payrollPeriodId,
                userId,
                cancellationToken);


        if (!result.Success)
            return BadRequest(result);


        return Ok(result);
    }



    [HttpPost("{id:int}/approve")]
    [Authorize(
        Policy = HRPermissions.PayrollApproval.Approve)]
    public async Task<IActionResult> Approve(int id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result =
            await _payrollService.ApproveAsync(
                id,
                userId,
                cancellationToken);


        if (!result.Success)
            return BadRequest(result);


        return Ok(result);
    }



    [HttpPost("{payrollPeriodId:int}/payPeriod")]
    [Authorize(
        Policy = HRPermissions.PayrollPayment.Pay)]
    public async Task<IActionResult> PayPeriod(int payrollPeriodId,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result =
            await _payrollService.MarkPeriodAsPaidAsync(
                payrollPeriodId,
                userId,
                cancellationToken);


        if (!result.Success)
            return BadRequest(result);


        return Ok(result);
    }


    [HttpPost("{id:int}/pay")]
    [Authorize(
        Policy = HRPermissions.PayrollPayment.Pay)]
    public async Task<IActionResult> Pay(int id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result =
            await _payrollService.MarkAsPaidAsync(
                id,
                userId,
                cancellationToken);


        if (!result.Success)
            return BadRequest(result);


        return Ok(result);
    }


    [HttpPost("{periodId:int}/close")]
    [Authorize(Policy = HRPermissions.Payroll.Update)]
    public async Task<IActionResult> Close(int periodId,
    CancellationToken cancellationToken)
    {
        var result =
            await _payrollService.ClosePeriodAsync(
                periodId,
                cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{periodId:int}")]
    [Authorize(Policy = HRPermissions.Payroll.Delete)]
    public async Task<IActionResult> DeletePeriod(int periodId,
    CancellationToken cancellationToken)
    {
        var result =
            await _payrollService.DeletePeriodAsync(
                periodId,
                cancellationToken);


        if (!result.Success)
            return BadRequest(result);


        return Ok(result);
    }
}