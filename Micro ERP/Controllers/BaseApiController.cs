using MicroERP.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

    protected string GetCurrentUserId()
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedAccessException(
                "User not authenticated.");
        }

        return userId;
    }
}