using Domin.Entities;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Domain.Audit;
using MicroERP.Domain.Identity;
using MicroERP.Domin.Common;
using MicroERP.Domin.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

public class ApplicationDbContext
    : IdentityDbContext<ApplicationUser, ApplicationRole, string>,
      IApplicationDbContext
{
    private readonly ICurrentUserService _currentUser;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUser)
        : base(options)
    {
        _currentUser = currentUser;
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Department>()
    .HasQueryFilter(x => !x.IsDeleted);

        builder.Entity<Employee>()
            .HasQueryFilter(x => !x.IsDeleted);

        builder.Entity<Permission>()
    .HasQueryFilter(x => !x.IsDeleted);

        builder.Entity<Permission>()
    .HasQueryFilter(x => !x.IsDeleted);

        builder.Entity<PermissionGroup>()
    .HasQueryFilter(x => !x.IsDeleted);
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(
               typeof(ApplicationDbContext).Assembly);
    }
    public override int SaveChanges()
    {
        ApplyAuditRules();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditRules();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditRules()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        var now = DateTime.UtcNow;
        var userId = _currentUser.UserId;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedOn = now;
                entry.Entity.CreatedBy = userId;
                entry.Entity.IsActive = true;
                entry.Entity.IsDeleted = false;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifiedOn = now;
                entry.Entity.ModifiedBy = userId;

                entry.Property(x => x.CreatedOn)
                    .IsModified = false;

                entry.Property(x => x.CreatedBy)
                    .IsModified = false;
            }
        }
    }
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<PermissionGroup> PermissionGroups { get; set; }
    public DbSet<PermissionGroupPermission> PermissionGroupPermissions { get; set; }
    public DbSet<RolePermissionGroup> RolePermissionGroups { get; set; }
    public DbSet<IdentityUserRole<string>> UserRoles => Set<IdentityUserRole<string>>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<UserPermissionAssignment> UserPermissionAssignments
     => Set<UserPermissionAssignment>();
}