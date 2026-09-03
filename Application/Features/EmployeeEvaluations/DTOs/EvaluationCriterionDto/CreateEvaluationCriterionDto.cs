namespace MicroERP.Application.Features.EmployeeEvaluations;

public class CreateEvaluationCriterionDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal MaxScore { get; set; } = 100;
    public decimal Weight { get; set; }
    public int SortOrder { get; set; }
}
