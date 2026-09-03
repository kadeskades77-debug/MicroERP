namespace MicroERP.Application.Features.EmployeeEvaluations;

public class UpdateEmployeeEvaluationItemDto
{
    public int CriterionId { get; set; }
    public decimal Score { get; set; }
    public string? Notes { get; set; }
}
