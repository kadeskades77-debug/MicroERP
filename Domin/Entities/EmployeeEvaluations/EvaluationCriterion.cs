

using MicroERP.Domin.Common;
using MicroERP.Domin.Enums;

namespace MicroERP.Domin.Entities.EmployeeEvaluations
{
    public class EvaluationCriterion : BaseEntity
    {

        public int TemplateId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public decimal MaxScore { get; set; } = 100;

        public decimal Weight { get; set; }

        public int SortOrder { get; set; }

        public EvaluationCriterionSource Source { get; set; }

        // Navigation
        public EvaluationTemplate Template { get; set; } = null!;

        public ICollection<EmployeeEvaluationItem> EvaluationItems { get; set; }
            = new List<EmployeeEvaluationItem>();
    }
}
