using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeEvaluations;

public class EmployeeEvaluationFilterDto
{
    public int? EmployeeId { get; set; }

    public string? EvaluatorId { get; set; }

    public int? TemplateId { get; set; }

    public int? Year { get; set; }

    public int? Month { get; set; }

    public EmployeeEvaluationStatus? Status { get; set; }

    public FinalRate? FinalRate { get; set; }
 
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}
