using MicroERP.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected IActionResult HandleResult(Result result)
    {
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}