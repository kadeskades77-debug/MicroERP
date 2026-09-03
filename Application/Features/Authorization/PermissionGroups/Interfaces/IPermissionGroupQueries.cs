using MicroERP.Application.Features.Authorization.PermissionGroups.DTOs;
using MicroERP.Domain.Identity;

namespace MicroERP.Application.Features.Authorization.PermissionGroups.Interfaces
{
    public interface IPermissionGroupQueries
    {
        Task<PermissionGroup?> GetById(int id);

        Task<PermissionGroupDto> GetByIdAsync(int id);

        Task<List<PermissionGroupDto>> GetAllAsync();

        Task<PermissionGroup?> GetByKeyAsync(string key);

        Task<PermissionGroup?> GetByNameAsync(string name);

        Task<PermissionGroup?> GetByIdWithPermissionsAsync(int id);

        Task<bool> ExistsByKeyAsync(string key);

        Task<bool> ExistsByKeyAsync(string key, int excludeId);

        Task<bool> ExistsByNameAsync(string name);

        Task<bool> ExistsByNameAsync(string name, int excludeId);
    }
}