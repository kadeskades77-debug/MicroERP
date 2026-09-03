

namespace MicroERP.Application.Features.Payrolls.Reports.Interfaces
{
    public interface IEmployeePayslipPdfService
    {
        Task<byte[]> GenerateAsync(
            int payrollId,
            CancellationToken cancellationToken = default);
    }
}
