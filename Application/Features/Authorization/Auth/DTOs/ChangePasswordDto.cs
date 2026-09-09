namespace MicroERP.Application.Features.Authorization.Auth.DTOs
{
    public class ChangePasswordDto
    {
        public string UserId { get; set; } = null!;

        public string CurrentPassword { get; set; } = null!;

        public string NewPassword { get; set; } = null!;
    }
}
