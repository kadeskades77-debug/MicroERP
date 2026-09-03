
namespace MicroERP.Application.Features.EmployeeEvaluations;

public class EvaluationTemplateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public List<EvaluationCriterionDto> Criteria { get; set; } = new();
}
