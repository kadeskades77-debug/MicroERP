

namespace MicroERP.Application.Features.EmployeeEvaluations;

public class UpdateEvaluationTemplateDto
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public List<UpdateEvaluationCriterionDto>? Criteria { get; set; }
}
