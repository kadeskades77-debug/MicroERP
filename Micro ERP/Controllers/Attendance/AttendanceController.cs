using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Report;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.Attendance;

[Route("api/hr/attendance")]
[ApiController]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _service;
    private readonly IAttendanceQueries _queries;


    public AttendanceController(
        IAttendanceService service,
        IAttendanceQueries queries)
    {
        _service = service;
        _queries = queries;
    }



    [HttpGet]
    [Authorize(Policy = HRPermissions.Attendance.View)]
    public async Task<IActionResult> GetAll(
        [FromQuery] AttendanceFilterDto filter,
        [FromQuery] PagedRequest request)
    {
        var result = await _queries.GetAllAsync(filter, request);

        return Ok(result);
    }


    [HttpPost]
    [Authorize(Policy = HRPermissions.Attendance.Create)]
    public async Task<IActionResult> Create(
        CreateAttendanceDto dto)
    {
        var result =
            await _service.CreateAsync(dto);

        return Ok(result);
    }



    [HttpPut("{id}")]
    [Authorize(Policy = HRPermissions.Attendance.Update)]
    public async Task<IActionResult> Update(int id,
        UpdateAttendanceDto dto)
    {
        var result =
            await _service.UpdateAsync(id, dto);

        return Ok(result);
    }



    [HttpDelete("{id}")]
    [Authorize(Policy = HRPermissions.Attendance.Update)]
    public async Task<IActionResult> Delete(int id)
    {
        var result =
            await _service.DeleteAsync(id);

        return Ok(result);
    }
}