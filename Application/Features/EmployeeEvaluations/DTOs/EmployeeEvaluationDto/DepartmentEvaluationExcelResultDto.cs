

namespace MicroERP.Application.Features.EmployeeEvaluations.DTOs.EmployeeEvaluationDto
{
    public class DepartmentEvaluationExcelResultDto
    {
        public DepartmentEvaluationExcelHeaderDto Header { get; set; }
            = null!;

        public List<DepartmentEmployeeEvaluationExcelDto> Employees { get; set; }
            = new();
    }
}
