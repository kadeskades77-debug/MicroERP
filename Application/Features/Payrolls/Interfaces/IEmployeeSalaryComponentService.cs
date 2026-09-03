using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Payrolls.DTOs;

namespace MicroERP.Application.Features.Payrolls.Interfaces
{
    public interface IEmployeeSalaryComponentService
    {
        Task<Result<EmployeeSalaryComponentDto>> CreateAsync(
            CreateEmployeeSalaryComponentDto dto,
            CancellationToken cancellationToken);

        Task<Result> UpdateAsync(int id,
       UpdateEmployeeSalaryComponentDto dto,
        CancellationToken cancellationToken = default);

        Task<Result> DeleteAsync(
            int id,
            CancellationToken cancellationToken);
    }
}
