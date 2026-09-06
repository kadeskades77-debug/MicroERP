

namespace MicroERP.Application.Features.Authorization.UserPermissions.DTOs
{
    public class AddUserPermissionGroupsDto
    {
        public string UserId { get; set; } = null!;

        public List<string> PermissionGroupKeys { get; set; }
            = new();
    }
}
