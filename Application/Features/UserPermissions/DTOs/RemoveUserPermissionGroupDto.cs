
namespace MicroERP.Application.Features.UserPermissions.DTOs
{
    public class RemoveUserPermissionGroupDto
    {
        public string UserId { get; set; } = null!;

        public string PermissionGroupKey { get; set; } = null!;
    }
}
