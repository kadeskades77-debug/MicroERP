namespace MicroERP.Application.Features.Authorization.Auth.DTOs
{
    public class CreateEmployeeUserResultDto
    {
        public string UserId { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}
