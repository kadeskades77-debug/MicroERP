using MicroERP.Application.Common.Models;
namespace MicroERP.Application.Features.EmployeeEvaluations.Interfaces;

public interface IEmployeeEvaluationService
{ 
    Task<Result<EmployeeEvaluationDto>> CreateAsync(CreateEmployeeEvaluationDto dto, CancellationToken ct = default);
    Task<Result<EmployeeEvaluationDto>> UpdateAsync(int id, UpdateEmployeeEvaluationDto dto, CancellationToken ct = default);
    Task<Result<bool>> SubmitAsync(int id, CancellationToken ct = default);
    Task<Result<bool>> ApproveAsync(int id, CancellationToken ct = default);
    Task<Result<bool>> RejectAsync(int id, string? reason = null, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default); 
}
