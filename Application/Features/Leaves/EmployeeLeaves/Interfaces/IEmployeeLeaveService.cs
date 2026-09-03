using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Leaves.EmployeeLeaves.DTOs;

namespace MicroERP.Application.Features.Leaves.EmployeeLeaves.Interfaces;

public interface IEmployeeLeaveService
{
    Task<Result<LeaveDto>> CreateAsync(string userId, CreateLeaveDto dto,
        CancellationToken cancellationToken = default);


    Task<Result<LeaveDto>> UpdateAsync(int id,UpdateLeaveDto dto,
        CancellationToken cancellationToken = default);


    Task<Result> DeleteAsync(int id,
        CancellationToken cancellationToken = default);


    Task<Result<LeaveDto>> ApproveAsync(int id,string userId,
        CancellationToken cancellationToken = default);


    Task<Result<LeaveDto>> RejectAsync(int id,string userId,
        RejectLeaveDto dto,
        CancellationToken cancellationToken = default);


    Task<Result<List<LeaveListDto>>> GetAllAsync(
        CancellationToken cancellationToken = default);


    Task<Result<LeaveDto>> GetByIdAsync(int id,
        CancellationToken cancellationToken = default);


    Task<Result<List<LeaveListDto>>> GetByEmployeeAsync(int employeeId,
        CancellationToken cancellationToken = default);
    Task<Result<LeaveDto>> CancelAsync(int id,string userId,
    CancellationToken cancellationToken = default);
}