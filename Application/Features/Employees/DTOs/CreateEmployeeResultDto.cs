
namespace MicroERP.Application.Features.Employees.DTOs
{
    public class CreateEmployeeResultDto
    {
        public int EmployeeId { get; set; }

        public string UserName { get; set; } = null!;

        public string GeneratedPassword { get; set; } = null!;
    }
}
