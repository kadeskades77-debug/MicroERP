

using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Payrolls.DTOs;

namespace MicroERP.Application.Features.Payrolls.Interfaces
{
    public interface IPayrollService
    {
        Task<Result<PayrollPeriodDto>> CreatePeriodAsync(
            CreatePayrollPeriodDto dto,
            CancellationToken cancellationToken);

        Task<Result> UpdatePeriodAsync(int id,
         UpdatePayrollPeriodDto dto,
         CancellationToken cancellationToken = default);

        Task<Result> DeletePeriodAsync(int id,
        CancellationToken cancellationToken = default);

        Task<Result> GeneratePayrollAsync(
            int payrollPeriodId,
            CancellationToken cancellationToken);

        Task<Result> ApproveAsync(int payrollId, string userId,
            CancellationToken cancellationToken);

        Task<Result> ApprovePeriodAsync(
        int payrollPeriodId,
        string userId,
        CancellationToken cancellationToken);

        Task<Result> MarkAsPaidAsync(int payrollId,string userId,
            CancellationToken cancellationToken);

        Task<Result> MarkPeriodAsPaidAsync(
        int payrollPeriodId,
        string userId,
        CancellationToken cancellationToken);

        Task<Result<bool>> ClosePeriodAsync(int periodId,
    CancellationToken cancellationToken);
    }
}
