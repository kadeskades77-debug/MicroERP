using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Features.Authorization.PermissionGroups.Interfaces;
using MicroERP.Application.Features.Authorization.Permissions.Interfaces;
using MicroERP.Application.Features.Departments.Interfaces;
using MicroERP.Application.Features.Documents.EmployeeDocuments.Interfaces;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using MicroERP.Application.Features.EmployeeAttendance.Holidays.Interfaces;
using MicroERP.Application.Features.EmployeeAttendance.WorkSchedules.Interfaces;
using MicroERP.Application.Features.Employees.Interfaces;
using MicroERP.Application.Features.Leaves.EmployeeLeaveBalances.Interfaces;
using MicroERP.Application.Features.Leaves.EmployeeLeaves.Interfaces;
using MicroERP.Application.Features.Leaves.EmployeeSpecialLeaves.Interfaces;
using MicroERP.Application.Features.Payrolls.Adjustments.Interfaces;
using MicroERP.Application.Features.Payrolls.Interfaces;
using MicroERP.Application.Features.Payrolls.SalaryComponents.Interfaces;
using MicroERP.Infrastructure.Services;
using MicroERP.Persistence.Authorization;
using MicroERP.Persistence.Queries;
using MicroERP.Persistence.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MicroERP.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")
            ));
        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        services.AddScoped<IPermissionGroupService, PermissionGroupService>();
        services.AddScoped<PermissionSeeder>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDepartmentQueries, DepartmentQueries>();
        services.AddScoped<IEmployeeQueries, EmployeeQueries>();
        services.AddScoped<IWorkScheduleQueries, WorkScheduleQueries>();
        services.AddScoped<IPermissionGroupQueries, PermissionGroupQueries>(); 
        services.AddScoped<IPermissionQueries, PermissionQueries>(); 
        services.AddScoped<IEmployeeDocumentQueries, EmployeeDocumentQueries>();
        services.AddScoped<IEmployeeLeaveQueries, EmployeeLeaveQueries>();
        services.AddScoped<IEmployeeSpecialLeaveQueries, EmployeeSpecialLeaveQueries>();
        services.AddScoped<IEmployeeLeaveBalanceQueries, EmployeeLeaveBalanceQueries>();
        services.AddScoped<IAttendanceQueries,AttendanceQueries>();
        services.AddScoped<IAttendanceDeviceQueries,AttendanceDeviceQueries>();
        services.AddScoped<IHolidayQueries, HolidayQueries>();
        services.AddScoped<IPayrollQueries, PayrollQueries>();
        services.AddScoped<ISalaryComponentQueries, SalaryComponentQueries>();
        services.AddScoped<IPayrollAdjustmentQueries,PayrollAdjustmentQueries>();
        services.AddScoped<IEmployeeOvertimeQueries, EmployeeOvertimeQueries>();
        services.AddScoped<IEmployeeLeaveReportQueries,EmployeeLeaveReportQueries>();
        services.AddScoped<ILeaveBalanceReportQueries, LeaveBalanceReportQueries>();
        services.AddScoped<IEmployeeSpecialLeaveReportQueries, EmployeeSpecialLeaveReportQueries>();
        services.AddScoped<IAttendancePerformanceQueries,AttendancePerformanceQueries>();
        // AutoMapper
        services.AddAutoMapper(
          Assembly.GetExecutingAssembly());

        return services;

    }
}