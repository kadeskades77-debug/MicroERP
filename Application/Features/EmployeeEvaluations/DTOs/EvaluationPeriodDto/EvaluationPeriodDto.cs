using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeEvaluations;

public class EvaluationPeriodDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public EvaluationPeriodStatus Status { get; set; }
    public bool IsActive { get; set; }
}
