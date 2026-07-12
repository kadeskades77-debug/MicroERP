
namespace MicroERP.Application.Features.Departments.DTOs
{
    public class DepartmentDto
    {
        public int Id { get; set; }

        public string Code { get; set; } = null!;

        public string NameAr { get; set; } = null!;

        public string? NameEn { get; set; }

        public int? ManagerEmployeeId { get; set; }

        public bool IsActive { get; set; }
        public bool HasManager { get; set; }
    }
}
