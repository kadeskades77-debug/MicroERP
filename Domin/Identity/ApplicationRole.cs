using Microsoft.AspNetCore.Identity;

namespace MicroERP.Domain.Identity;

public class ApplicationRole : IdentityRole
{
    public string? Description { get; set; }

    public bool IsSystem { get; set; }
    public ICollection<RolePermissionGroup> RolePermissionGroups { get; set; }
      = new List<RolePermissionGroup>();
}