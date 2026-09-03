

using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeEvaluations.DTOs.EmployeeEvaluationDto
{
    public class EmployeeMonthlyEvaluationExcelHeaderDto
    {
        public string EmployeeName { get; set; } = null!;

        public string DepartmentName { get; set; } = null!;

        public string PeriodName { get; set; } = null!;

        public decimal TotalScore { get; set; }

        public FinalRate FinalRate { get; set; }

        public EmployeeEvaluationStatus Status { get; set; }
    }
}
