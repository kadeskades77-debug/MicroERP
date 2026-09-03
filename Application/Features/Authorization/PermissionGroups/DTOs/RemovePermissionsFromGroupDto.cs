namespace MicroERP.Application.Features.Authorization.PermissionGroups.DTOs
{
    public class RemovePermissionsFromGroupDto
    {
        public List<string> PermissionKeys { get; set; } = new();
    }
}
