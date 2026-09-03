
namespace MicroERP.Application.Features.EmployeeEvaluations.DTOs.EmployeeEvaluationDto
{
    public class EmployeeMonthlyEvaluationExcelDto
    {
        public string CriterionName { get; set; } = null!;

        public decimal Score { get; set; }

        public string? Notes { get; set; }
    }
}
