using MicroERP.Application.Authorization.Interfaces;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.Authorization.Roles.DTOs;
using MicroERP.Application.Features.Authorization.Roles.Interfaces;
using MicroERP.Domain.Audit;
using MicroERP.Domain.Identity;
using MicroERP.Domin.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace MicroERP.Infrastructure.Services;

public class UserRoleService : IUserRoleService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IAuthorizationManager _authorizationManager;
    private readonly IAuditService _auditService;
    private readonly IApplicationDbContext _context;

    public UserRoleService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IAuthorizationManager authorizationManager,
        IAuditService auditService,
        IApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _authorizationManager = authorizationManager;
        _auditService = auditService;
        _context = context;
    }


    //================ GET USER ROLES =================

    public async Task<Result<List<UserRoleDto>>> GetUserRolesAsync(string userId,
        CancellationToken cancellationToken = default)
    {
        var userExists = await _userManager.Users
            .AnyAsync(
                x => x.Id == userId,
                cancellationToken);

        if (!userExists)
            return Result<List<UserRoleDto>>
                .Failure("User not found.");

        var roles = await _context.UserRoles
            .Where(x => x.UserId == userId)
            .Join(
                _roleManager.Roles,
                userRole => userRole.RoleId,
                role => role.Id,
                (userRole, role) => new UserRoleDto
                {
                    Id = role.Id,
                    Name = role.Name!
                })
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return Result<List<UserRoleDto>>
            .Succeeded(roles);
    }


    //================ AVAILABLE ROLES =================

    public async Task<Result<List<RoleLookupDto>>> GetAvailableRolesAsync(
        CancellationToken cancellationToken = default)
    {
        var roles = await _context.Roles
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new RoleLookupDto
            {
                Id = x.Id,
                Name = x.Name!
            })
            .ToListAsync(cancellationToken);

        return Result<List<RoleLookupDto>>
            .Succeeded(roles);
    }


    //================ ASSIGN =================

    public async Task<Result> AssignRolesAsync(string userId,
        AssignUserRolesDto dto,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure("User not found.");

        var roleIds = dto.RoleIds
            .Distinct()
            .ToList();

        if (roleIds.Count == 0)
            return Result.Failure("At least one role is required.");

        var roles = await _roleManager.Roles
            .Where(x => roleIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        if (roles.Count != roleIds.Count)
            return Result.Failure("One or more roles are invalid.");

        var oldRoleNames =
            await _userManager.GetRolesAsync(user);

        var currentRoleNames =
            oldRoleNames.ToHashSet(
                StringComparer.OrdinalIgnoreCase);

        var rolesToAdd = roles
            .Where(x =>
                !currentRoleNames.Contains(x.Name!))
            .ToList();

        if (rolesToAdd.Count == 0)
            return Result.Succeeded(
                "User already has the specified roles.");

        var roleNamesToAdd = rolesToAdd
            .Select(x => x.Name!)
            .ToList();

        var result =
            await _userManager.AddToRolesAsync(
                user,
                roleNamesToAdd);

        if (!result.Succeeded)
            return HandleIdentityResult(result);

        var newRoleNames =
            await _userManager.GetRolesAsync(user);

        await _auditService.LogAsync(
            AuditActions.AssignRole,
            nameof(ApplicationUser),
            user.Id,
            new
            {
                Roles = oldRoleNames
            },
            new
            {
                Roles = newRoleNames
            },
            cancellationToken);

        await _authorizationManager
            .ClearUserPermissionsCacheAsync(userId);

        return Result.Succeeded(
            "Roles assigned successfully.");
    }
    //================ REPLACE =================

    public async Task<Result> ReplaceRolesAsync(
        UpdateUserRolesDto dto,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);

        if (user is null)
            return Result.Failure("User not found.");

        var requestedRoleIds = dto.RoleIds
            .Distinct()
            .ToList();

        var roles = await _roleManager.Roles
            .Where(x => requestedRoleIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        if (roles.Count != requestedRoleIds.Count)
            return Result.Failure(
                "One or more roles are invalid.");

        var oldRoleNames =
            await _userManager.GetRolesAsync(user);

        var currentRoleNames =
            oldRoleNames.ToHashSet(
                StringComparer.OrdinalIgnoreCase);

        var requestedRoleNames = roles
            .Select(x => x.Name!)
            .ToHashSet(
                StringComparer.OrdinalIgnoreCase);

        var rolesToRemove = currentRoleNames
            .Except(requestedRoleNames)
            .ToList();

        var rolesToAdd = requestedRoleNames
            .Except(currentRoleNames)
            .ToList();

        if (rolesToRemove.Count > 0)
        {
            var removeResult =
                await _userManager.RemoveFromRolesAsync(
                    user,
                    rolesToRemove);

            if (!removeResult.Succeeded)
                return HandleIdentityResult(removeResult);
        }

        if (rolesToAdd.Count > 0)
        {
            var addResult =
                await _userManager.AddToRolesAsync(
                    user,
                    rolesToAdd);

            if (!addResult.Succeeded)
                return HandleIdentityResult(addResult);
        }

        var newRoleNames =
            await _userManager.GetRolesAsync(user);

        await _authorizationManager
            .ClearUserPermissionsCacheAsync(dto.UserId);

        await _auditService.LogAsync(
            AuditActions.ReplaceRole,
            nameof(ApplicationUser),
            user.Id,
            new
            {
                Roles = oldRoleNames
            },
            new
            {
                Roles = newRoleNames
            },
            cancellationToken);

        return Result.Succeeded(
            "User roles updated successfully.");
    }

    //================ REMOVE =================

    public async Task<Result> RemoveRolesAsync(
        string userId,
        AssignUserRolesDto dto,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure("User not found.");

        var roleIds = dto.RoleIds
            .Distinct()
            .ToList();

        if (roleIds.Count == 0)
            return Result.Failure("At least one role is required.");

        var roles = await _roleManager.Roles
            .Where(x => roleIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        if (roles.Count != roleIds.Count)
            return Result.Failure(
                "One or more roles are invalid.");

        var oldRoleNames =
            await _userManager.GetRolesAsync(user);

        var currentRoleNames =
            oldRoleNames.ToHashSet(
                StringComparer.OrdinalIgnoreCase);

        var rolesToRemove = roles
            .Where(x =>
                currentRoleNames.Contains(x.Name!))
            .Select(x => x.Name!)
            .ToList();

        if (rolesToRemove.Count == 0)
            return Result.Succeeded(
                "User does not have the specified roles.");

        var result =
            await _userManager.RemoveFromRolesAsync(
                user,
                rolesToRemove);

        if (!result.Succeeded)
            return HandleIdentityResult(result);

        var newRoleNames =
            await _userManager.GetRolesAsync(user);

        await _auditService.LogAsync(
            AuditActions.DeleteUserRole,
            nameof(ApplicationUser),
            user.Id,
            new
            {
                Roles = oldRoleNames
            },
            new
            {
                Roles = newRoleNames
            },
            cancellationToken);

        await _authorizationManager
            .ClearUserPermissionsCacheAsync(userId);

        return Result.Succeeded(
            "Roles removed successfully.");
    }


    //================ HELPERS =================


    private static Result HandleIdentityResult(IdentityResult result)
    {
        if (result.Succeeded)
            return Result.Succeeded();


        return Result.Failure(
            string.Join(
                Environment.NewLine,
                result.Errors.Select(x => x.Description)));
    }
}