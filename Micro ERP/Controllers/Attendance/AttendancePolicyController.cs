using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.Attendance;

[Route("api/[controller]")]
[ApiController]
public class AttendancePolicyController : ControllerBase
{
    private readonly IAttendancePolicyService _service;


    public AttendancePolicyController(
        IAttendancePolicyService service)
    {
        _service = service;
    }




    [HttpGet]
    [Authorize(Policy = HRPermissions.Attendance.View)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result =
            await _service.GetAllAsync(
                cancellationToken);


        return Ok(result);
    }





    [HttpGet("{id:int}")]
    [Authorize(Policy = HRPermissions.Attendance.View)]
    public async Task<IActionResult> GetById(int id,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.GetByIdAsync(
                id,
                cancellationToken);


        return Ok(result);
    }





    [HttpPost]
    [Authorize(Policy = HRPermissions.Attendance.Create)]
    public async Task<IActionResult> Create(
        CreateAttendancePolicyDto dto,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.CreateAsync(
                dto,
                cancellationToken);


        return Ok(result);
    }





    [HttpPut("{id:int}")]
    [Authorize(Policy = HRPermissions.Attendance.Update)]
    public async Task<IActionResult> Update(
        int id,
        UpdateAttendancePolicyDto dto,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.UpdateAsync(
                id,
                dto,
                cancellationToken);


        return Ok(result);
    }





    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.DeleteAsync(
                id,
                cancellationToken);


        return Ok(result);
    }
}