

namespace MicroERP.Application.Features.EmployeeEvaluations;

public class CreateEmployeeEvaluationDto
{
    public int EmployeeId { get; set; }
    public int PeriodId { get; set; }
    public int TemplateId { get; set; }
    public List<CreateEmployeeEvaluationItemDto> Items { get; set; } = new();
}
