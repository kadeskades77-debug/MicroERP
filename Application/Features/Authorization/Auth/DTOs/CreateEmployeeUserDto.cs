namespace MicroERP.Application.Features.Authentication.Auth.DTOs
{
    public class CreateEmployeeUserDto
    {
        public string FullName { get; set; } = null!;

        public string? Email { get; set; }
    }
}
