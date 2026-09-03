using Microsoft.EntityFrameworkCore;
using MicroERP.Application.Authorization.Interfaces;
using MicroERP.Domain.Identity;

namespace MicroERP.Persistence.Authorization;

public class AuthorizationInitializer : IAuthorizationInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly IPermissionDefinitionService _definitionService;

    public AuthorizationInitializer(
        ApplicationDbContext context,
        IPermissionDefinitionService definitionService)
    {
        _context = context;
        _definitionService = definitionService;
    }

    public async Task InitializeAsync()
    {
        // =========================================================
        // Ensure Permission Groups
        // =========================================================

        await EnsurePermissionGroupsAsync();

        // =========================================================
        // Ensure Permissions
        // =========================================================

        await EnsurePermissionsAsync();

        // =========================================================
        // Ensure Group Permissions
        // =========================================================

        await EnsureGroupPermissionsAsync();

        // =========================================================
        // Save Changes
        // =========================================================

        await _context.SaveChangesAsync();
    }

    // =============================================================
    // Permission Groups
    // =============================================================

    private async Task EnsurePermissionGroupsAsync()
    {
        var groups =
            _definitionService
                .GetGroups()
                .ToList();

        foreach (var definition in groups)
        {
            var group =
                await _context.PermissionGroups
                    .FirstOrDefaultAsync(
                        x => x.Key == definition.Key);

            if (group is null)
            {
                await _context.PermissionGroups.AddAsync(
                    new PermissionGroup
                    {
                        Key = definition.Key,
                        Name = definition.Name,
                        Description = definition.Description,
                        IsSystem = definition.IsSystem
                    });

                continue;
            }

            group.Name = definition.Name;
            group.Description = definition.Description;
            group.IsSystem = definition.IsSystem;
        }
    }

    // =============================================================
    // Permissions
    // =============================================================

    private async Task EnsurePermissionsAsync()
    {
        var permissions =
            _definitionService
                .GetPermissions()
                .ToList();

        foreach (var definition in permissions)
        {
            var permission =
                await _context.Permissions
                    .FirstOrDefaultAsync(
                        x => x.Key == definition.Key);

            if (permission is null)
            {
                await _context.Permissions.AddAsync(
                    new Permission
                    {
                        Key = definition.Key,
                        Name = definition.Name,
                        Description = definition.Description
                    });

                continue;
            }

            permission.Name = definition.Name;
            permission.Description = definition.Description;
        }
    }

    // =============================================================
    // Group Permissions
    // =============================================================

    private async Task EnsureGroupPermissionsAsync()
    {
        var groups =
            _definitionService
                .GetGroups()
                .ToList();

        var permissions =
            _definitionService
                .GetPermissions()
                .ToList();

        foreach (var permissionDefinition in permissions)
        {
            var groupDefinition =
           groups.FirstOrDefault(
        x => x.Key.Equals(
            permissionDefinition.GroupKey,
            StringComparison.OrdinalIgnoreCase));

            if (groupDefinition is null)
                continue;

            var group =
                await _context.PermissionGroups
                    .FirstOrDefaultAsync(
                        x => x.Key == groupDefinition.Key);

            if (group is null)
                continue;

            var permission =
                await _context.Permissions
                    .FirstOrDefaultAsync(
                        x => x.Key == permissionDefinition.Key);

            if (permission is null)
                continue;

            var exists =
                await _context.PermissionGroupPermissions
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