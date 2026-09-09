
using MicroERP.Application.Common.DTOs;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Employees.DTOs;

namespace MicroERP.Application.Features.Employees.Interfaces
{

    public interface IEmployeeService
    {
        Task<Result<CreateEmployeeResultDto>> CreateAsync(CreateEmployeeDto dto,CancellationToken cancellationToken = default);
        Task<Result<EmployeeDto>> GetByIdAsync(int id,CancellationToken cancellationToken = default);
        Task<Result<List<EmployeeListDto>>> GetAllAsync();
        Task<Result<List<EmployeeListDto>>> GetDeletedAsync();
        Task<Result> TransferEmployeeAsync(int employeeId,TransferEmployeeDto dto,CancellationToken cancellationToken = default);
        Task<Result> UpdateSalaryAsync(int employeeId,UpdateEmployeeSalaryDto dto,CancellationToken cancellationToken = default);
        Task<Result> UpdateAsync(int id,UpdateEmployeeDto dto,CancellationToken cancellationToken = default);
        Task<Result> AssignWorkScheduleToEmployeeAsync(int employeeId, int workScheduleId,
        CancellationToken cancellationToken = default);
        Task<Result> AssignWorkScheduleToDepartmentAsync(int departmentId, int workScheduleId,
        CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(int id,CancellationToken cancellationToken = default);
        Task<Result> RestoreAsync(int id,CancellationToken cancellationToken = default);
        Task<Result> ActivateAsync(int id,CancellationToken cancellationToken = default);
        Task<Result> DeactivateAsync(int id,CancellationToken cancellationToken = default);
        Task<Result> ChangeStatusAsync(int id,ChangeEmployeeStatusDto dto,CancellationToken cancellationToken = default);
        Task<Result<List<LookupDto>>> GetLookupAsync(CancellationToken cancellationToken = default);
    }


}
