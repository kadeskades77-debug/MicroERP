using MicroERP.Application.Features.Leaves.EmployeeLeaveBalances.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.EmployeeLeave;

[Route("api/employees/{employeeId}/leave-balances")]
[ApiController]
[Authorize]
public class EmployeeLeaveBalancesController : ControllerBase
{
    private readonly IEmployeeLeaveBalanceService _service;


    public EmployeeLeaveBalancesController(
        IEmployeeLeaveBalanceService service)
    {
        _service = service;
    }



    // Get employee leave balances
    [HttpGet]
    [Authorize(Policy = HRPermissions.EmployeeLeaveBalance.View)]
    public async Task<IActionResult> GetByEmployee(int employeeId,[FromQuery] int year,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetByEmployeeAsync(
            employeeId,
            year,
            cancellationToken);


        return Ok(result);
    }
}