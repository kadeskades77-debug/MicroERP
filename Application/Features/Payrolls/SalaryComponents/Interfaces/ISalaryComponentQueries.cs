
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Payrolls.SalaryComponents.DTOs;

namespace MicroERP.Application.Features.Payrolls.SalaryComponents.Interfaces
{
    public interface ISalaryComponentQueries
    {
        Task<Result<List<SalaryComponentDto>>> GetAllAsync(
            CancellationToken cancellationToken);

        Task<Result<SalaryComponentDto>> GetByIdAsync(
            int id,
            CancellationToken cancellationToken);
    }
}
