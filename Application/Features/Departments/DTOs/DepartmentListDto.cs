namespace MicroERP.Application.Features.Departments.DTOs
{
    public class DepartmentListDto
    {
        public int Id { get; set; }

        public string Code { get; set; } = null!;

        public string NameAr { get; set; } = null!;

        public string NameEn { get; set; } = null!;

        public string? ManagerName { get; set; }

        public bool IsActive { get; set; }
    }
}
