namespace MicroERP.Application.Features.Authorization.Roles.DTOs
{
    public class UpdateUserRolesDto
    {
        public string UserId { get; set; } = null!;
        public List<string> RoleIds { get; set; } = [];
    }
}
