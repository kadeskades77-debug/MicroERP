using MicroERP.Application.Authorization;
using MicroERP.Application.Authorization.Interfaces;
using MicroERP.Domain.Identity;
using MicroERP.Domin.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Persistence.Authorization;

public class PermissionSeeder
{
    private const string SuperAdminGroupKey = "SuperAdmin";
    private const string SuperAdminRoleName = "SuperAdmin";

    private readonly ApplicationDbContext _context;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public PermissionSeeder(
        ApplicationDbContext context,
        RoleManager<ApplicationRole> roleManager)
    {
        _context = context;
        _roleManager = roleManager;
    }

    // =========================================================
    // Seed
    // =========================================================

    public async Task SeedAsync(
        IEnumerable<IPermissionDefinitionProvider> providers,
        IEnumerable<IMultiPermissionDefinitionProvider> multiProviders)
    {
        var strategy =
            _context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // =================================================
                // Single Group Providers
                // =================================================

                foreach (var provider in providers)
                {
                    await SeedProviderAsync(provider);
                }

                // =================================================
                // Multi Group Providers
                // =================================================

                foreach (var provider in multiProviders)
                {
                    await SeedMultiProviderAsync(provider);
                }

                // =================================================
                // Super Admin Group
                // =================================================

                var superAdminGroup =
                    await EnsureSuperAdminGroupAsync();

                // =================================================
                // Super Admin Role
                // =================================================

                await EnsureSuperAdminRoleAsync();

                // =================================================
                // Link All Permissions
                // =================================================

                await LinkSuperAdminPermissionsAsync(
                    superAdminGroup);

                // =================================================
                // Assign Super Admin Users
                // =================================================

                await AssignSuperAdminUsersAsync(
                    superAdminGroup);

                // =================================================
                // Save
                // =================================================

                await _context.SaveChangesAsync();

                // =================================================
                // Commit
                // =================================================

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }

    // =========================================================
    // Seed Single Group Provider
    // =========================================================

    private async Task SeedProviderAsync(
        IPermissionDefinitionProvider provider)
    {
        var groupDefinition =
            provider.Group;

        var group =
            await EnsurePermissionGroupAsync(
                groupDefinition);

        foreach (var permissionDefinition
                 in provider.GetPermissions())
        {
            await EnsurePermissionAsync(
                permissionDefinition,
                group);
        }
    }

    // =========================================================
    // Seed Multi Group Provider
    // =========================================================

    private async Task SeedMultiProviderAsync(
        IMultiPermissionDefinitionProvider provider)
    {
        // =========================================================
        // Ensure Groups
        // =========================================================

        foreach (var groupDefinition in provider.Groups)
        {
            await EnsurePermissionGroupAsync(
                groupDefinition);
        }

        // =========================================================
        // Ensure Permissions
        // =========================================================

        foreach (var permissionDefinition
                 in provider.GetPermissions())
        {
            if (string.IsNullOrWhiteSpace(
                permissionDefinition.GroupKey))
            {
                throw new InvalidOperationException(
                    $"Permission '{permissionDefinition.Key}' " +
                    $"does not have a GroupKey.");
            }

            var group =
                await _context.PermissionGroups
                    .FirstOrDefaultAsync(
                        x =>
                            x.Key ==
                            permissionDefinition.GroupKey);

            if (group is null)
            {
                throw new InvalidOperationException(
                    $"Permission group " +
                    $"'{permissionDefinition.GroupKey}' " +
                    $"was not found for permission " +
                    $"'{permissionDefinition.Key}'.");
            }

            await EnsurePermissionAsync(
                permissionDefinition,
                group);
        }
    }

    // =========================================================
    // Ensure Permission Group
    // =========================================================

    private async Task<PermissionGroup>
        EnsurePermissionGroupAsync(
            PermissionGroupDefinition definition)
    {
        var group =
            await _context.PermissionGroups
                .FirstOrDefaultAsync(
                    x =>
                        x.Key == definition.Key);

        // =========================================================
        // Create
        // =========================================================

        if (group is null)
        {
            group =
                new PermissionGroup
                {
                    Key = definition.Key,
                    Name = definition.Name,
                    Description = definition.Description,
                    IsSystem = definition.IsSystem
                };

            await _context.PermissionGroups.AddAsync(
                group);

            await _context.SaveChangesAsync();

            return group;
        }

        // =========================================================
        // Restore Soft Deleted Group
        // =========================================================

        if (group.IsDeleted)
        {
            group.IsDeleted = false;
            group.IsActive = true;
        }

        // =========================================================
        // Update Definition
        // =========================================================

        group.Name =
            definition.Name;

        group.Description =
            definition.Description;

        group.IsSystem =
            definition.IsSystem;

        return group;
    }

    // =========================================================
    // Ensure Permission
    // =========================================================

    private async Task EnsurePermissionAsync(
        PermissionDefinition definition,
        PermissionGroup group)
    {
        var permission =
            await _context.Permissions
                .FirstOrDefaultAsync(
                    x =>
                        x.Key == definition.Key);

        // =========================================================
        // Create Permission
        // =========================================================

        if (permission is null)
        {
            permission =
                new Permission
                {
                    Key = definition.Key,
                    Name = definition.Name,
                    Description = definition.Description
                };

            await _context.Permissions.AddAsync(
                permission);

            await _context.SaveChangesAsync();
        }
        else
        {
            // =====================================================
            // Restore Soft Deleted Permission
            // =====================================================

            if (permission.IsDeleted)
            {
                permission.IsDeleted = false;
                permission.IsActive = true;
            }

            // =====================================================
            // Update Definition
            // =====================================================

            permission.Name =
                definition.Name;

            permission.Description =
                definition.Description;
        }

        // =========================================================
        // Ensure Group Permission
        // =========================================================

        await EnsureGroupPermissionAsync(
            group.Id,
            permission.Id);
    }

    // =========================================================
    // Ensure Group Permission
    // =========================================================

    private async Task EnsureGroupPermissionAsync(
        int groupId,
        int permissionId)
    {
        // =========================================================
        // Check Database
        // =========================================================

        var existsInDatabase =
            await _context.PermissionGroupPermissions
                .AnyAsync(
                    x =>
                        x.PermissionGroupId == groupId &&
                        x.PermissionId == permissionId);

        if (existsInDatabase)
            return;

        // =========================================================
        // Check Change Tracker
        // =========================================================

        var existsInTracker =
            _context.ChangeTracker
                .Entries<PermissionGroupPermission>()
                .Any(
                    x =>
                        x.Entity.PermissionGroupId == groupId &&
                        x.Entity.PermissionId == permissionId &&
                        x.State != EntityState.Deleted);

        if (existsInTracker)
            return;

        // =========================================================
        // Add Relation
        // =========================================================

        await _context.PermissionGroupPermissions
            .AddAsync(
                new PermissionGroupPermission
                {
                    PermissionGroupId = groupId,
                    PermissionId = permissionId
                });
    }

    // =========================================================
    // Ensure Super Admin Group
    // =========================================================

    private async Task<PermissionGroup>
        EnsureSuperAdminGroupAsync()
    {
        var group =
            await _context.PermissionGroups
                .FirstOrDefaultAsync(
                    x =>
                        x.Key ==
                        SuperAdminGroupKey);

        // =========================================================
        // Create Group
        // =========================================================

        if (group is null)
        {
            group =
                new PermissionGroup
                {
                    Key = SuperAdminGroupKey,
                    Name = "Super Admin",
                    Description =
                        "Full system permissions.",
                    IsSystem = true
                };

            await _context.PermissionGroups.AddAsync(
                group);

            await _context.SaveChangesAsync();

            return group;
        }

        // =========================================================
        // Restore Soft Deleted Group
        // =========================================================

        if (group.IsDeleted)
        {
            group.IsDeleted = false;
            group.IsActive = true;
        }

        // =========================================================
        // Update Group
        // =========================================================

        group.Name =
            "Super Admin";

        group.Description =
            "Full system permissions.";

        group.IsSystem =
            true;

        return group;
    }

    // =========================================================
    // Ensure Super Admin Role
    // =========================================================

    private async Task<ApplicationRole>
        EnsureSuperAdminRoleAsync()
    {
        var role =
            await _roleManager
                .FindByNameAsync(
                    SuperAdminRoleName);

        // =========================================================
        // Existing Role
        // =========================================================

        if (role is not null)
            return role;

        // =========================================================
        // Create Role
        // =========================================================

        role =
            new ApplicationRole
            {
                Name = SuperAdminRoleName,
                Description =
                    "Full system administrator role.",
                IsSystem = true
            };

        var result =
            await _roleManager.CreateAsync(
                role);

        // =========================================================
        // Validate Result
        // =========================================================

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(
                    Environment.NewLine,
                    result.Errors
                        .Select(
                            x => x.Description)));
        }

        return role;
    }

    // =========================================================
    // Link All Permissions To Super Admin
    // =========================================================

    private async Task LinkSuperAdminPermissionsAsync(
        PermissionGroup group)
    {
        // =========================================================
        // Get Active Permissions
        // =========================================================

        var allPermissions =
            await _context.Permissions
                .Where(
                    x =>
                        !x.IsDeleted)
                .ToListAsync();

        // =========================================================
        // Get Existing Links From Database
        // =========================================================

        var linkedIds =
            await _context.PermissionGroupPermissions
                .Where(
                    x =>
                        x.PermissionGroupId ==
                        group.Id)
                .Select(
                    x =>
                        x.PermissionId)
                .ToListAsync();

        // =========================================================
        // Get Existing Links From Change Tracker
        // =========================================================

        var trackedPermissionIds =
            _context.ChangeTracker
                .Entries<PermissionGroupPermission>()
                .Where(
                    x =>
                        x.Entity.PermissionGroupId ==
                            group.Id &&
                        x.State !=
                            EntityState.Deleted)
                .Select(
                    x =>
                        x.Entity.PermissionId)
                .ToHashSet();

        // =========================================================
        // Add Missing Permissions
        // =========================================================

        var missing =
            allPermissions
                .Where(
                    permission =>
                        !linkedIds.Contains(
                            permission.Id) &&
                        !trackedPermissionIds.Contains(
                            permission.Id))
                .Select(
                    permission =>
                        new PermissionGroupPermission
                        {
                            PermissionGroupId =
                                group.Id,

                            PermissionId =
                                permission.Id
                        })
                .ToList();

        if (missing.Count == 0)
            return;

        await _context.PermissionGroupPermissions
            .AddRangeAsync(
                missing);
    }

    // =========================================================
    // Assign Super Admin Group To Super Admin Users
    // =========================================================

    private async Task AssignSuperAdminUsersAsync(
        PermissionGroup group)
    {
        // =========================================================
        // Get Super Admin Role
        // =========================================================

        var role =
            await _roleManager
                .FindByNameAsync(
                    SuperAdminRoleName);

        if (role is null)
            return;

        // =========================================================
        // Get Users With Super Admin Role
        // =========================================================

        var userIds =
            await _context.UserRoles
                .Where(
                    x =>
                        x.RoleId ==
                        role.Id)
                .Select(
                    x =>
                        x.UserId)
                .ToListAsync();

        // =========================================================
        // Assign Group
        // =========================================================

        foreach (var userId in userIds)
        {
            // -----------------------------------------------------
            // Database Check
            // -----------------------------------------------------

            var existsInDatabase =
                await _context
                    .UserPermissionAssignments
                    .AnyAsync(
                        x =>
                            x.UserId == userId &&
                            x.PermissionGroupId ==
                                group.Id);

            if (existsInDatabase)
                continue;

            // -----------------------------------------------------
            // Change Tracker Check
            // -----------------------------------------------------

            var existsInTracker =
                _context.ChangeTracker
                    .Entries<UserPermissionAssignment>()
                    .Any(
                        x =>
                            x.Entity.UserId == userId &&
                            x.Entity.PermissionGroupId ==
                                group.Id &&
                            x.State !=
                                EntityState.Deleted);

            if (existsInTracker)
                continue;

            // -----------------------------------------------------
            // Add Assignment
            // -----------------------------------------------------

            await _context.UserPermissionAssignments
                .AddAsync(
                    new UserPermissionAssignment
                    {
                        UserId = userId,
                        PermissionGroupId =
                            group.Id
                    });
        }
    }
}