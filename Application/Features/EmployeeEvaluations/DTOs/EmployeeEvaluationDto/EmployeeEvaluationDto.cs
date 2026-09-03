
using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeEvaluations;

public class EmployeeEvaluationDto
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; } = null!;

    public string DepartmentName { get; set; } = null!;


    // =========================================================
    // Evaluator
    // =========================================================

    public string EvaluatorName { get; set; } = null!;


    // =========================================================
    // Period
    // =========================================================

    public int PeriodId { get; set; }

    public string PeriodName { get; set; } = null!;

    public int Year { get; set; }

    public int Month { get; set; }


    // =========================================================
    // Template
    // =========================================================

    public int TemplateId { get; set; }

    public string TemplateName { get; set; } = null!;


    // =========================================================
    // Result
    // =========================================================

    public decimal TotalScore { get; set; }

    public FinalRate FinalRate { get; set; }


    // =========================================================
    // Rejection
    // =========================================================

    public string? RejectedByName { get; set; }

    public DateTime? RejectedOn { get; set; }

    public string? RejectionReason { get; set; }


    // =========================================================
    // Approval
    // =========================================================

    public string? ApprovedByName { get; set; }

    public DateTime? ApprovedOn { get; set; }


    // =========================================================
    // Status
    // =========================================================

    public EmployeeEvaluationStatus Status { get; set; }


    // =========================================================
    // Criteria
    // =========================================================

    public List<EmployeeEvaluationItemDto> Items { get; set; }
        = new();
}
