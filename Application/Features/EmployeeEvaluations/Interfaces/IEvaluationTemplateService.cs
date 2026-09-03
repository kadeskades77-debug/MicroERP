using MicroERP.Application.Common.Models;
namespace MicroERP.Application.Features.EmployeeEvaluations.Interfaces;

public interface IEvaluationTemplateService 
{ 
    Task<Result<EvaluationTemplateDto>> CreateAsync(CreateEvaluationTemplateDto dto, CancellationToken ct = default);
    Task<Result<EvaluationTemplateDto>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Result<List<EvaluationTemplateDto>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<EvaluationTemplateDto>> UpdateAsync(int id, UpdateEvaluationTemplateDto dto, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default); 
}
