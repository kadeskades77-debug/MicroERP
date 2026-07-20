using MicroERP.Application.Features.EmployeeSpecialLeaves.DTOs;

namespace MicroERP.Application.Features.EmployeeSpecialLeaves.Interfaces;

public interface IEmployeeSpecialLeaveQueries
{
    Task<SpecialLeaveDto?> GetByIdAsync(int id,
        CancellationToken cancellationToken = default);


    Task<List<SpecialLeaveDto>> GetAllAsync(
        CancellationToken cancellationToken = default);
}