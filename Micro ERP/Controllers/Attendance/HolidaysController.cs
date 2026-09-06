using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.Holidays.DTOs;
using MicroERP.Application.Features.EmployeeAttendance.Holidays.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.Attendance;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class HolidaysController : ControllerBase
{
    private readonly IHolidayService _service;
    private readonly IHolidayQueries _queries;


    public HolidaysController(
        IHolidayService service,
        IHolidayQueries queries)
    {
        _service = service;
        _queries = queries;
    }



    [HttpGet("{id:int}")]
    [Authorize(Policy = HRPermissions.Holiday.View)]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetByIdAsync(
                id,
                cancellationToken);


        return result.Success
            ? Ok(result)
            : NotFound(result);
    }



    [HttpGet]
    [Authorize(Policy = HRPermissions.Holiday.View)]
    public async Task<IActionResult> GetAll(
        [FromQuery] HolidayFilterDto filter,
        [FromQuery] PagedRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetAllAsync(
                filter,
                request,
                cancellationToken);


        return result.Success
            ? Ok(result)
            : BadRequest(result);
    }



    [HttpGet("by-date")]
    [Authorize(Policy = HRPermissions.Holiday.View)]
    public async Task<IActionResult> GetByDate(
        [FromQuery] DateOnly date,
        CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetByDateAsync(
                date,
                cancellationToken);


        return result.Success
             ? Ok(result)
             : BadRequest(result);
    }



    [HttpPost]
    [Authorize(Policy = HRPermissions.Holiday.Create)]
    public async Task<IActionResult> Create(
        CreateHolidayDto dto,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.CreateAsync(
                dto,
                cancellationToken);


        return result.Success
            ? Ok(result)
            : BadRequest(result);
    }



    [HttpPut("{id:int}")]
    [Authorize(Policy = HRPermissions.Holiday.Update)]
    public async Task<IActionResult> Update(int id,UpdateHolidayDto dto,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.UpdateAsync(
                id,
                dto,
                cancellationToken);

        return result.Success
                    ? Ok(result)
                    : BadRequest(result);
    }



    [HttpDelete("{id:int}")]
    [Authorize(Policy = HRPermissions.Holiday.Delete)]
    public async Task<IActionResult> Delete(int id,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.DeleteAsync(
                id,
                cancellationToken);


        return result.Success
                    ? Ok(result)
                    : BadRequest(result);
    }
}