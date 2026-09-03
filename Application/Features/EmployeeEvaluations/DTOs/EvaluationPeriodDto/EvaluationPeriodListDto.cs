namespace MicroERP.Application.Features.EmployeeEvaluations;

public class EvaluationPeriodListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int Status { get; set; }
}
