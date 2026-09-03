

namespace MicroERP.Application.Features.EmployeeEvaluations.DTOs.EmployeeEvaluationDto
{
    public class EmployeeEvaluationHistoryExcelResultDto
    {
        public EmployeeEvaluationExcelHeaderDto Header { get; set; }
            = null!;

        public List<EmployeeEvaluationHistoryExcelDto> Evaluations { get; set; }
            = new();
    }
}
