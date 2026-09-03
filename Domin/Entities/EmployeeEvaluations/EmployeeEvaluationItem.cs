

using MicroERP.Domin.Common;

namespace MicroERP.Domin.Entities.EmployeeEvaluations
{
    public class EmployeeEvaluationItem : BaseEntity
    {

        public int EmployeeEvaluationId { get; set; }

        public int CriterionId { get; set; }

        public decimal Score { get; set; }

        public string? Notes { get; set; }

        // Navigation
        public EmployeeEvaluation EmployeeEvaluation { get; set; } = null!;

        public EvaluationCriterion Criterion { get; set; } = null!;
    }
}
