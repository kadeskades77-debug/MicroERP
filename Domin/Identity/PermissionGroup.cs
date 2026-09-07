using MicroERP.Domin.Common;
using MicroERP.Domin.Identity;

namespace MicroERP.Domain.Identity;

public class PermissionGroup : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string Key { get; set; } = null!;

    public bool IsSystem { get; set; }

    public bool IsRequired { get; set; }

    public ICollection<PermissionGroupPermission> PermissionGroupPermissions { get; set; }
        = new List<PermissionGroupPermission>();

    public ICollection<RolePermissionGroup> RolePermissionGroups { get; set; }
        = new List<RolePermissionGroup>();

    public ICollection<UserPermissionAssignment> UserPermissionAssignments { get; set; }
        = new List<UserPermissionAssignment>();
}