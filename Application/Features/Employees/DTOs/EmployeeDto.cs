
namespace MicroERP.Application.Features.Employees.DTOs
{
    public class EmployeeDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public string? Email { get; set; }

        public string Phone { get; set; } = null!;

        public decimal Salary { get; set; }

        public string DepartmentCode { get; set; } = null!;

        public string DepartmentName { get; set; } = null!;

        public int? PositionId { get; set; }

        public string? PositionCode { get; set; }

        public string? PositionName { get; set; }

        public bool IsActive { get; set; }
    }
}
