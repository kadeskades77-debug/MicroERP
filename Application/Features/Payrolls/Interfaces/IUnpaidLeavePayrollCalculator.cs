using MicroERP.Domin.Entities.Payrolls;

namespace MicroERP.Application.Features.Payrolls.Interfaces;

public interface IUnpaidLeavePayrollCalculator
{
    Task<List<PayrollItem>> CalculateAsync(
        int employeeId,
        DateOnly startDate,
        DateOnly endDate,
        decimal basicSalary,
        CancellationToken cancellationToken = default);
}