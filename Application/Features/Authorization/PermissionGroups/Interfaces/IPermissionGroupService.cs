using MicroERP.Application.Common.DTOs;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Authorization.PermissionGroups.DTOs;

namespace MicroERP.Application.Features.Authorization.PermissionGroups.Interfaces;

public interface IPermissionGroupService
{
    Task<Result<List<PermissionGroupDto>>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Result<PermissionGroupDto>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Result> CreateAsync(
        CreatePermissionGroupDto dto,
        CancellationToken cancellationToken = default);

    Task<Result> AddPermissionsToGroupAsync(
        int groupId,
        AddPermissionsToGroupDto dto,
        CancellationToken cancellationToken = default);

    Task<Result> MovePermissionsBetweenGroupsAsync(
      MovePermissionsBetweenGroupsDto dto,
      CancellationToken cancellationToken = default);

    Task<Result> RemovePermissionsFromGroupAsync(
        int groupId,
        RemovePermissionsFromGroupDto dto,
        CancellationToken cancellationToken = default);

    Task<Result> RemoveAllPermissionsFromGroupAsync(int groupId,
    CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(
        int id,
        UpdatePermissionGroupDto dto,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<List<LookupDto>> GetLookupAsync(
        CancellationToken cancellationToken = default);
}