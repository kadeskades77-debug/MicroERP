namespace MicroERP.Application.Features.Authorization.Auth.DTOs
{
    public class CurrentUserDto
    {
        public string Email { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public List<string> Roles { get; set; } = [];

        public List<string> Permissions { get; set; } = [];
    }
}