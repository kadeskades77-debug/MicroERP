using MicroERP.Domin.Common;

namespace MicroERP.Domain.Identity;

public class Permission : BaseEntity
{
    public string Key { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public ICollection<PermissionGroupPermission> PermissionGroupPermissions { get; set; }
        = new List<PermissionGroupPermission>();
}