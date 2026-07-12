namespace MicroERP.Domain.Identity;

public class RolePermissionGroup
{
    public string RoleId { get; set; }
    public ApplicationRole Role { get; set; }
    public int PermissionGroupId { get; set; }

    public PermissionGroup PermissionGroup { get; set; } = null!;
}