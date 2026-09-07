namespace MicroERP.Application.Features.Authorization.RolePermissionGroup.DTOs
{
    public class RolePermissionGroupDto
    {
        public string RoleId { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public List<string> PermissionGroupKeys { get; set; } = new();
    }
}
