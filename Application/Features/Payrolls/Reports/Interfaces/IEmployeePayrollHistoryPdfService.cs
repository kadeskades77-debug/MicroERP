

using MicroERP.Application.Features.Payrolls.Reports.DTOs;

namespace MicroERP.Application.Features.Payrolls.Reports.Interfaces
{
    public interface IEmployeePayrollHistoryPdfService
    {
        Task<byte[]> GenerateAsync(
            EmployeePayrollHistoryFilterDto filter,
            CancellationToken cancellationToken = default);
    }
}
