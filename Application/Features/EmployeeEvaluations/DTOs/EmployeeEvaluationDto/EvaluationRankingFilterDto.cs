

using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeEvaluations.DTOs.EmployeeEvaluationDto
{
    public class EvaluationRankingFilterDto
    {
        public EvaluationRankingType Type { get; set; }

        public int? RankingCount { get; set; }

        public int? DepartmentId { get; set; }

        public int? Year { get; set; }

        public int? Month { get; set; }

        public bool GroupByDepartment { get; set; }
    }
}
