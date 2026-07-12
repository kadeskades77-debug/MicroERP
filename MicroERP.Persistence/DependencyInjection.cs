using MicroERP.Application.Features.PermissionGroups.Interfaces;
using MicroERP.Infrastructure.Services;
using MicroERP.Persistence.Authorization;
using MicroERP.Persistence.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicroERP.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")
            ));
        services.AddScoped<IPermissionGroupService, PermissionGroupService>();
        services.AddScoped<PermissionSeeder>();
        return services;

    }
}