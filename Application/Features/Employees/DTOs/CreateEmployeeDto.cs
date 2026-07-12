namespace MicroERP.Application.Features.Employees.DTOs
{
    public class CreateEmployeeDto
    {
        public string FullName { get; set; } = null!;

        public string? Email { get; set; }

        public string Phone { get; set; } = null!;

        public decimal Salary { get; set; }

        public int DepartmentId { get; set; }
    }
}
