
using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeEvaluations.DTOs.EmployeeEvaluationDto
{
    public class DepartmentEvaluationExcelHeaderDto
    {
        public string DepartmentName { get; set; } = null!;

        public string? ManagerName { get; set; }

        public string PeriodName { get; set; } = null!;
        public string PeriodLabel { get; set; } = null!;

        public EvaluationReportPeriod PeriodType { get; set; }

        public decimal AverageScore { get; set; }

        public FinalRate FinalRate { get; set; }
    }
}
