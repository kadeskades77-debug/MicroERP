using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.Attendance;

[Route("api/hr/attendance-devices")]
[ApiController]
public class AttendanceDeviceController : ControllerBase
{
    private readonly IAttendanceDeviceService _service;
    private readonly IAttendanceDeviceQueries _queries;


    public AttendanceDeviceController(
        IAttendanceDeviceService service,
        IAttendanceDeviceQueries queries)
    {
        _service = service;
        _queries = queries;
    }


    [HttpPost]
    [Authorize(Policy = HRPermissions.AttendanceDevice.Create)]
    public async Task<IActionResult> Create(
        CreateAttendanceDeviceDto dto)
    {
        var result =
            await _service.CreateAsync(dto);


        return Ok(result);
    }




    [HttpPut("{id}")]
    [Authorize(Policy = HRPermissions.AttendanceDevice.Update)]
    public async Task<IActionResult> Update(int id,
        UpdateAttendanceDeviceDto dto)
    {
        var result =
            await _service.UpdateAsync(id, dto);


        return Ok(result);
    }




    [HttpDelete("{id}")]
    [Authorize(Policy = HRPermissions.AttendanceDevice.Delete)]
    public async Task<IActionResult> Delete(int id)
    {
        var result =
            await _service.DeleteAsync(id);


        return Ok(result);
    }


    [HttpGet]
    [Authorize(Policy = HRPermissions.AttendanceDevice.View)]
    public async Task<IActionResult> GetAll(
    CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetAllAsync(cancellationToken);

        return Ok(result);
    }


    [HttpGet("{id}")]
    [Authorize(Policy = HRPermissions.AttendanceDevice.View)]
    public async Task<IActionResult> GetById(int id,
    CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetByIdAsync(
                id,
                cancellationToken);

        return Ok(result);
    }

    [HttpPut("{id:int}/activate")]
    [Authorize(Policy = HRPermissions.AttendanceDevice.Update)]
    public async Task<IActionResult> Activate(int id,
    CancellationToken cancellationToken)
    {
        var result =
            await _service.ActivateAsync(
                id,
                cancellationToken);

        return Ok(result);
    }

    [HttpPut("{id:int}/deactivate")]
    [Authorize(Policy = HRPermissions.AttendanceDevice.Update)]
    public async Task<IActionResult> Deactivate(int id,
    CancellationToken cancellationToken)
    {
        var result =
            await _service.DeactivateAsync(
                id,
                cancellationToken);

        return Ok(result);
    }
}