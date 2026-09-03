
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Payrolls.SalaryComponents.DTOs;

namespace MicroERP.Application.Features.Payrolls.SalaryComponents.Interfaces
{
    public interface ISalaryComponentService
    {
        Task<Result<SalaryComponentDto>> CreateAsync(
            CreateSalaryComponentDto dto,
            CancellationToken cancellationToken);


        Task<Result<SalaryComponentDto>> UpdateAsync(
            int id,
            UpdateSalaryComponentDto dto,
            CancellationToken cancellationToken);


        Task<Result> DeleteAsync(
            int id,
            CancellationToken cancellationToken);
    }
}
