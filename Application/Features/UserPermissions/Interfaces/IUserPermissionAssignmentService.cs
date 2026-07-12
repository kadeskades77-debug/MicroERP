using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.UserPermissions.DTOs;

namespace MicroERP.Application.Features.UserPermissions.Interfaces;

public interface IUserPermissionAssignmentService
{
    Task<Result<List<string>>> GetUserPermissionGroupsAsync(string userId);

    Task<Result> ReplacePermissionGroupsAsync(UpdateUserPermissionGroupsDto dto);
    Task<Result> AddPermissionGroupAsync(string userId,string permissionGroupKey);

    Task<Result> RemovePermissionGroupFromUserAsync(RemoveUserPermissionGroupDto dto);
}