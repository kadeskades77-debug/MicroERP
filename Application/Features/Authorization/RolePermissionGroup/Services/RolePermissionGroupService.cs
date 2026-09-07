using MicroERP.Application.Authorization.Interfaces;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.Authorization.RolePermissionGroup.DTOs;
using MicroERP.Application.Features.Authorization.RolePermissionGroup.Interfaces;
using MicroERP.Domain.Audit;
using MicroERP.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RolePermissionGroupEntity =
    MicroERP.Domain.Identity.RolePermissionGroup;

namespace MicroERP.Application.Features.Authorization.RolePermissionGroup.Services
{
   

    public class RolePermissionGroupService
        : IRolePermissionGroupService
    {
        private readonly IApplicationDbContext _context;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IAuditService _auditService;
        private readonly IAuthorizationManager _authorizationManager;

        public RolePermissionGroupService(
            IApplicationDbContext context,
            RoleManager<ApplicationRole> roleManager,
            IAuditService auditService,
            IAuthorizationManager authorizationManager)
        {
            _context = context;
            _roleManager = roleManager;
            _auditService = auditService;
            _authorizationManager = authorizationManager;
        }

        //================ GET =================

        public async Task<Result<RolePermissionGroupDto>>
            GetRolePermissionGroupsAsync(
                string roleId,
                CancellationToken cancellationToken = default)
        {
            var role = await _roleManager.Roles
                .AsNoTracking()
                .Where(x => x.Id == roleId)
                .Select(x => new
                {
                    x.Id,
                    x.Name
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (role is null)
                return Result<RolePermissionGroupDto>
                    .Failure("Role not found.");

            var keys = await _context.RolePermissionGroups
                .AsNoTracking()
                .Where(x => x.RoleId == roleId)
                .Select(x => x.PermissionGroup.Key)
                .OrderBy(x => x)
                .ToListAsync(cancellationToken);

            return Result<RolePermissionGroupDto>.Succeeded(
                new RolePermissionGroupDto
                {
                    RoleId = role.Id,
                    RoleName = role.Name!,
                    PermissionGroupKeys = keys
                });
        }

        //================ ADD =================

        public async Task<Result> AddPermissionGroupsAsync(
            AddRolePermissionGroupsDto dto,
            CancellationToken cancellationToken = default)
        {
            return await ExecuteInTransaction(
                async () =>
                {
                    var role = await _roleManager.Roles
                        .FirstOrDefaultAsync(
                            x => x.Id == dto.RoleId,
                            cancellationToken);

                    if (role is null)
                        return Result.Failure(
                            "Role not found.");

                    var keys = NormalizeKeys(
                        dto.PermissionGroupKeys);

                    if (keys.Count == 0)
                        return Result.Failure(
                            "At least one permission group is required.");

                    var groups = await _context.PermissionGroups
                        .Where(x => keys.Contains(x.Key))
                        .ToListAsync(cancellationToken);

                    if (groups.Count != keys.Count)
                        return Result.Failure(
                            "One or more permission groups are invalid.");

                    var existingKeys =
                        await _context.RolePermissionGroups
                            .Where(x =>
                                x.RoleId == dto.RoleId &&
                                keys.Contains(
                                    x.PermissionGroup.Key))
                            .Select(x =>
                                x.PermissionGroup.Key)
                            .ToListAsync(cancellationToken);

                    if (existingKeys.Count > 0)
                        return Result.Failure(
                            "One or more permission groups are already assigned to the role.");

                    var oldKeys =
                        await _context.RolePermissionGroups
                            .Where(x => x.RoleId == role.Id)
                            .Select(x =>
                                x.PermissionGroup.Key)
                            .ToListAsync(cancellationToken);

                    foreach (var group in groups)
                    {
                        _context.RolePermissionGroups.Add(
                            new RolePermissionGroupEntity
                            {
                                RoleId = role.Id,
                                PermissionGroupId = group.Id
                            });
                    }

                    var newKeys = oldKeys
                        .Concat(groups.Select(x => x.Key))
                        .Distinct(
                            StringComparer.OrdinalIgnoreCase)
                        .OrderBy(x => x)
                        .ToList();

                    await _auditService.LogAsync(
                        AuditActions.Update,
                        nameof(RolePermissionGroup),
                        role.Id,
                        new
                        {
                            PermissionGroups = oldKeys
                        },
                        new
                        {
                            PermissionGroups = newKeys
                        },
                        cancellationToken: cancellationToken);

                    await _authorizationManager
                        .ClearRoleUsersPermissionsCacheAsync(
                            role.Id,
                            cancellationToken);

                    return Result.Succeeded(
                        "Permission groups assigned successfully.");
                },
                cancellationToken);
        }

        //================ REPLACE =================

        public async Task<Result> ReplacePermissionGroupsAsync(
            UpdateRolePermissionGroupsDto dto,
            CancellationToken cancellationToken = default)
        {
            return await ExecuteInTransaction(
                async () =>
                {
                    var role = await _roleManager.Roles
                        .FirstOrDefaultAsync(
                            x => x.Id == dto.RoleId,
                            cancellationToken);

                    if (role is null)
                        return Result.Failure(
                            "Role not found.");

                    var keys = NormalizeKeys(
                        dto.PermissionGroupKeys);

                    var groups = await _context.PermissionGroups
                        .Where(x => keys.Contains(x.Key))
                        .ToListAsync(cancellationToken);

                    if (groups.Count != keys.Count)
                        return Result.Failure(
                            "One or more permission groups are invalid.");

                    var oldLinks =
                        await _context.RolePermissionGroups
                            .Include(x => x.PermissionGroup)
                            .Where(x => x.RoleId == role.Id)
                            .ToListAsync(cancellationToken);

                    var oldKeys = oldLinks
                        .Select(x => x.PermissionGroup.Key)
                        .OrderBy(x => x)
                        .ToList();

                    _context.RolePermissionGroups
                        .RemoveRange(oldLinks);

                    foreach (var group in groups)
                    {
                        _context.RolePermissionGroups.Add(
                            new RolePermissionGroupEntity
                            {
                                RoleId = role.Id,
                                PermissionGroupId = group.Id
                            });
                    }

                    var newKeys = groups
                        .Select(x => x.Key)
                        .OrderBy(x => x)
                        .ToList();

                    await _auditService.LogAsync(
                        AuditActions.Update,
                        nameof(RolePermissionGroup),
                        role.Id,
                        new
                        {
                            PermissionGroups = oldKeys
                        },
                        new
                        {
                            PermissionGroups = newKeys
                        },
                        cancellationToken: cancellationToken);

                    await _authorizationManager
                        .ClearRoleUsersPermissionsCacheAsync(
                            role.Id,
                            cancellationToken);

                    return Result.Succeeded(
                        "Role permission groups replaced successfully.");
                },
                cancellationToken);
        }

        //================ REMOVE ONE =================

        public async Task<Result> RemovePermissionGroupAsync(
            RemoveRolePermissionGroupDto dto,
            CancellationToken cancellationToken = default)
        {
            return await ExecuteInTransaction(
                async () =>
                {
                    var role = await _roleManager.Roles
                        .FirstOrDefaultAsync(
                            x => x.Id == dto.RoleId,
                            cancellationToken);

                    if (role is null)
                        return Result.Failure(
                            "Role not found.");

                    var key =
                        dto.PermissionGroupKey.Trim();

                    var link =
                        await _context.RolePermissionGroups
                            .Include(x => x.PermissionGroup)
                            .FirstOrDefaultAsync(
                                x =>
                                    x.RoleId == dto.RoleId &&
                                    x.PermissionGroup.Key == key,
                                cancellationToken);

                    if (link is null)
                        return Result.Failure(
                            "Permission group is not assigned to the role.");

                    var oldKeys =
                        await _context.RolePermissionGroups
                            .Where(x => x.RoleId == role.Id)
                            .Select(x =>
                                x.PermissionGroup.Key)
                            .ToListAsync(cancellationToken);

                    _context.RolePermissionGroups
                        .Remove(link);

                    var newKeys = oldKeys
                        .Where(x =>
                            !x.Equals(
                                key,
                                StringComparison.OrdinalIgnoreCase))
                        .OrderBy(x => x)
                        .ToList();

                    await _auditService.LogAsync(
                        AuditActions.Update,
                        nameof(RolePermissionGroup),
                        role.Id,
                        new
                        {
                            PermissionGroups = oldKeys
                        },
                        new
                        {
                            PermissionGroups = newKeys
                        },
                        cancellationToken: cancellationToken);

                    await _authorizationManager
                        .ClearRoleUsersPermissionsCacheAsync(
                            role.Id,
                            cancellationToken);

                    return Result.Succeeded(
                        "Permission group removed successfully.");
                },
                cancellationToken);
        }

        //================ REMOVE ALL =================

        public async Task<Result> RemoveAllPermissionGroupsAsync(
            string roleId,
            CancellationToken cancellationToken = default)
        {
            return await ExecuteInTransaction(
                async () =>
                {
                    var role = await _roleManager.Roles
                        .FirstOrDefaultAsync(
                            x => x.Id == roleId,
                            cancellationToken);

                    if (role is null)
                        return Result.Failure(
                            "Role not found.");

                    var links =
                        await _context.RolePermissionGroups
                            .Include(x => x.PermissionGroup)
                            .Where(x => x.RoleId == roleId)
                            .ToListAsync(cancellationToken);

                    if (links.Count == 0)
                        return Result.Succeeded(
                            "Role has no permission groups.");

                    var oldKeys = links
                        .Select(x => x.PermissionGroup.Key)
                        .OrderBy(x => x)
                        .ToList();

                    _context.RolePermissionGroups
                        .RemoveRange(links);

                    await _auditService.LogAsync(
                        AuditActions.Update,
                        nameof(RolePermissionGroup),
                        role.Id,
                        new
                        {
                            PermissionGroups = oldKeys
                        },
                        new
                        {
                            PermissionGroups =
                                Array.Empty<string>()
                        },
                        cancellationToken: cancellationToken);

                    await _authorizationManager
                        .ClearRoleUsersPermissionsCacheAsync(
                            role.Id,
                            cancellationToken);

                    return Result.Succeeded(
                        "All permission groups removed successfully.");
                },
                cancellationToken);
        }

        //================ HELPERS =================

        private static List<string> NormalizeKeys(
            IEnumerable<string>? keys)
        {
            return (keys ?? Enumerable.Empty<string>())
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(
                    StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private async Task<Result> ExecuteInTransaction(
            Func<Task<Result>> action,
            CancellationToken cancellationToken)
        {
            var strategy =
                _context.Database
                    .CreateExecutionStrategy();

            return await strategy.ExecuteAsync(
                async () =>
                {
                    await using var transaction =
                        await _context.Database
                            .BeginTransactionAsync(
                                cancellationToken);

                    try
                    {
                        var result = await action();

                        if (!result.Success)
                        {
                            await transaction.RollbackAsync(
                                cancellationToken);

                            return result;
                        }

                        await _context.SaveChangesAsync(
                            cancellationToken);

                        await transaction.CommitAsync(
                            cancellationToken);

                        return result;
                    }
                    catch
                    {
                        await transaction.RollbackAsync(
                            cancellationToken);

                        throw;
                    }
                });
        }
    }
}
