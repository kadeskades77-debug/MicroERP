namespace MicroERP.Application.Features.Authorization.UserPermissions.DTOs
{
    public class RemoveUserPermissionGroupDto
    {
        public string UserId { get; set; } = null!;

        public string PermissionGroupKey { get; set; } = null!;
    }
}
