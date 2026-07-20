using MicroERP.Application.Common.Extensions;
using MicroERP.Application.Features.EmployeeSpecialLeaves.DTOs;
using MicroERP.Application.Features.EmployeeSpecialLeaves.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static MicroERP.Application.Authorization.Permissions.IdentityPermissions;

namespace MicroERP.API.Controllers;

[Route("api/special-leaves")]
[ApiController]
public class EmployeeSpecialLeavesController : ControllerBase
{
    private readonly IEmployeeSpecialLeaveService _service;
    private readonly IEmployeeSpecialLeaveQueries _queries;


    public EmployeeSpecialLeavesController(
        IEmployeeSpecialLeaveService service,
        IEmployeeSpecialLeaveQueries queries)
    {
        _service = service;
        _queries = queries;
    }



    [HttpPost]
    [Authorize(Policy = HRPermissions.EmployeeSpecialLeave.Create)]
    public async Task<IActionResult> Create(int employeeId,
        CreateSpecialLeaveDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(
            employeeId,
              User.GetUserId(),
            dto,
            cancellationToken);


        return Ok(result);
    }



    [HttpGet]
    [Authorize(Policy = HRPermissions.EmployeeSpecialLeave.View)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _queries.GetAllAsync(
            cancellationToken);


        return Ok(result);
    }



    [HttpGet("{id:int}")]
    [Authorize(Policy = HRPermissions.EmployeeSpecialLeave.View)]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _queries.GetByIdAsync(
            id,
            cancellationToken);


        if (result == null)
            return NotFound();


        return Ok(result);
    }




    [HttpPut("{id:int}/cancel")]
    [Authorize(Policy = HRPermissions.EmployeeSpecialLeave.Cancel)]
    public async Task<IActionResult> Cancel(int id,
     CancellationToken cancellationToken)
    {
        var result = await _service.CancelAsync(
            id,
            User.GetUserId(),
            cancellationToken);

        return Ok(result);
    }
}