

using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeEvaluations.DTOs.EmployeeEvaluationDto
{
    public class EmployeeEvaluationExcelHeaderDto
    {
        public string EmployeeName { get; set; } = null!;

        public string DepartmentName { get; set; } = null!;

        public string PeriodLabel { get; set; } = null!;

        public decimal TotalScore { get; set; }

        public FinalRate FinalRate { get; set; }

        public EvaluationReportPeriod PeriodType { get; set; }
    }
}
