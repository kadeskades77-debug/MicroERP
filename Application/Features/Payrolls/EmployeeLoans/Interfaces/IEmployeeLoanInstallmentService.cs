using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Payrolls.EmployeeLoans.DTOs;

namespace MicroERP.Application.Features.Payrolls.EmployeeLoans.Interfaces;

public interface IEmployeeLoanInstallmentService
{
    Task<Result<IReadOnlyList<EmployeeLoanInstallmentDto>>> GetByLoanIdAsync(
        int loanId,
        CancellationToken cancellationToken = default);

    Task<Result<EmployeeLoanInstallmentDto>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);
}
