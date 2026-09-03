using MicroERP.Application.Features.Payrolls.Reports.DTOs;

namespace MicroERP.Application.Features.Payrolls.Reports.Interfaces
{
    public interface IPayrollExcelExportService
    {
        Task<byte[]> ExportPayrollSummaryAsync(
            PayrollSummaryFilterDto filter,
            CancellationToken cancellationToken = default);


        Task<byte[]> ExportPayrollDetailsAsync(
            PayrollDetailsFilterDto filter,
            CancellationToken cancellationToken = default);


        Task<byte[]> ExportEmployeePayrollHistoryAsync(
            EmployeePayrollHistoryFilterDto filter,
            CancellationToken cancellationToken = default);


        Task<byte[]> ExportPayrollStatisticsAsync(
            PayrollStatisticsFilterDto filter,
            CancellationToken cancellationToken = default);


        Task<byte[]> ExportSalaryComponentReportAsync(
            SalaryComponentReportFilterDto filter,
            CancellationToken cancellationToken = default);


        Task<byte[]> ExportAttendanceDeductionReportAsync(
            AttendanceDeductionReportFilterDto filter,
            CancellationToken cancellationToken = default);


        Task<byte[]> ExportAdjustmentReportAsync(
            AdjustmentReportFilterDto filter,
            CancellationToken cancellationToken = default);


        Task<byte[]> ExportBankTransferReportAsync(
            BankTransferReportFilterDto filter,
            CancellationToken cancellationToken = default);
    }
}
