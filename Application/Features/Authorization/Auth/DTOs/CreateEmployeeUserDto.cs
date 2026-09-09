namespace MicroERP.Application.Features.Authorization.Auth.DTOs
{
    public class CreateEmployeeUserDto
    {
        public string FullName { get; set; } = null!;

        public string? Email { get; set; }
    }
}
