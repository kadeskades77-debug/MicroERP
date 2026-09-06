using MicroERP.Application.Features.EmployeeAttendance.WorkSchedules.DTOs;
using MicroERP.Application.Features.EmployeeAttendance.WorkSchedules.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.Attendance;

[Route("api/hr/work-schedules")]
[ApiController]
public class WorkScheduleController : ControllerBase
{
    private readonly IWorkScheduleService _service;
    private readonly IWorkScheduleQueries _queries;


    public WorkScheduleController(
        IWorkScheduleService service,
        IWorkScheduleQueries queries)
    {
        _service = service;
        _queries = queries;
    }



    [HttpGet]
    [Authorize(Policy = HRPermissions.WorkSchedule.View)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _queries.GetAllAsync(cancellationToken);

        return Ok(result);
    }



    [HttpGet("{id:int}")]
    [Authorize(Policy = HRPermissions.WorkSchedule.View)]
    public async Task<IActionResult> GetById(int id,
        CancellationToken cancellationToken)
    {
        var result = await _queries.GetByIdAsync(id,cancellationToken);

        if (result == null)
            return NotFound();

        return Ok(result);
    }



    [HttpPost]
    [Authorize(Policy = HRPermissions.WorkSchedule.Create)]
    public async Task<IActionResult> Create(CreateWorkScheduleDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(dto,cancellationToken);

        return Ok(result);
    }



    [HttpPut("{id:int}")]
    [Authorize(Policy = HRPermissions.WorkSchedule.Update)]
    public async Task<IActionResult> Update(int id,UpdateWorkScheduleDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(id,dto,cancellationToken);

        return Ok(result);
    }



    [HttpDelete("{id:int}")]
    [Authorize(Policy = HRPermissions.WorkSchedule.Delete)]
    public async Task<IActionResult> Delete(int id,
        CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(
            id,
            cancellationToken);

        return Ok(result);
    }
}