

using MicroERP.Domin.Common;
using MicroERP.Domin.Enums;

namespace MicroERP.Domin.Entities.EmployeeEvaluations
{
    public class EvaluationPeriod : BaseEntity
    {

        public string Name { get; set; } = null!;

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public EvaluationPeriodStatus Status { get; set; }

        // Navigation
        public ICollection<EmployeeEvaluation> EmployeeEvaluations { get; set; }
            = new List<EmployeeEvaluation>();
    }
}
