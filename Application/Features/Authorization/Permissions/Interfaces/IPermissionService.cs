using MicroERP.Application.Common.DTOs;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Authorization.Permissions.DTOs;

namespace MicroERP.Application.Features.Authorization.Permissions.Interfaces
{
    public interface IPermissionService
    {
        Task<Result<List<PermissionDto>>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<Result<PermissionDto>> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<Result<List<PermissionDto>>> GetAvailablePermissionsAsync(
       int groupId,
       CancellationToken cancellationToken = default);

        Task<Result> UpdateAsync(
            int id,
            UpdatePermissionDto dto,
            CancellationToken cancellationToken = default);

        Task<List<LookupDto>> GetLookupAsync(
            CancellationToken cancellationToken = default);
    }
}
