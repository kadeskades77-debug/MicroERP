using MicroERP.Application.Common.Interfaces;
using MicroERP.Domain.Audit;
using MicroERP.Domain.Identity;
using MicroERP.Domin.Common;
using MicroERP.Domin.Entities.EmployeeAttendance;
using MicroERP.Domin.Entities.EmployeeEvaluations;
using MicroERP.Domin.Entities.EmployeeLeaves;
using MicroERP.Domin.Entities.Employees;
using MicroERP.Domin.Entities.Payrolls;
using MicroERP.Domin.Entities.Policies;
using MicroERP.Domin.Entities.Ticketing;
using MicroERP.Domin.Identity;
using MicroERP.Persistence.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

public class ApplicationDbContext
    : IdentityDbContext<ApplicationUser, ApplicationRole, string>,
      IApplicationDbContext
{
    private readonly ICurrentUserService? _currentUser;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService? currentUser = null)
        : base(options)
    {
        _currentUser = currentUser;
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
     

        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(
               typeof(ApplicationDbContext).Assembly);


        foreach (var entityType in builder.Model
         .GetEntityTypes()
        .Where(e => typeof(BaseEntity)
        .IsAssignableFrom(e.ClrType)))
        {
            var parameter = Expression.Parameter(
                entityType.ClrType,
                "x");

            var property = Expression.Property(
                parameter,
                nameof(BaseEntity.IsDeleted));

            var falseExpression = Expression.Equal(
                property,
                Expression.Constant(false));

            var lambda = Expression.Lambda(
                falseExpression,
                parameter);


            builder.Entity(entityType.ClrType)
                .HasQueryFilter(lambda);
        }

        LeavePolicySeeder.Seed(builder);
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

        var userId = _currentUser?.UserId;


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
    public DbSet<TEntity> Set<TEntity>()
        where TEntity : class
    {
        return base.Set<TEntity>();
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
    public DbSet<EmployeeDocument> EmployeeDocuments { get; set; }
    public DbSet<EmployeeLeave> EmployeeLeaves { get; set; }
    public DbSet<EmployeeLeaveBalance> EmployeeLeaveBalances { get; set; }
    public DbSet<LeavePolicy> LeavePolicies { get; set; }
    public DbSet<EmployeeSpecialLeave> EmployeeSpecialLeaves { get; set; }
    public DbSet<LeaveAttachment> LeaveAttachments { get; set; }
    public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
    public DbSet<WorkSchedule> WorkSchedules { get; set; }
    public DbSet<AttendanceDevice> AttendanceDevices { get; set; }
    public DbSet<AttendanceLog> AttendanceLogs { get; set; }
    public DbSet<EmployeeAttendanceDevice> EmployeeAttendanceDevices { get; set; }
    public DbSet<AttendanceTransaction> AttendanceTransactions { get; set; }
    public DbSet<AttendancePolicy> AttendancePolicies { get; set; }
    public DbSet<AttendancePerformance> AttendancePerformances { get; set; }
    public DbSet<AttendanceCorrection> AttendanceCorrections { get; set; }
    public DbSet<Holiday> Holidays { get; set; }
    public DbSet<SalaryComponent> SalaryComponents { get; set; }

    public DbSet<EmployeeSalaryComponent> EmployeeSalaryComponents { get; set; }

    public DbSet<PayrollPeriod> PayrollPeriods { get; set; }

    public DbSet<Payroll> Payrolls { get; set; }

    public DbSet<PayrollItem> PayrollItems { get; set; }

    public DbSet<PayrollAdjustment> PayrollAdjustments { get; set; }

    public DbSet<EmployeeOvertime> EmployeeOvertimes { get; set; }

    public DbSet<PayrollPolicy> PayrollPolicys { get; set; }
    public DbSet<OvertimePolicy> OvertimePolicies { get; set; }
    public DbSet<EmployeeBankAccount> EmployeeBankAccounts { get; set; }
    public DbSet<Position> Positions { get; set; }

    public DbSet<EmployeeLoan> EmployeeLoans { get; set; } = null!;
    public DbSet<EvaluationPeriod> EvaluationPeriods =>
    Set<EvaluationPeriod>();

    public DbSet<EvaluationTemplate> EvaluationTemplates =>
        Set<EvaluationTemplate>();

    public DbSet<EvaluationCriterion> EvaluationCriteria =>
        Set<EvaluationCriterion>();

    public DbSet<EmployeeEvaluation> EmployeeEvaluations =>
        Set<EmployeeEvaluation>();

    public DbSet<EmployeeEvaluationItem> EmployeeEvaluationItems =>
        Set<EmployeeEvaluationItem>();

    public DbSet<EmployeeLoanInstallment> EmployeeLoanInstallments { get; set; } = null!;

    public DbSet<Ticket> Tickets { get; set; } = null!;

    public DbSet<TicketCategory> TicketCategories { get; set; } = null!;

    public DbSet<TicketComment> TicketComments { get; set; } = null!;

    public DbSet<TicketAssignment> TicketAssignments { get; set; } = null!;

    public DbSet<TicketAttachment> TicketAttachments { get; set; } = null!;

    public DbSet<TicketHistory> TicketHistories { get; set; } = null!;
}