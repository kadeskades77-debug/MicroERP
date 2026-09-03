
namespace MicroERP.Application.Features.EmployeeEvaluations;

public class CreateEvaluationTemplateDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public List<CreateEvaluationCriterionDto> Criteria { get; set; } = new();
}
