using MicroERP.Application.Common.DTOs;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Authentication.Roles.DTOs;

namespace MicroERP.Application.Features.Authentication.Roles.Interfaces;

public interface IRoleService
{
    Task<Result<List<RoleDto>>> GetAllAsync();

    Task<Result<RoleDto>> GetByIdAsync(string id);

    Task<Result> CreateAsync(CreateRoleDto dto);

    Task<Result> UpdateAsync(string id, UpdateRoleDto dto);

    Task<Result> DeleteAsync(string id);

    Task<List<LookupDto>> GetLookupAsync();
}