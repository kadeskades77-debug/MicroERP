

using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeEvaluations.DTOs.EmployeeEvaluationDto
{
    public class EmployeeEvaluationRankingDto
    {
        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; } = null!;

        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; } = null!;

        public int Year { get; set; }

        public int Month { get; set; }

        public string TemplateName { get; set; } = null!;

        public decimal TotalScore { get; set; }

        public FinalRate FinalRate { get; set; }

        public EmployeeEvaluationStatus Status { get; set; }
        public EvaluationRankingType Type { get; set; }

        public int Rank { get; set; }
    }
}
