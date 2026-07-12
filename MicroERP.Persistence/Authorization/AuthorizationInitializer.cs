using Microsoft.EntityFrameworkCore;
using MicroERP.Application.Authorization.Interfaces;
using MicroERP.Domain.Identity;
namespace MicroERP.Persistence.Authorization;

public class AuthorizationInitializer : IAuthorizationInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly IEnumerable<IPermissionDefinitionProvider> _providers;

    public AuthorizationInitializer(
        ApplicationDbContext context,
        IEnumerable<IPermissionDefinitionProvider> providers)
    {
        _context = context;
        _providers = providers;
    }

    public async Task InitializeAsync()
    {
        foreach (var provider in _providers)
        {
            await EnsurePermissionGroupAsync(provider);

            await EnsurePermissionsAsync(provider);

            await EnsureGroupPermissionsAsync(provider);
        }

        await _context.SaveChangesAsync();
    }

    private async Task EnsurePermissionGroupAsync(IPermissionDefinitionProvider provider)
    {
        var definition = provider.Group;

        var group = await _context.PermissionGroups
            .FirstOrDefaultAsync(x => x.Key == definition.Key);

        if (group is null)
        {
            await _context.PermissionGroups.AddAsync(new PermissionGroup
            {
                Name = definition.Name,
                Description = definition.Description,
                IsSystem = definition.IsSystem
            });

            return;
        }

        group.Name = definition.Name;
        group.Description = definition.Description;
        group.IsSystem = definition.IsSystem;
    }

    private async Task EnsurePermissionsAsync(IPermissionDefinitionProvider provider)
    {
        foreach (var definition in provider.GetPermissions())
        {
            var permission = await _context.Permissions
                .FirstOrDefaultAsync(x => x.Key == definition.Key);

            if (permission is null)
            {
                permission = new Permission
                {
                    Key = definition.Key,
                    Name = definition.Name,
                    Description = definition.Description
                };

                await _context.Permissions.AddAsync(permission);
            }
            else
            {
                permission.Name = definition.Name;
                permission.Description = definition.Description;
            }
        }
    }
    private async Task EnsureGroupPermissionsAsync(IPermissionDefinitionProvider provider)
    {
        var group = await _context.PermissionGroups
            .FirstAsync(x => x.Key == provider.Group.Key);

        foreach (var definition in provider.GetPermissions())
        {
            var permission = await _context.Permissions
                .FirstAsync(x => x.Key == definition.Key);

            var exists = await _context.PermissionGroupPermissions
                .AnyAsync(x =>
                    x.PermissionGroupId == group.Id &&
                    x.PermissionId == permission.Id);

            if (exists)
                continue;

            await _context.PermissionGroupPermissions.AddAsync(
                new PermissionGroupPermission
                {
                    PermissionGroupId = group.Id,
                    PermissionId = permission.Id
                });
        }
    }
}