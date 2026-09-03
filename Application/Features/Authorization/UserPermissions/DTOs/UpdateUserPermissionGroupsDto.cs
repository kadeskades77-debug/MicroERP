namespace MicroERP.Application.Features.Authorization.UserPermissions.DTOs
{
    public class UpdateUserPermissionGroupsDto
    {
        public string UserId { get; set; } = null!;

        public List<string> PermissionGroupKeys { get; set; } = [];
    }
}
