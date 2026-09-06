

namespace MicroERP.Application.Features.Departments.DTOs
{
    public class DepartmentFilterDto
    {
        public string? Search { get; set; }

        public bool? IsActive { get; set; }

        public bool? HasManager { get; set; }
    }
}
