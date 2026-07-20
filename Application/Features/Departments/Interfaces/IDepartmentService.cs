using MicroERP.Application.Common.DTOs;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Departments.DTOs;

namespace MicroERP.Application.Features.Departments.Interfaces
{
    public interface IDepartmentService
    {
        Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto);

        Task<DepartmentDto?> GetByIdAsync(int id);

        Task<Result<List<DepartmentListDto>>> GetAllAsync();

        Task<DepartmentDto> UpdateAsync(int id, UpdateDepartmentDto dto);

        Task<DepartmentDto> AssignManagerAsync(int id, AssignDepartmentManagerDto dto);

        Task<Result> TransferDepartmentManagerAsync(int managerEmployeeId,TransferDepartmentManagerDto dto);

        Task DeleteAsync(int id);
        Task RestoreAsync(int id);
        Task<List<LookupDto>> GetLookupAsync();
    }
}
