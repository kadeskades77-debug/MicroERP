using MicroERP.Application.Authorization.Interfaces;
using MicroERP.Application.Common.Files.Interfaces;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.Authentication.Auth.Interfaces;
using MicroERP.Application.Features.Authentication.Roles.Interfaces;
using MicroERP.Application.Features.Authorization.Permissions.Interfaces;
using MicroERP.Application.Features.Authorization.UserPermissions.Interfaces;
using MicroERP.Infrastructure.Authorization;
using MicroERP.Infrastructure.BackgroundJobs;
using MicroERP.Infrastructure.Security;
using MicroERP.Infrastructure.Services;
using MicroERP.Infrastructure.Services.Files;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace MicroERP.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        services.AddMemoryCache();

        services.AddHttpContextAccessor();


        // Current User

        services.AddScoped<ICurrentUserService, CurrentUserService>();



        // Authentication

        services.AddScoped<IJwtService, JwtService>();

        services.AddScoped<IdentityService, dentityService>();

        services.AddScoped<ICredentialGenerator, CredentialsGenerator>();



        // Authorization

        services.AddScoped<IAuthorizationManager, AuthorizationManager>();



        // Roles & Permissions

        services.AddScoped<IUserRoleService, UserRoleService>();

        services.AddScoped<IRoleService, RoleService>();

        services.AddScoped<IUserPermissionAssignmentService,
            UserPermissionAssignmentService>();

        services.AddScoped<IPermissionService, PermissionService>();



        // Audit

        services.AddScoped<IAuditService, AuditService>();



        // Files

        services.AddScoped<IFileStorageService, FileStorageService>();




        // Leave Background Jobs

        services.AddHostedService<YearlyLeaveBalanceJob>();


        return services;
    }
}