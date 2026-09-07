using MicroERP.Application.Authorization.Interfaces;
using MicroERP.Application.Common.DTOs;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Mappings;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.Authorization.Permissions.DTOs;
using MicroERP.Application.Features.Authorization.Permissions.Interfaces;
using MicroERP.Application.Features.Authorization.Permissions.Validators;
using MicroERP.Domain.Audit;
using MicroERP.Domain.Identity;
using MicroERP.Domin.Identity;
using MicroERP.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace MicroERP.Infrastructure.Services;

public class PermissionService : IPermissionService
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuditService _auditService;
    private readonly IAuthorizationManager _authorizationManager;
    private readonly IPermissionQueries _permissionQueries;
    private readonly ICurrentUserService _currentUserService;
    public PermissionService(
        IApplicationDbContext context, IAuditService auditService, IAuthorizationManager authorizationManager, IPermissionQueries permissionQueries, ICurrentUserService currentUserService, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _auditService = auditService;
        _authorizationManager = authorizationManager;
        _permissionQueries = permissionQueries;
        _currentUserService = currentUserService;
        _userManager = userManager;
    }


    //================ GET ALL =================
    public async Task<Result<List<PermissionDto>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var permissions =
            await _permissionQueries.GetAllAsync(
                cancellationToken);

        return Result<List<PermissionDto>>
            .Succeeded(permissions);
    }


    //================ GET AVAILABLE PERMISSIONS =================
    public async Task<Result<List<PermissionDto>>> GetAvailablePermissionsAsync(
      int groupId,
      CancellationToken cancellationToken = default)
    {
        var permissions =
            await _permissionQueries.GetAvailablePermissionsAsync(
                groupId,
                cancellationToken);

        return Result<List<PermissionDto>>
            .Succeeded(permissions);
    }

    //================ GET BY ID =================
    public async Task<Result<PermissionDto>> GetByIdAsync(int id,
        CancellationToken cancellationToken = default)
    {
        var permission =
            await _permissionQueries.GetByIdAsync(
                id,
                cancellationToken);

        if (permission is null)
        {
            return Result<PermissionDto>
                .Failure(
                    "Permission not found.");
        }

        return Result<PermissionDto>
            .Succeeded(permission);
    }


    //================ UPDATE =================
    public async Task<Result> UpdateAsync(int id,
     UpdatePermissionDto dto,
     CancellationToken cancellationToken = default)
    {
        return await ExecuteInTransaction(
            async () =>
            {
                var isSuperAdmin =
                    await IsCurrentUserSuperAdminAsync();

                if (!isSuperAdmin)
                {
                    return Result.Failure(
                        "Only SuperAdmin can update permissions.");
                }

                var permission =
                    await _permissionQueries.GetById(
                        id,
                        cancellationToken);

                if (permission is null)
                {
                    return Result.Failure(
                        "Permission not found.");
                }

                var oldValues = new
                {
                    permission.Key,
                    permission.Name,
                    permission.Description
                };

                if (!string.IsNullOrWhiteSpace(dto.Name))
                {
                    var name = dto.Name.Trim();

                    var nameExists =
                        await _permissionQueries
                            .ExistsByNameAsync(
                                name,
                                id,
                                cancellationToken);

                    if (nameExists)
                    {
                        return Result.Failure(
                            "Permission name already exists.");
                    }

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
                    },
                    cancellationToken);

                return Result.Succeeded(
                    "Permission updated successfully.");
            },
            cancellationToken);
    }

    //================ Get Lookup =================
    public async Task<List<LookupDto>> GetLookupAsync(
     CancellationToken cancellationToken = default)
    {
        return await _context.Permissions
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new LookupDto
            {
                Value = x.Key,
                Text = x.Name
            })
            .ToListAsync(cancellationToken);
    }


    //================ HELPERS =================

    private async Task<Result> ExecuteInTransaction(
     Func<Task<Result>> action,
     CancellationToken cancellationToken = default)
    {
        var strategy =
            _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(
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