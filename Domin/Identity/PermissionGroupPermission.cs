namespace MicroERP.Domain.Identity;

public class PermissionGroupPermission
{
    public int PermissionGroupId { get; set; }

    public PermissionGroup PermissionGroup { get; set; } = null!;

    public int PermissionId { get; set; }

    public Permission Permission { get; set; } = null!;
}