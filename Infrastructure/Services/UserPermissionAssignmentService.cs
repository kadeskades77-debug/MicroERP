using MicroERP.Application.Authorization.Interfaces;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.Authorization.UserPermissions.DTOs;
using MicroERP.Application.Features.Authorization.UserPermissions.Interfaces;
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



    public async Task<Result<List<string>>> GetUserPermissionGroupsAsync(
    string userId,
    CancellationToken cancellationToken = default)
    {
        var exists = await _userManager.Users
            .AnyAsync(
                x => x.Id == userId,
                cancellationToken);

        if (!exists)
        {
            return Result<List<string>>
                .Failure("User not found.");
        }

        var groups = await _context.UserPermissionAssignments
            .Where(x => x.UserId == userId)
            .Select(x => x.PermissionGroup.Key)
            .ToListAsync(cancellationToken);

        return Result<List<string>>
            .Succeeded(groups);
    }
    public async Task<Result> AddPermissionGroupsAsync(
    AddUserPermissionGroupsDto dto,
    CancellationToken cancellationToken = default)
{
    return await ExecuteInTransaction(async () =>
    {
        // =========================================================
        // Validate User
        // =========================================================

        var userExists = await _userManager.Users
            .AnyAsync(
                x => x.Id == dto.UserId,
                cancellationToken);

        if (!userExists)
            return Result.Failure("User not found.");

        // =========================================================
        // Normalize Permission Group Keys
        // =========================================================

        var keys = dto.PermissionGroupKeys
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (keys.Count == 0)
            return Result.Failure(
                "At least one permission group is required.");

        // =========================================================
        // Get Permission Groups
        // =========================================================

        var groups = await _context.PermissionGroups
            .Where(x => keys.Contains(x.Key))
            .ToListAsync(cancellationToken);

        if (groups.Count != keys.Count)
            return Result.Failure(
                "One or more permission groups are invalid.");

        // =========================================================
        // Check Existing Assignments
        // =========================================================

        var groupIds = groups
            .Select(x => x.Id)
            .ToList();

        var existingGroupIds = await _context
            .UserPermissionAssignments
            .Where(x =>
                x.UserId == dto.UserId &&
                groupIds.Contains(x.PermissionGroupId))
            .Select(x => x.PermissionGroupId)
            .ToListAsync(cancellationToken);

        if (existingGroupIds.Count > 0)
        {
            var existingGroups = groups
                .Where(x => existingGroupIds.Contains(x.Id))
                .Select(x => x.Key)
                .ToList();

            return Result.Failure(
                $"Permission groups already assigned: " +
                $"{string.Join(", ", existingGroups)}.");
        }

        // =========================================================
        // Add Assignments
        // =========================================================

        _context.UserPermissionAssignments.AddRange(
            groups.Select(x =>
                new UserPermissionAssignment
                {
                    UserId = dto.UserId,
                    PermissionGroupId = x.Id
                }));

        // =========================================================
        // Audit
        // =========================================================

        await _auditService.LogAsync(
            AuditActions.AssignPermissionGroup,
            nameof(ApplicationUser),
            dto.UserId,
            null,
            new
            {
                PermissionGroups = groups
                    .Select(x => x.Key)
                    .ToList()
            },
            cancellationToken);

        // =========================================================
        // Clear Authorization Cache
        // =========================================================

        await _authorizationManager
            .ClearUserPermissionsCacheAsync(dto.UserId);

        return Result.Succeeded(
            "Permission groups added successfully.");

    }, cancellationToken);
}   
    public async Task<Result> ReplacePermissionGroupsAsync(
    UpdateUserPermissionGroupsDto dto,
    CancellationToken cancellationToken = default)
    {
        return await ExecuteInTransaction(async () =>
        {
            // =========================================================
            // Validate User
            // =========================================================

            var userExists = await _userManager.Users
                .AnyAsync(
                    x => x.Id == dto.UserId,
                    cancellationToken);

            if (!userExists)
                return Result.Failure("User not found.");

            // =========================================================
            // Normalize Permission Group Keys
            // =========================================================

            var keys = dto.PermissionGroupKeys
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            // =========================================================
            // Employee Permission Group is mandatory
            // =========================================================

            if (!keys.Any(x =>
                x.Equals(
                    "Employee",
                    StringComparison.OrdinalIgnoreCase)))
            {
                return Result.Failure(
                    "Employee permission group is required.");
            }

            // =========================================================
            // Validate Permission Groups
            // =========================================================

            var groups = await _context.PermissionGroups
                .Where(x => keys.Contains(x.Key))
                .ToListAsync(cancellationToken);

            if (groups.Count != keys.Count)
            {
                return Result.Failure(
                    "One or more permission groups are invalid.");
            }

            // =========================================================
            // Get old permission groups before deleting them
            // =========================================================

            var oldGroups = await _context.UserPermissionAssignments
                .Where(x => x.UserId == dto.UserId)
                .Select(x => x.PermissionGroup.Key)
                .ToListAsync(cancellationToken);

            // =========================================================
            // Remove old assignments
            // =========================================================

            await _context.UserPermissionAssignments
                .Where(x => x.UserId == dto.UserId)
                .ExecuteDeleteAsync(cancellationToken);

            // =========================================================
            // Add new assignments
            // =========================================================

            _context.UserPermissionAssignments.AddRange(
                groups.Select(x =>
                    new UserPermissionAssignment
                    {
                        UserId = dto.UserId,
                        PermissionGroupId = x.Id
                    }));

            // =========================================================
            // New groups for audit
            // =========================================================

            var newGroups = groups
                .Select(x => x.Key)
                .ToList();

            // =========================================================
            // Audit
            // =========================================================

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
                    PermissionGroups = newGroups
                },
                cancellationToken);

            // =========================================================
            // Clear authorization cache
            // =========================================================

            await _authorizationManager
                .ClearUserPermissionsCacheAsync(
                    dto.UserId);

            return Result.Succeeded(
                "User permission groups updated successfully.");
        }, cancellationToken);
    }
    public async Task<Result> RemovePermissionGroupFromUserAsync(
    RemoveUserPermissionGroupDto dto,
    CancellationToken cancellationToken = default)
    {
        return await ExecuteInTransaction(async () =>
        {
            var key = dto.PermissionGroupKey.Trim();

            var assignment = await _context.UserPermissionAssignments
                .Include(x => x.PermissionGroup)
                .FirstOrDefaultAsync(
                    x =>
                        x.UserId == dto.UserId &&
                        x.PermissionGroup.Key == key,
                    cancellationToken);

            if (assignment is null)
            {
                return Result.Failure(
                    "Permission group is not assigned to this user.");
            }

            if (assignment.PermissionGroup.Key.Equals(
                    "Employee",
                    StringComparison.OrdinalIgnoreCase))
            {
                return Result.Failure(
                    "Employee permission group cannot be removed from users.");
            }

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
                null,
                cancellationToken);

            await _authorizationManager
                .ClearUserPermissionsCacheAsync(dto.UserId);

            return Result.Succeeded(
                "Permission group removed successfully.");

        }, cancellationToken);
    }

    private async Task<Result> ExecuteInTransaction(
    Func<Task<Result>> action,
    CancellationToken cancellationToken = default)
    {
        var strategy = _context.Database
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
                    await transaction.RollbackAsync(cancellationToken);
                    return result;
                }

                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return result;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }
}