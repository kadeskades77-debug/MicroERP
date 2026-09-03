using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeEvaluations.DTOs.EmployeeEvaluationDto
{
    public class DepartmentEvaluationExcelFilterDto
    {
        public int DepartmentId { get; set; }

        public int Year { get; set; }

        public EvaluationReportPeriod Period { get; set; }
            = EvaluationReportPeriod.Monthly;

        public int? Month { get; set; }

        public int? Quarter { get; set; }
        public EmployeeEvaluationStatus Status { get; set; }
    }
}
