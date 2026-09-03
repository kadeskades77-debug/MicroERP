

using MicroERP.Domin.Common;

namespace MicroERP.Domin.Entities.EmployeeEvaluations
{
    public class EvaluationTemplate : BaseEntity
    {

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        // Navigation
        public ICollection<EvaluationCriterion> Criteria { get; set; }
            = new List<EvaluationCriterion>();

        public ICollection<EmployeeEvaluation> EmployeeEvaluations { get; set; }
            = new List<EmployeeEvaluation>();
    }
}
