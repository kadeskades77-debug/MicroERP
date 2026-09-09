using MicroERP.Application.Common.DTOs;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.Authorization.Roles.DTOs;
using MicroERP.Application.Features.Authorization.Roles.Interfaces;
using MicroERP.Domain.Audit;
using MicroERP.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Infrastructure.Services;

public class RoleService : IRoleService
{
    private readonly IApplicationDbContext _context;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IAuditService _auditService;

    public RoleService(
        IApplicationDbContext context,
        RoleManager<ApplicationRole> roleManager,
        IAuditService auditService)
    {
        _context = context;
        _roleManager = roleManager;
        _auditService = auditService;
    }

    //================ GET ALL =================

    public async Task<Result<List<RoleDto>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var roles = await _roleManager.Roles
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new RoleDto
            {
                Id = x.Id,
                Name = x.Name!,
                Description = x.Description,
                IsSystem = x.IsSystem,
                PermissionGroupKeys = new List<string>()
            })
            .ToListAsync(cancellationToken);

        return Result<List<RoleDto>>
            .Succeeded(roles);
    }

    //================ GET BY ID =================

    public async Task<Result<RoleDto>> GetByIdAsync(string id,
        CancellationToken cancellationToken = default)
    {
        var role = await _roleManager.Roles
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new RoleDto
            {
                Id = x.Id,
                Name = x.Name!,
                Description = x.Description,
                IsSystem = x.IsSystem,

                PermissionGroupKeys =
                    x.RolePermissionGroups
                        .Select(rg => rg.PermissionGroup.Key)
                        .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (role is null)
            return Result<RoleDto>
                .Failure("Role not found.");

        return Result<RoleDto>
            .Succeeded(role);
    }

    //================ CREATE =================

    public async Task<Result> CreateAsync(
        CreateRoleDto dto,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteInTransaction(
            async () =>
            {
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return Result.Failure(
                        "Role name is required.");

                var name = dto.Name.Trim();

                var normalizedName =
                    _roleManager.NormalizeKey(name);

                var exists =
                    await _roleManager.Roles
                        .AnyAsync(
                            x => x.NormalizedName == normalizedName,
                            cancellationToken);

                if (exists)
                    return Result.Failure(
                        "Role already exists.");

                var role = new ApplicationRole
                {
                    Name = name,
                    Description = dto.Description?.Trim(),
                    IsSystem = false
                };

                var identityResult =
                    await _roleManager.CreateAsync(role);

                var result =
                    HandleIdentityResult(identityResult);

                if (!result.Success)
                    return result;

                await _auditService.LogAsync(
                    AuditActions.Create,
                    nameof(ApplicationRole),
                    role.Id,
                    null,
                    new
                    {
                        role.Name,
                        role.Description
                    },
                    cancellationToken);

                return Result.Succeeded(
                    "Role created successfully.");
            },
            cancellationToken);
    }

    //================ UPDATE =================

    public async Task<Result> UpdateAsync(string id,
        UpdateRoleDto dto,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteInTransaction(
            async () =>
            {
                var role =
                    await _roleManager.Roles
                        .FirstOrDefaultAsync(
                            x => x.Id == id,
                            cancellationToken);

                if (role is null)
                    return Result.Failure(
                        "Role not found.");

                if (role.IsSystem)
                    return Result.Failure(
                        "System roles cannot be modified.");

                var oldPermissionGroups =
                    await _context.RolePermissionGroups
                        .Where(x => x.RoleId == role.Id)
                        .Select(x => x.PermissionGroup.Key)
                        .ToListAsync(cancellationToken);

                var oldValues = new
                {
                    role.Name,
                    role.Description,
                    PermissionGroups = oldPermissionGroups
                };

                // ---------- Name ----------

                if (!string.IsNullOrWhiteSpace(dto.Name))
                {
                    var name = dto.Name.Trim();

                    var normalizedName =
                        _roleManager.NormalizeKey(name);

                    var exists =
                        await _roleManager.Roles
                            .AnyAsync(
                                x =>
                                    x.Id != id &&
                                    x.NormalizedName == normalizedName,
                                cancellationToken);

                    if (exists)
                        return Result.Failure(
                            "Role name already exists.");

                    role.Name = name;
                }

                // ---------- Description ----------

                if (dto.Description != null)
                {
                    role.Description =
                        dto.Description.Trim();
                }

                var identityResult =
                    await _roleManager.UpdateAsync(role);

                var result =
                    HandleIdentityResult(identityResult);

                if (!result.Success)
                    return result;

                var newValues = new
                {
                    role.Name,
                    role.Description,
                    PermissionGroups = oldPermissionGroups
                };

                await _auditService.LogAsync(
                    AuditActions.Update,
                    nameof(ApplicationRole),
                    role.Id,
                    oldValues,
                    newValues,
                    cancellationToken);

                return Result.Succeeded(
                    "Role updated successfully.");
            },
            cancellationToken);
    }

    //================ DELETE =================

    public async Task<Result> DeleteAsync(string id,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteInTransaction(
            async () =>
            {
                var role =
                    await _roleManager.Roles
                        .FirstOrDefaultAsync(
                            x => x.Id == id,
                            cancellationToken);

                if (role is null)
                    return Result.Failure(
                        "Role not found.");

                if (role.IsSystem)
                    return Result.Failure(
                        "System roles cannot be deleted.");

                var permissionGroups =
                    await _context.RolePermissionGroups
                        .Where(x => x.RoleId == role.Id)
                        .Select(x => x.PermissionGroup.Key)
                        .ToListAsync(cancellationToken);

                var oldValues = new
                {
                    role.Name,
                    role.Description,
                    PermissionGroups = permissionGroups
                };

                var hasUsers =
                    await _context.UserRoles
                        .AnyAsync(
                            x => x.RoleId == id,
                            cancellationToken);

                if (hasUsers)
                    return Result.Failure(
                        "Role is assigned to users.");

                var identityResult =
                    await _roleManager.DeleteAsync(role);

                if (!identityResult.Succeeded)
                    return HandleIdentityResult(
                        identityResult);

                await _auditService.LogAsync(
                    AuditActions.Delete,
                    nameof(ApplicationRole),
                    role.Id,
                    oldValues,
                    null,
                    cancellationToken);

                return Result.Succeeded(
                    "Role deleted successfully.");
            },
            cancellationToken);
    }

    //================ LOOKUP =================

    public async Task<List<LookupDto>> GetLookupAsync(
        CancellationToken cancellationToken = default)
    {
        return await _roleManager.Roles
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new LookupDto
            {
                Value = x.Id,
                Text = x.Name!
            })
            .ToListAsync(cancellationToken);
    }

    //================ HELPERS =================

    private static Result HandleIdentityResult(
        IdentityResult result)
    {
        if (result.Succeeded)
            return Result.Succeeded();

        return Result.Failure(
            string.Join(
                Environment.NewLine,
                result.Errors
                    .Select(x => x.Description)));
    }

    private async Task<Result> ExecuteInTransaction(Func<Task<Result>> action,
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