
using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Employees.DTOs
{
    public class EmployeeListDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public string? Email { get; set; }

        public string Phone { get; set; } = null!;
         
        public decimal Salary { get; set; }

        public string DepartmentCode { get; set; } = null!;

        public string DepartmentName { get; set; } = null!;

        public bool IsActive { get; set; }
    }
}
