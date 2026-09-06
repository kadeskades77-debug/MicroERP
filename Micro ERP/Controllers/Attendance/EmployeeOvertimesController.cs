using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Overtime;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.Attendance;


[Route("api/[controller]")]
[ApiController]
[Authorize]
public class EmployeeOvertimesController : ControllerBase
{
    private readonly IEmployeeOvertimeService _service;
    private readonly IEmployeeOvertimeQueries _queries;


    public EmployeeOvertimesController(
        IEmployeeOvertimeService service,
        IEmployeeOvertimeQueries queries)
    {
        _service = service;
        _queries = queries;
    }



    [HttpPost]
    [Authorize(Policy = HRPermissions.Overtime.Create)]
    public async Task<IActionResult> CreateManualOvertimeAsync(
        CreateManualOvertimeDto dto,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.CreateManualOvertimeAsync(
                dto,
                cancellationToken);


        if (!result.Success)
            return BadRequest(result);


        return Ok(result);
    }



    [HttpPut("{id:int}/UpdateOvertime")]
    [Authorize(Policy = HRPermissions.Overtime.Update)]
    public async Task<IActionResult> UpdateOvertime(int id,
        UpdateOvertimeDto dto,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.UpdateOvertimeAsync(
                id,
                dto,
                cancellationToken);


        if (!result.Success)
            return BadRequest(result);


        return Ok(result);
    }



    [HttpDelete("{id:int}")]
    [Authorize(Policy = HRPermissions.Overtime.Delete)]
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



    [HttpPut("{id:int}/approve")]
    [Authorize(Policy = HRPermissions.OvertimeApproval.Approve)]
    public async Task<IActionResult> Approve(int id,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.ApproveAsync(
                id,
                cancellationToken);


        if (!result.Success)
            return BadRequest(result);


        return Ok(result);
    }



    [HttpPut("{id:int}/reject")]
    [Authorize(Policy = HRPermissions.OvertimeApproval.Reject)]
    public async Task<IActionResult> Reject(int id,
        [FromBody] string reason,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.RejectAsync(
                id,
                reason,
                cancellationToken);


        if (!result.Success)
            return BadRequest(result);


        return Ok(result);
    }


    [HttpPut("{id:int}/cancel")]
    [Authorize(Policy = HRPermissions.Overtime.Delete)]
    public async Task<IActionResult> Cancel(int id,
    CancelEmployeeOvertimeDto dto,
    CancellationToken cancellationToken)
    {
        var result =
            await _service.CancelAsync(
                id,
                dto,
                cancellationToken);


        if (!result.Success)
            return BadRequest(result);


        return Ok(result);
    }



    [HttpGet]
    [Authorize(Policy = HRPermissions.Overtime.View)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetAllAsync(
                cancellationToken);


        return Ok(result);
    }



    [HttpGet("{id:int}")]
    [Authorize(Policy = HRPermissions.Overtime.View)]
    public async Task<IActionResult> GetById(int id,
        CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetByIdAsync(
                id,
                cancellationToken);


        return Ok(result);
    }



    [HttpGet("employee/{employeeId:int}")]
    [Authorize(Policy = HRPermissions.Overtime.View)]
    public async Task<IActionResult> GetEmployeeHistory(int employeeId,
        CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetEmployeeHistoryAsync(
                employeeId,
                cancellationToken);


        return Ok(result);
    }



    [HttpGet("pending")]
    [Authorize(Policy = HRPermissions.Overtime.View)]
    public async Task<IActionResult> GetPending(
        CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetPendingAsync(
                cancellationToken);


        return Ok(result);
    }



    [HttpGet("unpaid")]
    [Authorize(Policy = HRPermissions.Overtime.View)]
    public async Task<IActionResult> GetUnpaid(
        CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetUnpaidAsync(
                cancellationToken);


        return Ok(result);
    }
}