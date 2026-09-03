using MicroERP.Application.Common.DTOs;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Authorization.PermissionGroups.DTOs;

namespace MicroERP.Application.Features.Authorization.PermissionGroups.Interfaces;

public interface IPermissionGroupService
{
    Task<Result<List<PermissionGroupDto>>> GetAllAsync();
    Task<Result<PermissionGroupDto>> GetByIdAsync(int id);
    Task<Result> CreateAsync(CreatePermissionGroupDto dto);
    Task<Result> AddPermissionsToGroupAsync(int groupId,AddPermissionsToGroupDto dto);
    Task<Result> RemovePermissionsFromGroupAsync(int groupId,RemovePermissionsFromGroupDto dto);
    Task<Result> UpdateAsync(int id, UpdatePermissionGroupDto dto);
    Task<Result> DeleteAsync(int id);
    Task<List<LookupDto>> GetLookupAsync();
}