using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Payrolls.EmployeeLoans.DTOs;

namespace MicroERP.Application.Features.Payrolls.EmployeeLoans.Interfaces;

public interface IEmployeeLoanService
{
    Task<Result<EmployeeLoanDto>> CreateAsync(
        CreateEmployeeLoanDto dto,
        CancellationToken cancellationToken = default);

    Task<Result<EmployeeLoanDto>> UpdateAsync(
        int id,
        UpdateEmployeeLoanDto dto,
        CancellationToken cancellationToken = default);

    Task<Result<EmployeeLoanDto>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Result<PagedResult<EmployeeLoanDto>>> GetAllAsync(
        EmployeeLoanFilterDto filter,
        PagedRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> CancelAsync(
     int id,
     string userId,
     string? reason,
     CancellationToken cancellationToken = default);

    Task<Result> SuspendAsync(
     int id,
     string userId,
     string reason,
     CancellationToken cancellationToken = default);

    Task<Result> ResumeAsync(
        int id,
        CancellationToken cancellationToken = default);

}
