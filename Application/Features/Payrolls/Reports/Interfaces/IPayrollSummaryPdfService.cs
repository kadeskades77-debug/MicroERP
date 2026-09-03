using MicroERP.Application.Features.Payrolls.Reports.DTOs;

namespace MicroERP.Application.Features.Payrolls.Reports.Interfaces
{
    public interface IPayrollSummaryPdfService
    {
        Task<byte[]> GenerateAsync(
            PayrollSummaryFilterDto filter,
            CancellationToken cancellationToken = default);
    }
}