using Domin.Entities;
using MicroERP.Application.Authorization.Interfaces;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.UserPermissions.DTOs;
using MicroERP.Application.Features.UserPermissions.Interfaces;
using MicroERP.Domain.Audit;
using MicroERP.Domin.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace MicroERP.Infrastructure.Services;

public class UserPermissionAssignmentService
    : IUserPermissionAssignmentService
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuthorizationManager _authorizationManager;
    private readonly IAuditService _auditService;

    public UserPermissionAssignmentService(
        IApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IAuthorizationManager authorizationManager,
        IAuditService auditService)
    {
        _context = context;
        _userManager = userManager;
        _authorizationManager = authorizationManager;
        _auditService = auditService;
    }



    public async Task<Result<List<string>>> GetUserPermissionGroupsAsync(string userId)
    {
        var exists = await _userManager.Users
            .AnyAsync(x => x.Id == userId);

        if (!exists)
            return Result<List<string>>
                .Failure("User not found.");


        var groups = await _context.UserPermissionAssignments
            .Where(x => x.UserId == userId)
            .Select(x => x.PermissionGroup.Key)
            .ToListAsync();


        return Result<List<string>>
            .Succeeded(groups);
    }
    public async Task<Result> AddPermissionGroupAsync(string userId,string permissionGroupKey)
    {
        return await ExecuteInTransaction(async () =>
        {
            var userExists = await _userManager.Users
                .AnyAsync(x => x.Id == userId);

            if (!userExists)
                return Result.Failure("User not found.");


            var group = await _context.PermissionGroups
                .FirstOrDefaultAsync(x =>
                    x.Key == permissionGroupKey);


            if (group is null)
                return Result.Failure(
                    "Permission group not found.");



            var exists = await _context.UserPermissionAssignments
                .AnyAsync(x =>
                    x.UserId == userId &&
                    x.PermissionGroupId == group.Id);


            if (exists)
                return Result.Failure(
                    "Permission group already assigned.");



            _context.UserPermissionAssignments.Add(
                new UserPermissionAssignment
                {
                    UserId = userId,
                    PermissionGroupId = group.Id
                });
            await _auditService.LogAsync(
           AuditActions.AssignPermissionGroup,
           nameof(ApplicationUser),
           userId,
           null,
           new
           {
               PermissionGroup = permissionGroupKey
           });

            await _authorizationManager
                .ClearUserPermissionsCacheAsync(userId);


            return Result.Succeeded(
                "Permission group added successfully.");
        });
    }
    public async Task<Result> ReplacePermissionGroupsAsync(UpdateUserPermissionGroupsDto dto)
    {
        return await ExecuteInTransaction(async () =>
        {
            var userExists = await _userManager.Users
                .AnyAsync(x => x.Id == dto.UserId);


            if (!userExists)
                return Result.Failure("User not found.");


            var keys = dto.PermissionGroupKeys
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();



            var groups = await _context.PermissionGroups
                .Where(x => keys.Contains(x.Key))
                .ToListAsync();


            if (groups.Count != keys.Count)
                return Result.Failure(
                    "One or more permission groups are invalid.");



            await _context.UserPermissionAssignments
                .Where(x => x.UserId == dto.UserId)
                .ExecuteDeleteAsync();

            var oldGroups = await _context.UserPermissionAssignments
           .Where(x => x.UserId == dto.UserId)
           .Select(x => x.PermissionGroup.Key)
           .ToListAsync();

            _context.UserPermissionAssignments.AddRange(
                groups.Select(x =>
                    new UserPermissionAssignment
                    {
                        UserId = dto.UserId,
                        PermissionGroupId = x.Id
                    }));
            var permissionGroupKeys = await _context.UserPermissionAssignments
         .Where(x => x.UserId == dto.UserId)
         .Select(x => x.PermissionGroup.Key)
         .ToListAsync();
            await _auditService.LogAsync(
               AuditActions.ReplacePermissionGroup,
               nameof(ApplicationUser),
               dto.UserId,
               new
               {
                   PermissionGroups = oldGroups
               },
               new
               {
                   PermissionGroups = permissionGroupKeys
               });
            await _authorizationManager
                .ClearUserPermissionsCacheAsync(dto.UserId);


            return Result.Succeeded(
                "User permission groups updated successfully.");
        });
    }
    public async Task<Result> RemovePermissionGroupFromUserAsync(RemoveUserPermissionGroupDto dto)
    {
        return await ExecuteInTransaction(async () =>
        {
            var key = dto.PermissionGroupKey.Trim();

            var assignment = await _context.UserPermissionAssignments
                .Include(x => x.PermissionGroup)
                .FirstOrDefaultAsync(x =>
                    x.UserId == dto.UserId &&
                    x.PermissionGroup.Key == key);

            if (assignment is null)
                return Result.Failure(
                    "Permission group is not assigned to this user.");

            if (assignment.PermissionGroup.Key == "Employee")
                return Result.Failure(
                    "Employee permission group cannot be removed from users.");

            var oldValues = new
            {
                assignment.UserId,
                PermissionGroup = assignment.PermissionGroup.Key
            };

            _context.UserPermissionAssignments.Remove(assignment);

            await _auditService.LogAsync(
                AuditActions.Delete,
                nameof(UserPermissionAssignment),
                $"{assignment.UserId}-{assignment.PermissionGroup.Key}",
                oldValues,
                null);

            await _authorizationManager
                .ClearUserPermissionsCacheAsync(dto.UserId);

            return Result.Succeeded(
                "Permission group removed successfully.");
        });
    }
    private async Task<Result> ExecuteInTransaction(Func<Task<Result>> action)
    {
        var strategy = _context.Database
            .CreateExecutionStrategy();


        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();


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