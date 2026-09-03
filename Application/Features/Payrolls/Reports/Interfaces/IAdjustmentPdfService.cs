

using MicroERP.Application.Features.Payrolls.Reports.DTOs;

namespace MicroERP.Application.Features.Payrolls.Reports.Interfaces
{
    public interface IAdjustmentPdfService
    {
        public Task<byte[]> GenerateAsync(
       AdjustmentReportFilterDto filter,
       CancellationToken cancellationToken = default);
    }
}
