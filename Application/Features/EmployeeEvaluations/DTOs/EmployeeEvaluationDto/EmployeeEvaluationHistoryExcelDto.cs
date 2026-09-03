

using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeEvaluations.DTOs.EmployeeEvaluationDto
{
    public class EmployeeEvaluationHistoryExcelDto
    {
        public string PeriodLabel { get; set; } = null!;

        public decimal TotalScore { get; set; }

        public FinalRate FinalRate { get; set; }

    }
}
