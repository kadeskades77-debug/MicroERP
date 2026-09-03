namespace MicroERP.Application.Features.EmployeeEvaluations;

public class CreateEvaluationPeriodDto
{
    public string Name { get; set; } = null!;
    public int Year { get; set; }
    public int Month { get; set; }
}

