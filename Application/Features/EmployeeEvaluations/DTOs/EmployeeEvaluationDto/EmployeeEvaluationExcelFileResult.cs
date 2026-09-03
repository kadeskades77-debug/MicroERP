
namespace MicroERP.Application.Features.EmployeeEvaluations.DTOs.EmployeeEvaluationDto
{
    public class EmployeeEvaluationExcelFileResult
    {
        public byte[] File { get; set; } = null!;

        public string EmployeeName { get; set; } = null!;
    }
}
