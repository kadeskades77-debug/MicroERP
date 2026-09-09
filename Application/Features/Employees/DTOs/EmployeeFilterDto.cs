

using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Employees.DTOs
{
    public class EmployeeFilterDto
    {
        public string? Search { get; set; }
        public EmployeeSearchBy SearchBy { get; set; } = EmployeeSearchBy.Name;
    }
}
