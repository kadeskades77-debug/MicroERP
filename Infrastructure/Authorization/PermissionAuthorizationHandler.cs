using MicroERP.Application.Authorization.Interfaces;
using MicroERP.Application.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace MicroERP.Infrastructure.Authorization;

public class PermissionAuthorizationHandler
    : AuthorizationHandler<PermissionRequirement>
{
    private readonly IAuthorizationManager _authorizationManager;

    public PermissionAuthorizationHandler(
        IAuthorizationManager authorizationManager)
    {
        _authorizationManager = authorizationManager;
    }


    protected override async Task HandleRequirementAsync(
     AuthorizationHandlerContext context,
     PermissionRequirement requirement)
    {
        var userId = context.User
            .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
            ?.Value;


        if (string.IsNullOrEmpty(userId))
            return;


        var permissions =
            await _authorizationManager
                .GetPermissionsByUserAsync(userId);


        if (permissions.Contains(
            requirement.Permission,
            StringComparer.OrdinalIgnoreCase))
        {
            context.Succeed(requirement);
        }
    }
}