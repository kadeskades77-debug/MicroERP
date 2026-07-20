using MicroERP.Application.Common.DTOs;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Permissions.DTOs;

namespace MicroERP.Application.Features.Permissions.Interfaces
{
    public interface IPermissionService
    {
        Task<Result<List<PermissionDto>>> GetAllAsync();

        Task<Result<PermissionDto>> GetByIdAsync(int id);

        Task<Result<List<PermissionDto>>> GetAvailablePermissionsAsync();

        Task<Result> CreateAsync(CreatePermissionDto dto);

        Task<Result> UpdateAsync(int id, UpdatePermissionDto dto);

        Task<Result> DeleteAsync(int id);
        Task<List<LookupDto>> GetLookupAsync();
    }
}
