using MicroERP.Application.Common.DTOs;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Positions.DTOs;

namespace MicroERP.Application.Features.Positions.Interfaces;

public interface IPositionService
{
    Task<Result<PositionDto>> CreateAsync(
        CreatePositionDto dto,
        CancellationToken cancellationToken = default);

    Task<Result<PositionDto>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Result<List<PositionListDto>>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(
        int id,
        UpdatePositionDto dto,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Result> RestoreAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Result> AssignEmployeeAsync(
    int positionId,
    int employeeId,
    CancellationToken cancellationToken = default);

    Task<Result> RemoveEmployeeAsync(
        int positionId,
        int employeeId,
        CancellationToken cancellationToken = default);

    Task<Result<List<EmployeePositionListDto>>> GetEmployeesAsync(
        int positionId,
        CancellationToken cancellationToken = default);

    Task<Result<List<LookupDto>>> GetLookupAsync(
        CancellationToken cancellationToken = default);
}
