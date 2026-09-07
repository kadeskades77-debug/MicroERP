using FluentValidation;
using MicroERP.Application.Authorization.Interfaces;
using MicroERP.Application.Authorization.Providers;
using MicroERP.Application.Authorization.Services;
using MicroERP.Application.Common.Excel.Interfaces;
using MicroERP.Application.Common.Excel.Services;
using MicroERP.Application.Common.Files;
using MicroERP.Application.Common.Files.Interfaces;
using MicroERP.Application.Features.Authorization.RolePermissionGroup.Interfaces;
using MicroERP.Application.Features.Authorization.RolePermissionGroup.Services;
using MicroERP.Application.Features.Departments.Interfaces;
using MicroERP.Application.Features.Departments.Services;
using MicroERP.Application.Features.Documents.EmployeeDocuments.Interfaces;
using MicroERP.Application.Features.Documents.EmployeeDocuments.Service;
using MicroERP.Application.Features.Documents.LeaveDocuments.Interfaces;
using MicroERP.Application.Features.Documents.LeaveDocuments.Service;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Services;
using MicroERP.Application.Features.EmployeeAttendance.AttendanceLogs.Calculators;
using MicroERP.Application.Features.EmployeeAttendance.AttendanceLogs.Helpers;
using MicroERP.Application.Features.EmployeeAttendance.AttendanceLogs.Interfaces;
using MicroERP.Application.Features.EmployeeAttendance.AttendanceLogs.Services;
using MicroERP.Application.Features.EmployeeAttendance.AttendanceTransactions.Interfaces;
using MicroERP.Application.Features.EmployeeAttendance.AttendanceTransactions.Services;
using MicroERP.Application.Features.EmployeeAttendance.Holidays.Interfaces;
using MicroERP.Application.Features.EmployeeAttendance.Holidays.Services;
using MicroERP.Application.Features.EmployeeAttendance.WorkSchedules.Interfaces;
using MicroERP.Application.Features.EmployeeAttendance.WorkSchedules.Services;
using MicroERP.Application.Features.EmployeeEvaluations.Interfaces;
using MicroERP.Application.Features.EmployeeEvaluations.Queries;
using MicroERP.Application.Features.EmployeeEvaluations.Queries.MicroERP.Application.Features.EmployeeEvaluations.Excel;
using MicroERP.Application.Features.EmployeeEvaluations.Services;
using MicroERP.Application.Features.Employees.Interfaces;
using MicroERP.Application.Features.Employees.Services;
using MicroERP.Application.Features.Leaves.EmployeeLeaveBalances.Interfaces;
using MicroERP.Application.Features.Leaves.EmployeeLeaveBalances.Services;
using MicroERP.Application.Features.Leaves.EmployeeLeaves.Interfaces;
using MicroERP.Application.Features.Leaves.EmployeeLeaves.Services;
using MicroERP.Application.Features.Leaves.EmployeeSpecialLeaves.Interfaces;
using MicroERP.Application.Features.Leaves.EmployeeSpecialLeaves.Services;
using MicroERP.Application.Features.Payrolls.Adjustments.Interfaces;
using MicroERP.Application.Features.Payrolls.Adjustments.Services;
using MicroERP.Application.Features.Payrolls.EmployeeLoans.Interfaces;
using MicroERP.Application.Features.Payrolls.EmployeeLoans.Services;
using MicroERP.Application.Features.Payrolls.Interfaces;
using MicroERP.Application.Features.Payrolls.Reports.Interfaces;
using MicroERP.Application.Features.Payrolls.Reports.Queries;
using MicroERP.Application.Features.Payrolls.SalaryComponents.Interfaces;
using MicroERP.Application.Features.Payrolls.SalaryComponents.Services;
using MicroERP.Application.Features.Payrolls.Services;
using MicroERP.Application.Features.Payrolls.Services.Calculators;
using MicroERP.Application.Features.Positions.Interfaces;
using MicroERP.Application.Features.Positions.Queries;
using MicroERP.Application.Features.Positions.Services;
using MicroERP.Application.Features.Ticketing.Interfaces;
using MicroERP.Application.Features.Ticketing.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MicroERP.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddScoped<IPermissionDefinitionService,PermissionDefinitionService>();
        services.AddScoped<IMultiPermissionDefinitionProvider, TicketDefinitionProvider>();
        services.AddScoped<IRolePermissionGroupService,RolePermissionGroupService>();


        #region Employees

        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IPositionService, PositionService>();
        services.AddScoped<IPositionQueries, PositionQueries>();
        services.AddScoped<IEmployeeDocumentService,EmployeeDocumentService>();
        services.AddScoped<IEmployeeDocumentService, EmployeeDocumentService>();
        services.AddScoped<IFileValidationService,FileValidationService>();
        #endregion


        #region EmployeeEvaluations

        services.AddScoped<IEvaluationCalculationService,EvaluationCalculationService>();
        services.AddScoped<IEvaluationTemplateService,EvaluationTemplateService>();
        services.AddScoped<IEvaluationPeriodService,EvaluationPeriodService>();
        services.AddScoped<IEmployeeEvaluationService,EmployeeEvaluationService>();
        services.AddScoped<IEmployeeEvaluationExcelExportService,EmployeeEvaluationExcelExportService>();
        services.AddScoped<IEmployeeEvaluationQueries,EmployeeEvaluationQueries>();
        #endregion



        #region Leaves

        services.AddScoped<IEmployeeLeaveService, EmployeeLeaveService>();
        services.AddScoped<IEmployeeSpecialLeaveService,EmployeeSpecialLeaveService>();
        services.AddScoped<IEmployeeLeaveBalanceService,EmployeeLeaveBalanceService>();
        services.AddScoped<ILeaveBalanceGenerator,LeaveBalanceGeneratorService>();
        services.AddScoped<ILeaveAttachmentService,LeaveAttachmentService>();
        services.AddScoped<IHolidayService, HolidayService>();
        services.AddScoped<ILeaveAttachmentService, LeaveAttachmentService>();
        #endregion

        #region Attendance

        services.AddScoped<IAttendanceService,AttendanceService>();
        services.AddScoped<IAttendanceDeviceService, AttendanceDeviceService>();
        services.AddScoped<IAttendanceLogProcessor, AttendanceLogProcessor>();
        services.AddScoped<IAttendanceTransactionService,AttendanceTransactionService>();
        services.AddScoped<IShiftResolver, ShiftResolver>();
        services.AddScoped<IAttendanceCalculator, AttendanceCalculator>();
        services.AddScoped<IAttendancePolicyService, AttendancePolicyService>();
        services.AddScoped<AttendancePerformanceCalculator>();
        services.AddScoped<IAttendancePerformanceService, AttendancePerformanceService>();
        services.AddScoped<IAttendanceReportQueries,AttendanceReportService>();
        services.AddScoped<IAttendanceExcelExportService,AttendanceExcelExportService>();
        services.AddScoped<IExcelExportService,ExcelExportService>();
        services.AddScoped<IMyAttendanceQueries,MyAttendanceQueries>();
        services.AddScoped<IAttendanceRecalculateService, AttendanceRecalculateService>();
        services.AddScoped<IAttendanceCorrectionService, AttendanceCorrectionService>();
        services.AddScoped<IEmployeeOvertimeService, EmployeeOvertimeService>();
        services.AddScoped<IOvertimeCalculationService,OvertimeCalculationService>();
        services.AddScoped<IAttendanceOvertimeService, AttendanceOvertimeService>();
        services.AddScoped<IOvertimeReportQueries, OvertimeReportQueries>();
        services.AddScoped<IOvertimeExcelExportService,OvertimeExcelExportService>();
        services.AddScoped<IEmployeeOvertimePdfService,EmployeeOvertimePdfService>();
        services.AddScoped<IWorkScheduleService,WorkScheduleService>();
#endregion

        # region Payroll

        services.AddScoped<IPayrollService, PayrollService>();
        services.AddScoped<IEmployeeSalaryComponentService,EmployeeSalaryComponentService>();
        services.AddScoped<ISalaryComponentService, SalaryComponentService>();
        services.AddScoped<IPayrollCalculator, PayrollCalculator>();
        services.AddScoped<IAttendancePayrollCalculator,AttendancePayrollCalculator>();
        services.AddScoped<IPayrollAdjustmentService,PayrollAdjustmentService>();
        services.AddScoped<IPayrollReportQueries, PayrollReportQueries>();
        services.AddScoped<IPayrollExcelExportService,PayrollExcelExportService>();
        services.AddScoped<IUnpaidLeavePayrollCalculator,UnpaidLeavePayrollCalculator>();
        services.AddScoped<OvertimePayrollCalculator>();
        services.AddScoped<IEmployeePayslipPdfService,EmployeePayslipPdfService>();
        services.AddScoped<IPayrollSummaryPdfService,PayrollSummaryPdfService>();
        services.AddScoped<IPayrollStatisticsPdfService,PayrollStatisticsPdfService>();
        services.AddScoped<IPayrollDetailsPdfService,PayrollDetailsPdfService>();
        services.AddScoped<IAdjustmentPdfService,AdjustmentPdfService>();
        services.AddScoped<IEmployeePayrollHistoryPdfService,EmployeePayrollHistoryPdfService>();
        services.AddScoped<IEmployeeLoanService,EmployeeLoanService>();
        services.AddScoped<IEmployeeLoanInstallmentService,EmployeeLoanInstallmentService>();

        #endregion

        #region Tickets

        services.AddScoped<ITicketService, TicketService>();
        services.AddScoped<ITicketCategoryService, TicketCategoryService>();
        services.AddScoped<ITicketCommentService, TicketCommentService>();
        services.AddScoped<ITicketAssignmentService, TicketAssignmentService>();
        services.AddScoped<ITicketAttachmentService, TicketAttachmentService>();
        services.AddScoped<ITicketHistoryService, TicketHistoryService>();
        services.AddScoped<ITicketNumberGenerator, TicketNumberGenerator>();
        #endregion

        return services;
    }
}