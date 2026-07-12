namespace MicroERP.Application.Features.PermissionGroups.DTOs
{
    public class RemovePermissionsFromGroupDto
    {
        public List<string> PermissionKeys { get; set; } = new();
    }
}
