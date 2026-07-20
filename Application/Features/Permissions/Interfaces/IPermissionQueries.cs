using MicroERP.Application.Features.Permissions.DTOs;
using MicroERP.Domain.Identity;
namespace MicroERP.Application.Features.Permissions.Interfaces
{
    public interface IPermissionQueries
    {
        Task<Permission?> GetById(int id);
        Task<PermissionDto>GetByIdAsync(int id);
        Task<List<PermissionDto>> GetAllAsync();
        Task<List<PermissionDto>> GetAvailablePermissionsAsync();
        Task<Permission?> GetByKeyAsync(string key);

        Task<List<Permission>> GetByKeysAsync(IEnumerable<string> keys);

        Task<bool> ExistsByKeyAsync(string key);

        Task<bool> ExistsByKeyAsync(string key, int excludeId);

        Task<bool> ExistsByNameAsync(string name);

        Task<bool> ExistsByNameAsync(string name, int excludeId);
    }
}
