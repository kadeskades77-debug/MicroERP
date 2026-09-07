using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Authorization.RolePermissionGroup.DTOs;

namespace MicroERP.Application.Features.Authorization.RolePermissionGroup.Interfaces
{
 

    public interface IRolePermissionGroupService
    {
        Task<Result<RolePermissionGroupDto>> GetRolePermissionGroupsAsync(
            string roleId,
            CancellationToken cancellationToken = default);

        Task<Result> AddPermissionGroupsAsync(
            AddRolePermissionGroupsDto dto,
            CancellationToken cancellationToken = default);

        Task<Result> ReplacePermissionGroupsAsync(
            UpdateRolePermissionGroupsDto dto,
            CancellationToken cancellationToken = default);

        Task<Result> RemovePermissionGroupAsync(
            RemoveRolePermissionGroupDto dto,
            CancellationToken cancellationToken = default);

        Task<Result> RemoveAllPermissionGroupsAsync(
            string roleId,
            CancellationToken cancellationToken = default);
    }
}
