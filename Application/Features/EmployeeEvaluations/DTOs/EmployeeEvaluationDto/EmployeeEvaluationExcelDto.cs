
using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeEvaluations.DTOs.EmployeeEvaluationDto
{
    public class EmployeeEvaluationExcelDto
    {
        public string EmployeeName { get; set; } = null!;

        public string DepartmentName { get; set; } = null!;

        public string PeriodLabel { get; set; } = null!;

        public EvaluationReportPeriod PeriodType { get; set; }

        public decimal TotalScore { get; set; }

        public FinalRate FinalRate { get; set; }

        public EmployeeEvaluationStatus Status { get; set; }
    }
}
