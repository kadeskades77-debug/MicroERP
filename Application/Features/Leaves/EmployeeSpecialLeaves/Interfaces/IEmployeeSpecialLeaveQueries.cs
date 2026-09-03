using MicroERP.Application.Features.Leaves.EmployeeSpecialLeaves.DTOs;

namespace MicroERP.Application.Features.Leaves.EmployeeSpecialLeaves.Interfaces;

public interface IEmployeeSpecialLeaveQueries
{
    Task<SpecialLeaveDto?> GetByIdAsync(int id,
        CancellationToken cancellationToken = default);


    Task<List<SpecialLeaveDto>> GetAllAsync(
        CancellationToken cancellationToken = default);
}