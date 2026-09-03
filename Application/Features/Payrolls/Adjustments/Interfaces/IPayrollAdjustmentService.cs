using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Payrolls.Adjustments.DTOs;
using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Payrolls.Adjustments.Interfaces;

public interface IPayrollAdjustmentService
{
    Task<Result<PayrollAdjustmentDto>> CreateAsync(
        CreatePayrollAdjustmentDto dto,
        CancellationToken cancellationToken);


    Task<Result<List<PayrollAdjustmentDto>>>
           CreateForAllEmployeesAsync(
               CreatePayrollAdjustmentForAllEmployeesDto dto,
               CancellationToken cancellationToken = default);


    Task<Result<PayrollAdjustmentDto>> UpdateAsync(
        int id,
        UpdatePayrollAdjustmentDto dto,
        CancellationToken cancellationToken);


    Task<Result> DeleteAsync(
        int id,
        CancellationToken cancellationToken);
}