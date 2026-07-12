using System.Security.Claims;
using MicroERP.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace MicroERP.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }


    private ClaimsPrincipal? User =>
        _httpContextAccessor.HttpContext?.User;


    public string? UserId =>
        User?.FindFirstValue(
            ClaimTypes.NameIdentifier);


    public string? UserName =>
        User?.FindFirstValue(
            ClaimTypes.Name)
        ??
        User?.Identity?.Name;


    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated ?? false;
}