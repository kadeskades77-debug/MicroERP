using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Roles.DTOs;

namespace MicroERP.Application.Features.Roles.Interfaces
{
    public interface IUserRoleService
    {
        Task<Result<List<UserRoleDto>>> GetUserRolesAsync(string userId);

        Task<Result<List<RoleLookupDto>>> GetAvailableRolesAsync();

        Task<Result> AssignRolesAsync(string userId,AssignUserRolesDto dto);

        Task<Result> RemoveRolesAsync(string userId, AssignUserRolesDto dto);
        Task<Result> ReplaceRolesAsync(UpdateUserRolesDto dto);
    }
}
