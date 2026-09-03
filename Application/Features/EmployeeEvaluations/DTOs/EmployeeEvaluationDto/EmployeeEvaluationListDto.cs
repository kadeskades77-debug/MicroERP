using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeEvaluations;

public class EmployeeEvaluationListDto
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; } = null!;

    public string EvaluatorName { get; set; } = null!;

    public int PeriodId { get; set; }

    public string PeriodName { get; set; } = null!;

    public int Year { get; set; }

    public int Month { get; set; }

    public int TemplateId { get; set; }

    public string TemplateName { get; set; } = null!;

    public decimal TotalScore { get; set; }

    public FinalRate FinalRate { get; set; }

    public string? RejectedByName { get; set; }

    public DateTime? RejectedOn { get; set; }

    public string? RejectionReason { get; set; }

    public string? ApprovedByName { get; set; }

    public DateTime? ApprovedOn { get; set; }

    public EmployeeEvaluationStatus Status { get; set; }
}
