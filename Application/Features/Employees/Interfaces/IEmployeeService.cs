
using MicroERP.Application.Common.DTOs;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Employees.DTOs;

namespace MicroERP.Application.Features.Employees.Interfaces
{
    public interface IEmployeeService
    {
        Task<CreateEmployeeResultDto> CreateAsync(CreateEmployeeDto dto,
            CancellationToken cancellationToken = default);
        Task<EmployeeDto> GetByIdAsync(int id);
        Task<Result<List<EmployeeListDto>>> GetAllAsync();
        Task<Result> TransferEmployeeAsync(int employeeId,TransferEmployeeDto dto);
        Task<Result> UpdateSalaryAsync(int employeeId,UpdateEmployeeSalaryDto dto);
        Task<Result> UpdateAsync(int id, UpdateEmployeeDto dto);
        Task DeleteAsync(int id);
        Task RestoreAsync(int id);
        Task ActivateAsync(int id);
        Task DeactivateAsync(int id);
        Task<List<LookupDto>> GetLookupAsync();
    }
}
