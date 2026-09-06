using MicroERP.Application.Common.DTOs;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Departments.DTOs;

namespace MicroERP.Application.Features.Departments.Interfaces;

public interface IDepartmentService
{
    Task<Result<DepartmentDto>> CreateAsync(
        CreateDepartmentDto dto,
        CancellationToken cancellationToken = default);

    Task<Result<DepartmentDto>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Result<List<DepartmentListDto>>> GetAllAsync(
     DepartmentFilterDto filter,
     CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(
        int id,
        UpdateDepartmentDto dto,
        CancellationToken cancellationToken = default);

    Task<Result<DepartmentDto>> AssignManagerAsync(
        int id,
        AssignDepartmentManagerDto dto,
        CancellationToken cancellationToken = default);

    Task<Result> TransferDepartmentManagerAsync(
        int managerEmployeeId,
        TransferDepartmentManagerDto dto,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Result> RestoreAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Result<List<LookupDto>>> GetLookupAsync(
        CancellationToken cancellationToken = default);

    Task<Result<List<LookupDto>>> GetDepartmentsWithoutManagerAsync(
    CancellationToken cancellationToken = default);
}