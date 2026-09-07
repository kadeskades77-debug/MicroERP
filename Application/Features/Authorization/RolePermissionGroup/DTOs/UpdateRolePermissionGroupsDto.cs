namespace MicroERP.Application.Features.Authorization.RolePermissionGroup.DTOs
{
    public class UpdateRolePermissionGroupsDto
    {
        public string RoleId { get; set; } = null!;
        public List<string> PermissionGroupKeys { get; set; } = new();
    }
}
