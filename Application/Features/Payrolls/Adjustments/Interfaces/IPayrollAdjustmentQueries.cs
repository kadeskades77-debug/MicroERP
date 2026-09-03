using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Payrolls.Adjustments.DTOs;

namespace MicroERP.Application.Features.Payrolls.Adjustments.Interfaces;

public interface IPayrollAdjustmentQueries
{
    Task<Result<PagedResult<PayrollAdjustmentDto>>> GetAllAsync(
        PayrollAdjustmentFilterDto filter,
        PagedRequest request,
        CancellationToken cancellationToken);


    Task<Result<PayrollAdjustmentDto>> GetByIdAsync(int id,
        CancellationToken cancellationToken);
}