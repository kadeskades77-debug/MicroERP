using MicroERP.Application.Authorization.Interfaces;
using MicroERP.Application.Common.DTOs;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.Authorization.PermissionGroups.DTOs;
using MicroERP.Application.Features.Authorization.PermissionGroups.Interfaces;
using MicroERP.Application.Features.Authorization.Permissions.Validators;
using MicroERP.Domain.Audit;
using MicroERP.Domain.Identity;
using MicroERP.Domin.Identity;
using MicroERP.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace MicroERP.Infrastructure.Services;

public class PermissionGroupService : IPermissionGroupService
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuthorizationManager _authorizationManager;
    private readonly IAuditService _auditService;
    private readonly IPermissionGroupQueries _permissionGroupQueries;
    private readonly ICurrentUserService _currentUserService;

    public PermissionGroupService(
        IApplicationDbContext context,
        IAuthorizationManager authorizationManager,
        IAuditService auditService,
        IPermissionGroupQueries permissionGroupQueries,
        ICurrentUserService currentUserService,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _authorizationManager = authorizationManager;
        _auditService = auditService;
        _permissionGroupQueries = permissionGroupQueries;
        _currentUserService = currentUserService;
        _userManager = userManager;
    }



    //================ GET ALL =================

    public async Task<Result<List<PermissionGroupDto>>> GetAllAsync(
    CancellationToken cancellationToken = default)
    {
        var groups =
            await _permissionGroupQueries.GetAllAsync(
                cancellationToken);

        return Result<List<PermissionGroupDto>>
            .Succeeded(groups);
    }


    //================ GET BY ID =================

    public async Task<Result<PermissionGroupDto>> GetByIdAsync(int id,
    CancellationToken cancellationToken = default)
    {
        var group =
            await _permissionGroupQueries.GetByIdAsync(
                id,
                cancellationToken);

        if (group is null)
        {
            return Result<PermissionGroupDto>
                .Failure("Permission group not found.");
        }

        return Result<PermissionGroupDto>
            .Succeeded(group);
    }


    //================ CREATE =================

    public async Task<Result> CreateAsync(
     CreatePermissionGroupDto dto,
     CancellationToken cancellationToken = default)
    {
        return await ExecuteInTransaction(async () =>
        {
            var name = dto.Name.Trim();
            var key = dto.Key.Trim();

            var existsName =
                await _permissionGroupQueries.ExistsByNameAsync(
                    name,
                    cancellationToken);

            if (existsName)
                return Result.Failure(
                    "Permission group name already exists.");

            var existsKey =
                await _permissionGroupQueries.ExistsByKeyAsync(
                    key,
                    cancellationToken);

            if (existsKey)
                return Result.Failure(
                    "Permission group key already exists.");

            var group = new PermissionGroup
            {
                Name = name,
                Key = key,
                Description = dto.Description?.Trim(),
                IsSystem = false,
                IsRequired = false
            };

            _context.PermissionGroups.Add(group);

            await _auditService.LogAsync(
                AuditActions.Create,
                nameof(PermissionGroup),
                group.Id.ToString(),
                null,
                new
                {
                    group.Key,
                    group.Name,
                    group.Description,
                    group.IsSystem,
                    group.IsRequired
                },
                cancellationToken);

            return Result.Succeeded(
                "Permission group created successfully.");

        }, cancellationToken);
    }


    //================ Add Permissions =================

    public async Task<Result> AddPermissionsToGroupAsync(
      int groupId,
      AddPermissionsToGroupDto dto,
      CancellationToken cancellationToken = default)
    {
        return await ExecuteInTransaction(
            async () =>
            {
                var group =
                    await _permissionGroupQueries.GetById(
                        groupId,
                        cancellationToken);

                if (group is null)
                    return Result.Failure(
                        "Permission group not found.");

                // System Groups لا يتم تعديلها إلا بواسطة SuperAdmin
                if (group.IsSystem &&
                    !await IsCurrentUserSuperAdminAsync())
                {
                    return Result.Failure(
                        "You are not allowed to modify a system permission group.");
                }

                var permissionsResult =
                    await GetPermissionsAsync(
                        dto.PermissionKeys,
                        cancellationToken);

                if (!permissionsResult.Success)
                    return permissionsResult.ToResult();

                var permissions =
                    permissionsResult.Data!;

                var existingPermissionIds =
                    await _context.PermissionGroupPermissions
                        .Where(x =>
                            x.PermissionGroupId == groupId)
                        .Select(x => x.PermissionId)
                        .ToListAsync(cancellationToken);

                var newPermissions =
                    permissions
                        .Where(x =>
                            !existingPermissionIds.Contains(x.Id))
                        .Select(x =>
                            new PermissionGroupPermission
                            {
                                PermissionGroupId = groupId,
                                PermissionId = x.Id
                            })
                        .ToList();

                if (newPermissions.Count == 0)
                {
                    return Result.Failure(
                        "All permissions are already assigned to this group.");
                }

                _context.PermissionGroupPermissions
                    .AddRange(newPermissions);

                var addedPermissionKeys =
                    permissions
                        .Where(x =>
                            newPermissions.Any(np =>
                                np.PermissionId == x.Id))
                        .Select(x => x.Key)
                        .ToList();

                await _auditService.LogAsync(
                    AuditActions.Update,
                    nameof(PermissionGroup),
                    group.Id.ToString(),
                    null,
                    new
                    {
                        AddedPermissions =
                            addedPermissionKeys
                    },
                    cancellationToken: cancellationToken);

                await InvalidatePermissionGroupCacheAsync(
                    group.Id);

                return Result.Succeeded(
                    "Permissions added successfully.");
            },
            cancellationToken);
    }

    public async Task<Result> MovePermissionsBetweenGroupsAsync(
      MovePermissionsBetweenGroupsDto dto,
      CancellationToken cancellationToken = default)
    {
        return await ExecuteInTransaction(async () =>
        {
            var sourceGroupKey = dto.SourceGroupKey.Trim();
            var targetGroupKey = dto.TargetGroupKey.Trim();

            if (string.Equals(
                    sourceGroupKey,
                    targetGroupKey,
                    StringComparison.OrdinalIgnoreCase))
            {
                return Result.Failure(
                    "Source and target permission groups must be different.");
            }

            var sourceGroup =
                await _context.PermissionGroups
                    .FirstOrDefaultAsync(
                        x => x.Key == sourceGroupKey,
                        cancellationToken);

            if (sourceGroup is null)
            {
                return Result.Failure(
                    "Source permission group not found.");
            }

            var targetGroup =
                await _context.PermissionGroups
                    .FirstOrDefaultAsync(
                        x => x.Key == targetGroupKey,
                        cancellationToken);

            if (targetGroup is null)
            {
                return Result.Failure(
                    "Target permission group not found.");
            }

            var isSuperAdmin =
                await IsCurrentUserSuperAdminAsync();

            if ((sourceGroup.IsSystem || targetGroup.IsSystem) &&
                !isSuperAdmin)
            {
                return Result.Failure(
                    "System permission groups can only be modified by SuperAdmin.");
            }

            var permissionKeys =
                dto.PermissionKeys
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x))
                    .Select(x =>
                        x.Trim())
                    .Distinct(
                        StringComparer.OrdinalIgnoreCase)
                    .ToList();

            if (permissionKeys.Count == 0)
            {
                return Result.Failure(
                    "At least one permission is required.");
            }

            var permissions =
                await _context.Permissions
                    .Where(x =>
                        permissionKeys.Contains(x.Key))
                    .ToListAsync(cancellationToken);

            if (permissions.Count != permissionKeys.Count)
            {
                return Result.Failure(
                    "One or more permissions are invalid.");
            }

            var permissionIds =
                permissions
                    .Select(x => x.Id)
                    .ToList();

            // التأكد أن جميع البرمشنز موجودة في القروب المصدر
            var sourcePermissions =
                await _context.PermissionGroupPermissions
                    .Where(x =>
                        x.PermissionGroupId == sourceGroup.Id &&
                        permissionIds.Contains(x.PermissionId))
                    .ToListAsync(cancellationToken);

            if (sourcePermissions.Count != permissionIds.Count)
            {
                return Result.Failure(
                    "One or more permissions are not assigned to the source group.");
            }

            // التأكد أن البرمشنز غير موجودة مسبقاً في القروب الهدف
            var existingInTarget =
                await _context.PermissionGroupPermissions
                    .Where(x =>
                        x.PermissionGroupId == targetGroup.Id &&
                        permissionIds.Contains(x.PermissionId))
                    .Select(x =>
                        x.PermissionId)
                    .ToListAsync(cancellationToken);

            if (existingInTarget.Count > 0)
            {
                var duplicatedKeys =
                    permissions
                        .Where(x =>
                            existingInTarget.Contains(x.Id))
                        .Select(x =>
                            x.Key)
                        .ToList();

                return Result.Failure(
                    "The following permissions already exist in the target group: " +
                    string.Join(", ", duplicatedKeys));
            }

            // إضافة البرمشنز إلى القروب الهدف
            var targetPermissions =
                permissions
                    .Select(permission =>
                        new PermissionGroupPermission
                        {
                            PermissionGroupId = targetGroup.Id,
                            PermissionId = permission.Id
                        })
                    .ToList();

            _context.PermissionGroupPermissions
                .AddRange(targetPermissions);

            // حذف البرمشنز من القروب المصدر
            _context.PermissionGroupPermissions
                .RemoveRange(sourcePermissions);

            await _auditService.LogAsync(
     AuditActions.Update,
     nameof(PermissionGroup),
     sourceGroup.Id.ToString(),
     new
     {
         Group = sourceGroup.Key,
         Permissions = permissionKeys
     },
     new
     {
         Group = targetGroup.Key,
         Permissions = permissionKeys
     },
     cancellationToken: cancellationToken);

            await InvalidatePermissionGroupCacheAsync(
                sourceGroup.Id);

            await InvalidatePermissionGroupCacheAsync(
                targetGroup.Id);

            return Result.Succeeded(
                "Permissions moved successfully.");

        }, cancellationToken);
    }

    //================ RemovePermissions =================

    public async Task<Result> RemovePermissionsFromGroupAsync(int groupId,
        RemovePermissionsFromGroupDto dto,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteInTransaction(async () =>
        {
            var group =
                await _permissionGroupQueries.GetById(
                    groupId,
                    cancellationToken);

            if (group is null)
            {
                return Result.Failure(
                    "Permission group not found.");
            }

            if (group.IsSystem &&
        !await IsCurrentUserSuperAdminAsync())
            {
                return Result.Failure(
                    "System permission groups can only be modified by SuperAdmin.");
            }

            var permissionKeys = dto.PermissionKeys
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (permissionKeys.Count == 0)
            {
                return Result.Failure(
                    "At least one permission is required.");
            }

            var permissions =
                await _context.Permissions
                    .Where(x => permissionKeys.Contains(x.Key))
                    .ToListAsync(cancellationToken);

            if (permissions.Count == 0)
            {
                return Result.Failure(
                    "No permissions found.");
            }

            var permissionIds =
                permissions
                    .Select(x => x.Id)
                    .ToList();

            var groupPermissions =
                await _context.PermissionGroupPermissions
                    .Where(x =>
                        x.PermissionGroupId == groupId &&
                        permissionIds.Contains(x.PermissionId))
                    .ToListAsync(cancellationToken);

            if (groupPermissions.Count == 0)
            {
                return Result.Failure(
                    "Permissions are not assigned to this group.");
            }

            _context.PermissionGroupPermissions
                .RemoveRange(groupPermissions);

            var removedPermissionIds =
                groupPermissions
                    .Select(x => x.PermissionId)
                    .ToHashSet();

            var removedPermissionKeys =
                permissions
                    .Where(x => removedPermissionIds.Contains(x.Id))
                    .Select(x => x.Key)
                    .ToList();

            await _auditService.LogAsync(
                AuditActions.Update,
                nameof(PermissionGroup),
                group.Id.ToString(),
                new
                {
                    RemovedPermissions = removedPermissionKeys
                },
                null,
                cancellationToken);

            await InvalidatePermissionGroupCacheAsync(
                group.Id);

            return Result.Succeeded(
                "Permissions removed successfully.");

        }, cancellationToken);
    }

    public async Task<Result> RemoveAllPermissionsFromGroupAsync(int groupId,
    CancellationToken cancellationToken = default)
    {
        return await ExecuteInTransaction(async () =>
        {
            var group =
                await _permissionGroupQueries.GetById(
                    groupId,
                    cancellationToken);

            if (group is null)
            {
                return Result.Failure(
                    "Permission group not found.");
            }

            if (group.IsSystem &&
      !await IsCurrentUserSuperAdminAsync())
            {
                return Result.Failure(
                    "System permission groups can only be modified by SuperAdmin.");
            }

            var groupPermissions =
                await _context.PermissionGroupPermissions
                    .Include(x => x.Permission)
                    .Where(x =>
                        x.PermissionGroupId == groupId)
                    .ToListAsync(cancellationToken);

            if (groupPermissions.Count == 0)
            {
                return Result.Failure(
                    "No permissions are assigned to this group.");
            }

            var removedPermissions =
                groupPermissions
                    .Select(x => x.Permission.Key)
                    .ToList();

            _context.PermissionGroupPermissions
                .RemoveRange(groupPermissions);

            await _auditService.LogAsync(
                AuditActions.Update,
                nameof(PermissionGroup),
                group.Id.ToString(),
                new
                {
                    RemovedPermissions = removedPermissions
                },
                null,
                cancellationToken);

            await InvalidatePermissionGroupCacheAsync(
                group.Id);

            return Result.Succeeded(
                "All permissions removed successfully.");

        }, cancellationToken);
    }

    //================ UPDATE =================

    public async Task<Result> UpdateAsync(int id,
     UpdatePermissionGroupDto dto,
     CancellationToken cancellationToken = default)
    {
        return await ExecuteInTransaction(async () =>
        {
            var group =
                await _permissionGroupQueries.GetById(
                    id,
                    cancellationToken);

            if (group is null)
            {
                return Result.Failure(
                    "Permission group not found.");
            }

            // System Groups لا يمكن تعديلها
            if (group.IsSystem)
            {
                return Result.Failure(
                    "System permission groups cannot be modified.");
            }

            var oldValues = new
            {
                group.Key,
                group.Name,
                group.Description
            };

            // Update Key
            if (!string.IsNullOrWhiteSpace(dto.Key))
            {
                var key = dto.Key.Trim();

                var existsKey =
                    await _permissionGroupQueries
                        .ExistsByKeyAsync(
                            key,
                            id,
                            cancellationToken);

                if (existsKey)
                {
                    return Result.Failure(
                        "Permission group key already exists.");
                }

                group.Key = key;
            }

            // Update Name
            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                var name = dto.Name.Trim();

                var existsName =
                    await _permissionGroupQueries
                        .ExistsByNameAsync(
                            name,
                            id,
                            cancellationToken);

                if (existsName)
                {
                    return Result.Failure(
                        "Permission group name already exists.");
                }

                group.Name = name;
            }

            // Update Description
            if (dto.Description != null)
            {
                group.Description =
                    dto.Description.Trim();
            }

            await _auditService.LogAsync(
                AuditActions.Update,
                nameof(PermissionGroup),
                group.Id.ToString(),
                oldValues,
                new
                {
                    group.Key,
                    group.Name,
                    group.Description
                },
                cancellationToken);

            await InvalidatePermissionGroupCacheAsync(
                group.Id);

            return Result.Succeeded(
                "Permission group updated successfully.");

        }, cancellationToken);
    }
    //================ DELETE =================

    public async Task<Result> DeleteAsync(int id,
     CancellationToken cancellationToken = default)
    {
        return await ExecuteInTransaction(async () =>
        {
            var group =
                await _permissionGroupQueries.GetById(
                    id,
                    cancellationToken);

            if (group is null)
            {
                return Result.Failure(
                    "Permission group not found.");
            }

            // System Groups لا يمكن حذفها
            if (group.IsSystem)
            {
                return Result.Failure(
                    "System permission groups cannot be deleted.");
            }

            // لا يمكن حذف Group مرتبط بمستخدمين
            var assigned =
                await _context.UserPermissionAssignments
                    .AnyAsync(
                        x => x.PermissionGroupId == id,
                        cancellationToken);

            if (assigned)
            {
                return Result.Failure(
                    "Permission group is assigned to users.");
            }

            var oldValues = new
            {
                group.Key,
                group.Name,
                group.Description,
                group.IsSystem,
                group.IsRequired,

                Permissions =
                    await _context.PermissionGroupPermissions
                        .Where(x =>
                            x.PermissionGroupId == group.Id)
                        .Select(x => x.Permission.Key)
                        .ToListAsync(cancellationToken)
            };

            // Soft Delete
            group.IsDeleted = true;
            group.IsActive = false;

            await _auditService.LogAsync(
                AuditActions.Delete,
                nameof(PermissionGroup),
                group.Id.ToString(),
                oldValues,
                null,
                cancellationToken);

            await InvalidatePermissionGroupCacheAsync(
                group.Id);

            return Result.Succeeded(
                "Permission group deleted successfully.");

        }, cancellationToken);
    }


    //================ HELPERS =================

    public async Task<List<LookupDto>> GetLookupAsync(
    CancellationToken cancellationToken = default)
    {
        return await _context.PermissionGroups
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new LookupDto
            {
                Value = x.Key,
                Text = x.Name
            })
            .ToListAsync(cancellationToken);
    }

    private async Task<Result<List<Permission>>> GetPermissionsAsync(
        IEnumerable<string> keys,
        CancellationToken cancellationToken = default)
    {
        var distinctKeys = keys
            .Where(x =>
                !string.IsNullOrWhiteSpace(x))
            .Select(x =>
                x.Trim())
            .Distinct(
                StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (distinctKeys.Count == 0)
        {
            return Result<List<Permission>>
                .Failure(
                    "At least one permission is required.");
        }

        var permissions =
            await _context.Permissions
                .Where(x =>
                    distinctKeys.Contains(x.Key))
                .ToListAsync(cancellationToken);

        if (permissions.Count != distinctKeys.Count)
        {
            return Result<List<Permission>>
                .Failure(
                    "One or more permissions are invalid.");
        }

        return Result<List<Permission>>
            .Succeeded(permissions);
    }

    private async Task InvalidatePermissionGroupCacheAsync(int permissionGroupId)
    {
        // المستخدمين المرتبطين مباشرة
        var userIds = await _context.UserPermissionAssignments
            .Where(x => x.PermissionGroupId == permissionGroupId)
            .Select(x => x.UserId)
            .Distinct()
            .ToListAsync();


        await _authorizationManager
            .ClearUsersPermissionsCacheAsync(userIds);
        
    }

    private async Task<Result> ExecuteInTransaction(
    Func<Task<Result>> action,
    CancellationToken cancellationToken = default)
    {
        var strategy =
            _context.Database
                .CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction =
                await _context.Database
                    .BeginTransactionAsync(cancellationToken);

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

    private async Task<bool> IsCurrentUserSuperAdminAsync()
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
            return false;

        return await _userManager.IsInRoleAsync(
            new ApplicationUser { Id = userId },
            SystemRoles.SuperAdmin);
    }
}