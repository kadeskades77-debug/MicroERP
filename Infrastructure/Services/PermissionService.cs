using MicroERP.Application.Authorization.Interfaces;
using MicroERP.Application.Common.DTOs;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Mappings;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.Permissions.DTOs;
using MicroERP.Application.Features.Permissions.Interfaces;
using MicroERP.Application.Features.Permissions.Validators;
using MicroERP.Domain.Audit;
using MicroERP.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace MicroERP.Infrastructure.Services;

public class PermissionService : IPermissionService
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;
    private readonly IAuthorizationManager _authorizationManager;
    private readonly IPermissionQueries _permissionQueries;
    public PermissionService(
        IApplicationDbContext context, IAuditService auditService, IAuthorizationManager authorizationManager, IPermissionQueries permissionQueries)
    {
        _context = context;
        _auditService = auditService;
        _authorizationManager = authorizationManager;
        _permissionQueries = permissionQueries;
    }


    //================ GET ALL =================
    public async Task<Result<List<PermissionDto>>> GetAllAsync()
    {
        var permissions = await _permissionQueries.GetAllAsync();

        return Result<List<PermissionDto>>
            .Succeeded(permissions);
    }


    //================ GET Available Permissions =================
    public async Task<Result<List<PermissionDto>>> GetAvailablePermissionsAsync()
    {
        

        var permissions = await _permissionQueries.GetAvailablePermissionsAsync();


        return Result<List<PermissionDto>>
            .Succeeded(permissions);
    }

    //================ GET BY ID =================
    public async Task<Result<PermissionDto>> GetByIdAsync(int id)
    {
        var permission = await _permissionQueries.GetByIdAsync(id);
        

        if (permission is null)
            return Result<PermissionDto>
                .Failure("Permission not found.");


        return Result<PermissionDto>
            .Succeeded(permission);
    }



    //================ CREATE =================
    public async Task<Result> CreateAsync(CreatePermissionDto dto)
    {
        return await ExecuteInTransaction(async () =>
        {
            var key = dto.Key.Trim();

            var exists = await _permissionQueries.ExistsByKeyAsync(key);

            if (exists)
                return Result.Failure(
                    "Permission key already exists.");


            var nameExists = await _permissionQueries.ExistsByNameAsync(dto.Name.Trim());

            if (nameExists)
                return Result.Failure(
                    "Permission name already exists.");


            var permission = new Permission
            {
                Key = key,
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim()
            };


            _context.Permissions.Add(permission);

            await _context.SaveChangesAsync();


            var superAdminGroup = await _context.PermissionGroups
            .FirstOrDefaultAsync(x => x.Key == SystemGroups.SuperAdmin);


            if (superAdminGroup == null)
                return Result.Failure(
                    "SuperAdmin group not found.");


            var groupPermission = new PermissionGroupPermission
            {
                PermissionGroupId = superAdminGroup.Id,
                PermissionId = permission.Id
            };


            _context.PermissionGroupPermissions
                .Add(groupPermission);



            await _auditService.LogAsync(
                AuditActions.Create,
                nameof(Permission),
                permission.Id.ToString(),
                null,
                new
                {
                    permission.Key,
                    permission.Name,
                    permission.Description
                });


            return Result.Succeeded(
                "Permission created successfully.");
        });
    }


    //================ UPDATE =================
    public async Task<Result> UpdateAsync(int id,UpdatePermissionDto dto)
    {
        return await ExecuteInTransaction(async () =>
        {
            var permission = await _permissionQueries.GetById(id);

            if (permission is null)
                return Result.Failure(
                    "Permission not found.");


            var affectedUserIds = await _context.PermissionGroupPermissions
                .Where(x => x.PermissionId == id)
                .SelectMany(x =>
                    _context.UserPermissionAssignments
                        .Where(u =>
                            u.PermissionGroupId == x.PermissionGroupId)
                        .Select(u => u.UserId))
                .Distinct()
                .ToListAsync();


            var oldValues = new
            {
                permission.Key,
                permission.Name,
                permission.Description
            };


            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                var name = dto.Name.Trim();

                var nameExists =await _permissionQueries.ExistsByNameAsync(name,id);

                if (nameExists)
                    return Result.Failure(
                        "Permission name already exists.");

                permission.Name = name;
            }


            if (dto.Description != null)
            {
                permission.Description =
                    dto.Description.Trim();
            }


            await _auditService.LogAsync(
                AuditActions.Update,
                nameof(Permission),
                permission.Id.ToString(),
                oldValues,
                new
                {
                    permission.Key,
                    permission.Name,
                    permission.Description
                });


            if (affectedUserIds.Any())
            {
                await _authorizationManager
                    .ClearUsersPermissionsCacheAsync(
                        affectedUserIds);
            }


            return Result.Succeeded(
                "Permission updated successfully.");
        });
    }

    //================ DELETE =================
    public async Task<Result> DeleteAsync(int id)
    {
        return await ExecuteInTransaction(async () =>
        {
            var permission = await _permissionQueries.GetById(id);

            if (permission is null)
                return Result.Failure(
                    "Permission not found.");


            var affectedUserIds = await _context.PermissionGroupPermissions
                .Where(x => x.PermissionId == id)
                .SelectMany(x =>
                    _context.UserPermissionAssignments
                        .Where(u =>
                            u.PermissionGroupId == x.PermissionGroupId)
                        .Select(u => u.UserId))
                .Distinct()
                .ToListAsync();


            var oldValues = new
            {
                permission.Key,
                permission.Name,
                permission.Description
            };


            permission.IsDeleted = true;


            await _auditService.LogAsync(
                AuditActions.Delete,
                nameof(Permission),
                permission.Id.ToString(),
                oldValues,
                null);


            if (affectedUserIds.Any())
            {
                await _authorizationManager
                    .ClearUsersPermissionsCacheAsync(affectedUserIds);
            }


            return Result.Succeeded(
                "Permission deleted successfully.");
        });
    }


    //================ HELPERS =================

    private async Task<Result> ExecuteInTransaction(
        Func<Task<Result>> action)
    {
        var strategy =
            _context.Database.CreateExecutionStrategy();


        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();


            try
            {
                var result = await action();


                if (!result.Success)
                {
                    await transaction.RollbackAsync();
                    return result;
                }


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

    public async Task<List<LookupDto>> GetLookupAsync()
    {
        return await _context.Permissions
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new LookupDto
            {
                Value = x.Key,
                Text = x.Name
            })
            .ToListAsync();
    }

}