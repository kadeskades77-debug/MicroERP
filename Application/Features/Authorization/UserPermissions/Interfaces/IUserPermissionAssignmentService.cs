using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Authorization.UserPermissions.DTOs;

namespace MicroERP.Application.Features.Authorization.UserPermissions.Interfaces;

public interface IUserPermissionAssignmentService
{
    Task<Result<List<string>>> GetUserPermissionGroupsAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> ReplacePermissionGroupsAsync(
        UpdateUserPermissionGroupsDto dto,
        CancellationToken cancellationToken = default);

    Task<Result> AddPermissionGroupsAsync(
        AddUserPermissionGroupsDto dto,
        CancellationToken cancellationToken = default);

    Task<Result> RemovePermissionGroupFromUserAsync(
        RemoveUserPermissionGroupDto dto,
        CancellationToken cancellationToken = default);
    Task<Result> RemoveAllOptionalPermissionGroupsAsync(
    string userId,
    CancellationToken cancellationToken = default);
}