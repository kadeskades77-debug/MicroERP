using MicroERP.Application.Authorization.Interfaces;
using MicroERP.Application.Common.DTOs;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.PermissionGroups.DTOs;
using MicroERP.Application.Features.PermissionGroups.Interfaces;
using MicroERP.Domain.Audit;
using MicroERP.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
namespace MicroERP.Infrastructure.Services;

public class PermissionGroupService : IPermissionGroupService
{
    private readonly IApplicationDbContext _context;
    private readonly IAuthorizationManager _authorizationManager;
    private readonly IAuditService _auditService;
    private readonly IPermissionGroupQueries _permissionGroupQueries;

    public PermissionGroupService(
        IApplicationDbContext context,
        IAuthorizationManager authorizationManager,
        IAuditService auditService,
        IPermissionGroupQueries permissionGroupQueries)
    {
        _context = context;
        _authorizationManager = authorizationManager;
        _auditService = auditService;
        _permissionGroupQueries = permissionGroupQueries;
    }



    //================ GET ALL =================

    public async Task<Result<List<PermissionGroupDto>>> GetAllAsync()
    {
        var groups = await _permissionGroupQueries.GetAllAsync();


        return Result<List<PermissionGroupDto>>
            .Succeeded(groups);
    }


    //================ GET BY ID =================

    public async Task<Result<PermissionGroupDto>> GetByIdAsync(int id)
    {
        var group = await _permissionGroupQueries.GetByIdAsync(id);


        if (group is null)
            return Result<PermissionGroupDto>
                .Failure("Permission group not found.");


        return Result<PermissionGroupDto>
            .Succeeded(group);
    }


    //================ CREATE =================

    public async Task<Result> CreateAsync(CreatePermissionGroupDto dto)
    {
        return await ExecuteInTransaction(async () =>
        {
            var name = dto.Name.Trim();
            var key = dto.Key.Trim();



            var existsName =
               await _permissionGroupQueries.ExistsByNameAsync(name);

            if (existsName)
                return Result.Failure(
                    "Permission group name already exists.");



            var existsKey =
                await _permissionGroupQueries.ExistsByKeyAsync(key);


            if (existsKey)
                return Result.Failure(
                    "Permission group key already exists.");


            var group = new PermissionGroup
            {
                Name = name,
                Key = key,
                Description = dto.Description?.Trim(),
                IsSystem = false
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
     });


            return Result.Succeeded(
                "Permission group created successfully.");
        });
    }


    //================ Add Permissions =================

    public async Task<Result> AddPermissionsToGroupAsync(int groupId,AddPermissionsToGroupDto dto)
    {
        return await ExecuteInTransaction(async () =>
        {
            var group = await _permissionGroupQueries.GetById(groupId);

            if (group is null)
                return Result.Failure(
                    "Permission group not found.");
            if (group.Key == "SuperAdmin")
            {
                return Result.Failure(
                    "You cannot add permissions manually to SuperAdmin group.");
            }
            var permissionsResult =
                await GetPermissionsAsync(dto.PermissionKeys);

            if (!permissionsResult.Success)
                return permissionsResult.ToResult();

            var permissions = permissionsResult.Data!;

            var permissionIds = permissions
                .Select(x => x.Id)
                .ToList();

            // التحقق أن الـ Permission غير مرتبطة بقروب آخر
            // مع استثناء SuperAdmin
            var duplicatedPermissions =
                await _context.PermissionGroupPermissions
                .Where(x =>
                    permissionIds.Contains(x.PermissionId) &&
                    x.PermissionGroupId != groupId &&
                    x.PermissionGroup.Key != "SuperAdmin")
                .Select(x => new
                {
                    x.Permission.Key,
                    GroupName = x.PermissionGroup.Name
                })
                .ToListAsync();

            if (duplicatedPermissions.Any())
            {
                return Result.Failure(
                    "The following permissions are already assigned to another group: " +
                    string.Join(", ",
                        duplicatedPermissions.Select(x =>
                            $"{x.Key} ({x.GroupName})")));
            }

            var existingPermissionIds =
                await _context.PermissionGroupPermissions
                .Where(x =>
                    x.PermissionGroupId == groupId)
                .Select(x => x.PermissionId)
                .ToListAsync();

            var newPermissions = permissions
                .Where(x =>
                    !existingPermissionIds.Contains(x.Id))
                .Select(x =>
                    new PermissionGroupPermission
                    {
                        PermissionGroupId = groupId,
                        PermissionId = x.Id
                    })
                .ToList();

            if (!newPermissions.Any())
                return Result.Failure(
                    "All permissions are already assigned to this group.");

            _context.PermissionGroupPermissions
                .AddRange(newPermissions);

            await _auditService.LogAsync(
                AuditActions.Update,
                nameof(PermissionGroup),
                group.Id.ToString(),
                null,
                new
                {
                    AddedPermissions = dto.PermissionKeys
                });

            await InvalidatePermissionGroupCacheAsync(group.Id);

            return Result.Succeeded(
                "Permissions added successfully.");
        });
    }   
    
    //================ RemovePermissions =================

    public async Task<Result> RemovePermissionsFromGroupAsync(int groupId,RemovePermissionsFromGroupDto dto)
    {
        return await ExecuteInTransaction(async () =>
        {
            var group =
               await _permissionGroupQueries.GetById(groupId);


            if (group is null)
                return Result.Failure(
                    "Permission group not found.");


            if (group.IsSystem)
                return Result.Failure(
                    "System permission groups cannot be modified.");



            var permissions =
                await _context.Permissions
                .Where(x =>
                    dto.PermissionKeys.Contains(x.Key))
                .ToListAsync();



            if (!permissions.Any())
                return Result.Failure(
                    "No permissions found.");



            var permissionIds =
                permissions
                .Select(x => x.Id)
                .ToList();



            var groupPermissions =
                await _context.PermissionGroupPermissions
                .Where(x =>
                    x.PermissionGroupId == groupId &&
                    permissionIds.Contains(x.PermissionId))
                .ToListAsync();



            if (!groupPermissions.Any())
                return Result.Failure(
                    "Permissions are not assigned to this group.");



            _context.PermissionGroupPermissions
                .RemoveRange(groupPermissions);



            await _auditService.LogAsync(
                AuditActions.Update,
                nameof(PermissionGroup),
                group.Id.ToString(),
                new
                {
                    RemovedPermissions =
                        dto.PermissionKeys
                },
                null);



            await InvalidatePermissionGroupCacheAsync(
                group.Id);



            return Result.Succeeded(
                "Permissions removed successfully.");

        });
    }

    //================ UPDATE =================

    public async Task<Result> UpdateAsync(int id,UpdatePermissionGroupDto dto)
    {
        return await ExecuteInTransaction(async () =>
        {
            var group =
               await _permissionGroupQueries.GetById(id);


            if (group is null)
                return Result.Failure(
                    "Permission group not found.");


            if (group.IsSystem)
                return Result.Failure(
                    "System permission groups cannot be modified.");


            var oldValues = new
            {
                group.Key,
                group.Name,
                group.Description,

                Permissions = await _context.PermissionGroupPermissions
                    .Where(x => x.PermissionGroupId == group.Id)
                    .Select(x => x.Permission.Key)
                    .ToListAsync()
            };



            // Update Key
            if (!string.IsNullOrWhiteSpace(dto.Key))
            {
                var key = dto.Key.Trim();


                var existsKey =
                    await _context.PermissionGroups
                    .AnyAsync(x =>
                        x.Id != id &&
                        x.Key.ToUpper() == key.ToUpper());


                if (existsKey)
                    return Result.Failure(
                        "Permission group key already exists.");


                group.Key = key;
            }



            // Update Name
            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                var name = dto.Name.Trim();


                var existsName =
                    await _context.PermissionGroups
                    .AnyAsync(x =>
                        x.Id != id &&
                        x.Name.ToUpper() == name.ToUpper());


                if (existsName)
                    return Result.Failure(
                        "Permission group name already exists.");


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
                    group.Description,
                });



            await InvalidatePermissionGroupCacheAsync(
                group.Id);



            return Result.Succeeded(
                "Permission group updated successfully.");

        });
    }

    //================ DELETE =================

    public async Task<Result> DeleteAsync(int id)
    {
        return await ExecuteInTransaction(async () =>
        {
            var group =
               await _permissionGroupQueries.GetById(id); ;



            if (group is null)
                return Result.Failure(
                    "Permission group not found.");



            if (group.IsSystem)
                return Result.Failure(
                    "System permission groups cannot be deleted.");



            var assigned =
                await _context.UserPermissionAssignments
                .AnyAsync(x =>
                    x.PermissionGroupId == id);



            if (assigned)
            {
                return Result.Failure(
                    "Permission group is assigned to users.");
            }

            var oldValues = new
            {
                group.Key,
                group.Name,

                Permissions = await _context.PermissionGroupPermissions
        .Where(x => x.PermissionGroupId == group.Id)
        .Select(x => x.Permission.Key)
        .ToListAsync()
            };

            group.IsDeleted = true;
            group.IsActive = false;
;

            await _auditService.LogAsync(
                AuditActions.Delete,
                nameof(PermissionGroup),
                group.Id.ToString(),
                oldValues,
                null);

            await InvalidatePermissionGroupCacheAsync(
              group.Id);
            return Result.Succeeded(
                "Permission group deleted successfully.");
        });
    }


    //================ HELPERS =================

    public async Task<List<LookupDto>> GetLookupAsync()
    {
        return await _context.PermissionGroups
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new LookupDto
            {
                Value = x.Key,
                Text = x.Name
            })
            .ToListAsync();
    }

    private async Task<Result<List<Permission>>>GetPermissionsAsync(IEnumerable<string> keys)
    {
        var distinctKeys = keys
            .Where(x =>
                !string.IsNullOrWhiteSpace(x))
            .Select(x =>
                x.Trim())
            .Distinct(
                StringComparer.OrdinalIgnoreCase)
            .ToList();



        var permissions =
            await _context.Permissions
            .Where(x =>
                distinctKeys.Contains(x.Key))
            .ToListAsync();



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

    private async Task UpdatePermissionGroupPermissionsAsync(int permissionGroupId,IEnumerable<Permission> permissions)
    {
        await _context.PermissionGroupPermissions
            .Where(x =>
                x.PermissionGroupId == permissionGroupId)
            .ExecuteDeleteAsync();



        _context.PermissionGroupPermissions
            .AddRange(
                permissions.Select(permission =>
                    new PermissionGroupPermission
                    {
                        PermissionGroupId = permissionGroupId,
                        PermissionId = permission.Id
                    }));
    }

    private async Task<Result> ExecuteInTransaction(Func<Task<Result>> action)
    {
        var strategy =
            _context.Database
            .CreateExecutionStrategy();



        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction =
                await _context.Database
                .BeginTransactionAsync();



            try
            {
                var result = await action();



                if (!result.Success)
                    return result;



                await _context.SaveChangesAsync();



                await transaction.CommitAsync();



                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }
}