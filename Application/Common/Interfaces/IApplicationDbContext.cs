using Domin.Entities;
using MicroERP.Domain.Audit;
using MicroERP.Domain.Identity;
using MicroERP.Domin.Entities;
using MicroERP.Domin.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace MicroERP.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Employee> Employees { get; }
        DbSet<Department> Departments { get; }
        DbSet<Permission> Permissions { get; }
        DbSet<PermissionGroup> PermissionGroups { get; }
        DbSet<PermissionGroupPermission> PermissionGroupPermissions { get; }
        DbSet<RolePermissionGroup> RolePermissionGroups { get; }
        DbSet<UserPermissionAssignment> UserPermissionAssignments { get; }
        DbSet<EmployeeDocument> EmployeeDocuments { get; }
        DbSet<IdentityUserRole<string>> UserRoles { get; }
        DbSet<ApplicationRole> Roles { get; }
        DbSet<AuditLog> AuditLogs { get; }
        DbSet<EmployeeLeave> EmployeeLeaves { get; }
        DbSet<EmployeeLeaveBalance> EmployeeLeaveBalances { get; }
        DbSet<LeavePolicy> LeavePolicies { get; }
        DbSet<EmployeeSpecialLeave> EmployeeSpecialLeaves { get; }
        DbSet<LeaveAttachment> LeaveAttachments { get; }
        DatabaseFacade Database { get; }
        Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}