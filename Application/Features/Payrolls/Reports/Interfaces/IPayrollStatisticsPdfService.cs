using MicroERP.Application.Features.Payrolls.Reports.DTOs;

namespace MicroERP.Application.Features.Payrolls.Reports.Interfaces
{
    public interface IPayrollStatisticsPdfService
    {
        Task<byte[]> GenerateAsync(
            PayrollStatisticsFilterDto filter,
            CancellationToken cancellationToken = default);
    }
}
