

using MicroERP.Application.Features.Payrolls.Reports.DTOs;

namespace MicroERP.Application.Features.Payrolls.Reports.Interfaces
{
    public interface IPayrollDetailsPdfService
    {
        Task<byte[]> GenerateAsync(
            PayrollDetailsFilterDto filter,
            CancellationToken cancellationToken = default);
    }
}
