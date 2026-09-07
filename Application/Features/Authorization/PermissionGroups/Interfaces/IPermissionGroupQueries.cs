using MicroERP.Application.Features.Authorization.PermissionGroups.DTOs;
using MicroERP.Domain.Identity;

namespace MicroERP.Application.Features.Authorization.PermissionGroups.Interfaces
{
    public interface IPermissionGroupQueries
    {
        Task<PermissionGroup?> GetById(
            int id,
            CancellationToken cancellationToken = default);

        Task<PermissionGroupDto> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<List<PermissionGroupDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<bool> ExistsByKeyAsync(
            string key,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsByKeyAsync(
            string key,
            int excludeId,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsByNameAsync(
            string name,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsByNameAsync(
            string name,
            int excludeId,
            CancellationToken cancellationToken = default);
    }
}