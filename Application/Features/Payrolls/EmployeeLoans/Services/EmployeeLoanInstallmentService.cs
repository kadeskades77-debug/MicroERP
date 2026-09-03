
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Payrolls.EmployeeLoans.DTOs;
using MicroERP.Application.Features.Payrolls.EmployeeLoans.Interfaces;


namespace MicroERP.Application.Features.Payrolls.EmployeeLoans.Services;

public class EmployeeLoanInstallmentService : IEmployeeLoanInstallmentService
{
    private readonly IApplicationDbContext _context;

    public EmployeeLoanInstallmentService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IReadOnlyList<EmployeeLoanInstallmentDto>>> GetByLoanIdAsync(
        int loanId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<EmployeeLoanInstallmentDto>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
