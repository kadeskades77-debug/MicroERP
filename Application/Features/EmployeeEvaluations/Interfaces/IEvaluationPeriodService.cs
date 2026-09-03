using MicroERP.Application.Common.Models;
namespace MicroERP.Application.Features.EmployeeEvaluations.Interfaces;

public interface IEvaluationPeriodService
{ 
    Task<Result<EvaluationPeriodDto>> CreateAsync(CreateEvaluationPeriodDto dto, CancellationToken ct = default);
    Task<Result<EvaluationPeriodDto>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Result<List<EvaluationPeriodDto>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<EvaluationPeriodDto>> UpdateAsync(int id, UpdateEvaluationPeriodDto dto, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default);
    Task<Result<bool>> OpenAsync(int id,CancellationToken ct = default);
    Task<Result<bool>> CloseAsync(int id,CancellationToken ct = default);
}
