namespace MicroERP.Application.Features.EmployeeEvaluations;

public class EmployeeEvaluationItemDto
{
    public int Id { get; set; }
    public int CriterionId { get; set; }
    public string CriterionName { get; set; } = null!;
    public decimal MaxScore { get; set; }
    public decimal Weight { get; set; }
    public decimal Score { get; set; }
    public decimal WeightedScore { get; set; }
    public string? Notes { get; set; }
}
