namespace MicroERP.Application.Features.EmployeeEvaluations;

public class EvaluationCriterionLookupDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal MaxScore { get; set; }
    public decimal Weight { get; set; }
    public int SortOrder { get; set; }
}
