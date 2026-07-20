using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeLeaveBalances.DTOs;

namespace MicroERP.Application.Features.EmployeeLeaveBalances.Interfaces;

public interface IEmployeeLeaveBalanceService
{
    Task<Result<List<LeaveBalanceDto>>> GetByEmployeeAsync(int employeeId,int year,
        CancellationToken cancellationToken = default);


    Task<Result<LeaveBalanceDto>> CreateAsync(int employeeId,CreateLeaveBalanceDto dto,
        CancellationToken cancellationToken = default);


    Task<Result<LeaveBalanceDto>> UpdateAsync(int id,UpdateLeaveBalanceDto dto,
        CancellationToken cancellationToken = default);


    Task<Result> DeleteAsync(int id,
        CancellationToken cancellationToken = default);
}