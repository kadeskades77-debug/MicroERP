namespace MicroERP.Application.Features.Authorization.PermissionGroups.DTOs
{
    public class AddPermissionsToGroupDto
    {
        public List<string> PermissionKeys { get; set; } = new();
    }
}
