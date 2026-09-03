using MicroERP.Domain.Identity;

namespace MicroERP.Domin.Identity
{
    public class UserPermissionAssignment
    {
        public string UserId { get; set; } = null!;

        public ApplicationUser User { get; set; } = null!;

        public int PermissionGroupId { get; set; }

        public PermissionGroup PermissionGroup { get; set; } = null!;
    }
}
