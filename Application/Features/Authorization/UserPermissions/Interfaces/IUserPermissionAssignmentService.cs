using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Authorization.UserPermissions.DTOs;

namespace MicroERP.Application.Features.Authorization.UserPermissions.Interfaces;

public interface IUserPermissionAssignmentService
{
    Task<Result<List<string>>> GetUserPermissionGroupsAsync(string userId);

    Task<Result> ReplacePermissionGroupsAsync(UpdateUserPermissionGroupsDto dto);
    Task<Result> AddPermissionGroupAsync(string userId,string permissionGroupKey);

    Task<Result> RemovePermissionGroupFromUserAsync(RemoveUserPermissionGroupDto dto);
}