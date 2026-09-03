using MicroERP.Application.Authorization.Interfaces;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.Authentication.Roles.DTOs;
using MicroERP.Application.Features.Authentication.Roles.Interfaces;
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

    public async Task<Result<List<UserRoleDto>>> GetUserRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result<List<UserRoleDto>>
                .Failure("User not found.");


        var roles = await _userManager.GetRolesAsync(user);


        var result = await _roleManager.Roles
            .Where(x => roles.Contains(x.Name!))
            .Select(x => new UserRoleDto
            {
                Id = x.Id,
                Name = x.Name!
            })
            .ToListAsync();


        return Result<List<UserRoleDto>>
            .Succeeded(result);
    }



    //================ AVAILABLE ROLES =================

    public async Task<Result<List<RoleLookupDto>>> GetAvailableRolesAsync()
    {
        var roles = await _roleManager.Roles
            .OrderBy(x => x.Name)
            .Select(x => new RoleLookupDto
            {
                Id = x.Id,
                Name = x.Name!
            })
            .ToListAsync();


        return Result<List<RoleLookupDto>>
            .Succeeded(roles);
    }



    //================ ASSIGN =================

    public async Task<Result> AssignRolesAsync(string userId,AssignUserRolesDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure("User not found.");


        var roles = await GetRolesAsync(dto.RoleIds);
        var oldRoles = await _userManager.GetRolesAsync(user);

        foreach (var role in roles)
        {
            if (!await _userManager.IsInRoleAsync(
                    user,
                    role.Name!))
            {
                var result =
                    await _userManager.AddToRoleAsync(
                        user,
                        role.Name!);


                if (!result.Succeeded)
                    return HandleIdentityResult(result);
            }
        }
        await _auditService.LogAsync(
            AuditActions.AssignRole,
            nameof(ApplicationUser),
            user.Id,
            new
            {
                Roles = oldRoles
            },
            new
            {
                Roles = roles
            });

        await _authorizationManager
            .ClearUserPermissionsCacheAsync(userId);
        await _context.SaveChangesAsync();

        return Result.Succeeded(
            "Roles assigned successfully.");
    }

    //================ REPLACE =================

    public async Task<Result> ReplaceRolesAsync( UpdateUserRolesDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);

        if (user is null)
            return Result.Failure("User not found.");


        var requestedRoleIds = dto.RoleIds
            .Distinct()
            .ToList();


        var roles = await _roleManager.Roles
            .Where(x => requestedRoleIds.Contains(x.Id))
            .ToListAsync();
        var oldRoles = await _userManager.GetRolesAsync(user);

        if (roles.Count != requestedRoleIds.Count)
            return Result.Failure(
                "One or more roles are invalid.");



        var currentRoleNames = await _userManager
            .GetRolesAsync(user);



        var requestedRoleNames = roles
            .Select(x => x.Name!)
            .ToList();



        // Roles to remove
        var rolesToRemove = currentRoleNames
            .Except(requestedRoleNames)
            .ToList();



        // Roles to add
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
        var newRoles = await _userManager.GetRolesAsync(user);


        await _authorizationManager
            .ClearUserPermissionsCacheAsync(dto.UserId);
               await _auditService.LogAsync(
           AuditActions.ReplaceRole,
           nameof(ApplicationUser),
           user.Id,
           new
           {
               Roles = oldRoles
           },
           new
           {
               Roles = newRoles
           });

        await _context.SaveChangesAsync();
        return Result.Succeeded(
            "User roles updated successfully.");

    }   
    //================ REMOVE =================

    public async Task<Result> RemoveRolesAsync(string userId,AssignUserRolesDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure("User not found.");


        var roles = await GetRolesAsync(dto.RoleIds);
        var oldRoles = await _userManager.GetRolesAsync(user);

        foreach (var role in roles)
        {
            if (await _userManager.IsInRoleAsync(
                    user,
                    role.Name!))
            {
                var result =
                    await _userManager.RemoveFromRoleAsync(
                        user,
                        role.Name!);


                if (!result.Succeeded)
                    return HandleIdentityResult(result);
            }
        }


        await _authorizationManager
            .ClearUserPermissionsCacheAsync(userId);
        await _auditService.LogAsync(
              AuditActions.DeleteUserRole,
              nameof(ApplicationUser),
              user.Id,
              new
              {
                  Roles = oldRoles
              },
              null);
        await _context.SaveChangesAsync();
        return Result.Succeeded(
            "Roles removed successfully.");
    }



    //================ HELPERS =================

    private async Task<List<ApplicationRole>> GetRolesAsync(IEnumerable<string> roleIds)
    {
        var roles = await _roleManager.Roles
            .Where(x => roleIds.Contains(x.Id))
            .ToListAsync();


        if (roles.Count != roleIds.Count())
            throw new InvalidOperationException(
                "One or more roles are invalid.");


        return roles;
    }

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