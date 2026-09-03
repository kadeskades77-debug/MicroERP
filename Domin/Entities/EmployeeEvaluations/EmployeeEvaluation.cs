using MicroERP.Domin.Common;
using MicroERP.Domin.Entities.Employees;
using MicroERP.Domin.Enums;
using MicroERP.Domin.Identity;

namespace MicroERP.Domin.Entities.EmployeeEvaluations
{
    public class EmployeeEvaluation : BaseEntity
    {

        public int EmployeeId { get; set; }

        public string EvaluatorId { get; set; } = null!;

        public int PeriodId { get; set; }

        public int TemplateId { get; set; }

        public decimal TotalScore { get; set; }

        public FinalRate FinalRate { get; set; }

        public EmployeeEvaluationStatus Status { get; set; }
        public string? RejectedBy { get; set; }

        public DateTime? RejectedOn { get; set; }

        public string? RejectionReason { get; set; }

        public string? ApprovedBy { get; set; }

        public DateTime? ApprovedOn { get; set; }

        // Navigation
        public Employee Employee { get; set; } = null!;

        public ApplicationUser Evaluator { get; set; } = null!;

        public EvaluationPeriod Period { get; set; } = null!;

        public EvaluationTemplate Template { get; set; } = null!;

        public ICollection<EmployeeEvaluationItem> Items { get; set; }
            = new List<EmployeeEvaluationItem>();
    }
}
