using MicroERP.Domin.Entities.Employees;

namespace MicroERP.Application.Features.Positions.Interfaces;

public interface IPositionQueries
{
    Task<Position?> GetByIdAsync(int id,
        CancellationToken cancellationToken = default);

    Task<List<Position>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<bool> CodeExistsAsync(
        string code,
        int? excludeId = null,
        CancellationToken cancellationToken = default);

    Task<bool> HasEmployeesAsync(
        int positionId,
        CancellationToken cancellationToken = default);
    Task<bool> EmployeeExistsAsync(
    int employeeId,
    CancellationToken cancellationToken = default);

    Task<bool> EmployeeAssignedToPositionAsync(
        int employeeId,
        int positionId,
        CancellationToken cancellationToken = default);

    Task<List<Employee>> GetEmployeesAsync(
        int positionId,
        CancellationToken cancellationToken = default);

    Task<bool> PositionAlreadyAssignedInDepartmentAsync(
    int positionId,
    int departmentId,
    int employeeId,
    CancellationToken cancellationToken = default);

    Task<bool> PositionAlreadyAssignedCompanyWideAsync(
        int positionId,
        int employeeId,
        CancellationToken cancellationToken = default);
}
