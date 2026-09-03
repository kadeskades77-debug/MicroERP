namespace MicroERP.Application.Features.EmployeeEvaluations;

public class EvaluationTemplateListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int CriteriaCount { get; set; }
}
