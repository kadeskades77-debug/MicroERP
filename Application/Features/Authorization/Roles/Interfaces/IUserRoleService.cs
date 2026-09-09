using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Authorization.Roles.DTOs;

namespace MicroERP.Application.Features.Authorization.Roles.Interfaces
{
    public interface IUserRoleService
    {
        Task<Result<List<UserRoleDto>>> GetUserRolesAsync(
            string userId,
            CancellationToken cancellationToken = default);

        Task<Result<List<RoleLookupDto>>> GetAvailableRolesAsync(
            CancellationToken cancellationToken = default);

        Task<Result> AssignRolesAsync(
            string userId,
            AssignUserRolesDto dto,
            CancellationToken cancellationToken = default);

        Task<Result> RemoveRolesAsync(
            string userId,
            AssignUserRolesDto dto,
            CancellationToken cancellationToken = default);

        Task<Result> ReplaceRolesAsync(
            UpdateUserRolesDto dto,
            CancellationToken cancellationToken = default);
    }
}
