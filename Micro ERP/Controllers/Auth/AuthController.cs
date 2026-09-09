using MicroERP.Application.Authorization.Permissions;
using MicroERP.Application.Features.Authorization.Auth.DTOs;
using MicroERP.Application.Features.Authorization.Auth.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.Auth;

[Route("api/[controller]")]
[ApiController]
public class AuthController : BaseApiController
{
    private readonly IdentityService _authService;

    public AuthController(IdentityService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [Authorize(Policy = IdentityPermissions.User.Register)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }


    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

  [HttpGet("me")]
  [Authorize]
  public async Task<IActionResult> GetCurrentUser()
  {
    var userId = User
        .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
        ?.Value;

    if (string.IsNullOrEmpty(userId))
        return Unauthorized();

    var result = await _authService.GetCurrentUserAsync(userId);

    return HandleResult(result);
  }

    [HttpPut("change-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var result = await _authService.ChangePasswordAsync(dto);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }


    [HttpPut("change-email")]
    [Authorize(Policy = IdentityPermissions.User.ChangeEmail)]
    public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailDto dto)
    {
        var result = await _authService.ChangeEmailAsync(dto);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }


    [HttpPost("reset-password/{userId}")]
    [Authorize(Policy = IdentityPermissions.UserPassword.Reset)]
    public async Task<IActionResult> ResetPassword(string userId)
    {
        var result = await _authService.ResetPasswordAsync(userId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }


    [HttpPut("activate/{userId}")]
    [Authorize(Policy = IdentityPermissions.UserActivation.Activate)]
    public async Task<IActionResult> ActivateUser(string userId)
    {
        var result = await _authService.ActivateUserAsync(userId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }


    [HttpPut("deactivate/{userId}")]
    [Authorize(Policy = IdentityPermissions.UserActivation.Deactivate)]
    public async Task<IActionResult> DeactivateUser(string userId)
    {
        var result = await _authService.DeactivateUserAsync(userId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }


    [HttpPut("lock/{userId}")]
    [Authorize(Policy = IdentityPermissions.UserLock.Lock)]
    public async Task<IActionResult> LockUser(string userId)
    {
        var result = await _authService.LockUserAsync(userId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }


    [HttpPut("unlock/{userId}")]
    [Authorize(Policy = IdentityPermissions.UserLock.Unlock)]
    public async Task<IActionResult> UnlockUser(string userId)
    {
        var result = await _authService.UnlockUserAsync(userId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}