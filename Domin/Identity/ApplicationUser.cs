using MicroERP.Domin.Entities.Employees;
using Microsoft.AspNetCore.Identity;


namespace MicroERP.Domin.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public Employee? Employee { get; set; }
        public ICollection<UserPermissionAssignment> UserPermissionAssignments { get; set; }
    = new List<UserPermissionAssignment>();
    }
}
