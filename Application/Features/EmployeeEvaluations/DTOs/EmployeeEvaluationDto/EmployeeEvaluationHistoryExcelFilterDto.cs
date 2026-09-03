
using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeEvaluations.DTOs.EmployeeEvaluationDto
{
    public class EmployeeEvaluationHistoryExcelFilterDto
    {
        public int EmployeeId { get; set; }

        public int? Year { get; set; }

        public EvaluationReportPeriod Period { get; set; }
            = EvaluationReportPeriod.Monthly;

        public int? Quarter { get; set; }

        public int? HalfYear { get; set; }
    }
}
