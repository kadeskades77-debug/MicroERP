using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeEvaluations.DTOs.EmployeeEvaluationDto
{
    public class EmployeeEvaluationExcelFilterDto
    {
        public int Year { get; set; }

        public EvaluationReportPeriod Period { get; set; }

        public int? Month { get; set; }

        public int? Quarter { get; set; }

        public int? HalfYear { get; set; }

        public int? DepartmentId { get; set; }

        public EmployeeEvaluationStatus? Status { get; set; }
    }
}
