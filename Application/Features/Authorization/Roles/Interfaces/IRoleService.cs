using MicroERP.Application.Common.DTOs;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Authentication.Roles.DTOs;

namespace MicroERP.Application.Features.Authentication.Roles.Interfaces;

public interface IRoleService
{
    Task<Result<List<RoleDto>>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Result<RoleDto>> GetByIdAsync(
        string id,
        CancellationToken cancellationToken = default);

    Task<Result> CreateAsync(
        CreateRoleDto dto,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(
        string id,
        UpdateRoleDto dto,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(
        string id,
        CancellationToken cancellationToken = default);

    Task<List<LookupDto>> GetLookupAsync(
        CancellationToken cancellationToken = default);
}