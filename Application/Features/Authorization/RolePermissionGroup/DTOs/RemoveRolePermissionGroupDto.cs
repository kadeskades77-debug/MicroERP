

namespace MicroERP.Application.Features.Authorization.RolePermissionGroup.DTOs
{

    public class RemoveRolePermissionGroupDto
    {
        public string RoleId { get; set; } = null!;
        public string PermissionGroupKey { get; set; } = null!;
    }
}
