using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Leaves.EmployeeSpecialLeaves.DTOs;

namespace MicroERP.Application.Features.Leaves.EmployeeSpecialLeaves.Interfaces;

public interface IEmployeeSpecialLeaveService
{
    Task<Result<SpecialLeaveDto>> CreateAsync(
       int employeeId,
       string userId,
       CreateSpecialLeaveDto dto,
       CancellationToken cancellationToken = default);


    Task<Result<SpecialLeaveDto>> CancelAsync(int id,string userId,
        CancellationToken cancellationToken = default);
}