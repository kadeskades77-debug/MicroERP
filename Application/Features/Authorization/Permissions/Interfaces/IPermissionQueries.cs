using MicroERP.Application.Features.Authorization.Permissions.DTOs;
using MicroERP.Domain.Identity;
namespace MicroERP.Application.Features.Authorization.Permissions.Interfaces
{
    public interface IPermissionQueries
    {
        Task<Permission?> GetById(
            int id,
            CancellationToken cancellationToken = default);

        Task<PermissionDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<List<PermissionDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<List<PermissionDto>> GetAvailablePermissionsAsync(
      int groupId,
      CancellationToken cancellationToken = default);

        Task<bool> ExistsByNameAsync(
            string name,
            int excludeId,
            CancellationToken cancellationToken = default);
    }
}
