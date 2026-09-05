using MicroERP.Domain.Audit;
using MicroERP.Domain.Identity;
using MicroERP.Domin.Entities.EmployeeAttendance;
using MicroERP.Domin.Entities.EmployeeEvaluations;
using MicroERP.Domin.Entities.EmployeeLeaves;
using MicroERP.Domin.Entities.Employees;
using MicroERP.Domin.Entities.Payrolls;
using MicroERP.Domin.Entities.Policies;
using MicroERP.Domin.Entities.Ticketing;
using MicroERP.Domin.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace MicroERP.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<TEntity> Set<TEntity>()
       where TEntity : class;
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
        DbSet<ApplicationUser> Users { get; }
        DbSet<AuditLog> AuditLogs { get; }
        DbSet<EmployeeLeave> EmployeeLeaves { get; }
        DbSet<EmployeeLeaveBalance> EmployeeLeaveBalances { get; }
        DbSet<LeavePolicy> LeavePolicies { get; }
        DbSet<EmployeeSpecialLeave> EmployeeSpecialLeaves { get; }
        DbSet<LeaveAttachment> LeaveAttachments { get; }
        DbSet<AttendanceRecord> AttendanceRecords { get; }
        DbSet<WorkSchedule> WorkSchedules { get; }
        DbSet<AttendanceDevice> AttendanceDevices { get; }
        DbSet<AttendanceLog> AttendanceLogs { get; }
        DbSet<AttendanceTransaction> AttendanceTransactions { get; }
        DbSet<EmployeeAttendanceDevice> EmployeeAttendanceDevices { get; }
        DbSet<AttendancePolicy> AttendancePolicies { get; }
        DbSet<AttendancePerformance> AttendancePerformances { get; }
        DbSet<Holiday> Holidays { get; }
        DbSet<SalaryComponent> SalaryComponents { get; }
        DbSet<AttendanceCorrection> AttendanceCorrections { get; }
         DbSet<EmployeeSalaryComponent> EmployeeSalaryComponents { get; }
         DbSet<PayrollPeriod> PayrollPeriods { get; }
         DbSet<Payroll> Payrolls { get; }
         DbSet<PayrollItem> PayrollItems { get; }
         DbSet<PayrollAdjustment> PayrollAdjustments { get;}
         DbSet<EmployeeOvertime> EmployeeOvertimes { get; }
        DbSet<PayrollPolicy> PayrollPolicys { get; }
        DbSet<OvertimePolicy> OvertimePolicies { get; }
        DbSet<EmployeeBankAccount> EmployeeBankAccounts { get; }
        DbSet<Position> Positions { get; }
        DbSet<EmployeeLoan> EmployeeLoans { get; }
        DbSet<EmployeeLoanInstallment> EmployeeLoanInstallments { get; }
         DbSet<EvaluationPeriod> EvaluationPeriods { get; }
        DbSet<EvaluationTemplate> EvaluationTemplates { get; }
         DbSet<EvaluationCriterion> EvaluationCriteria { get; }
         DbSet<EmployeeEvaluation> EmployeeEvaluations { get; }
        DbSet<EmployeeEvaluationItem> EmployeeEvaluationItems { get; }
         DbSet<Ticket> Tickets { get; }
         DbSet<TicketCategory> TicketCategories { get; }
        DbSet<TicketComment> TicketComments { get; }
        DbSet<TicketAssignment> TicketAssignments { get; }
        DbSet<TicketAttachment> TicketAttachments { get; }
        DbSet<TicketHistory> TicketHistories { get; }
        DatabaseFacade Database { get; }
        Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}