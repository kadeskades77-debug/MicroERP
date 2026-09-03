namespace MicroERP.Application.Features.EmployeeEvaluations.DTOs.EmployeeEvaluationDto
{
    public class EmployeeMonthlyEvaluationExcelResultDto
    {
        public EmployeeMonthlyEvaluationExcelHeaderDto Header { get; set; }
            = null!;

        public List<EmployeeMonthlyEvaluationExcelDto> Items { get; set; }
            = new();
    }
}
