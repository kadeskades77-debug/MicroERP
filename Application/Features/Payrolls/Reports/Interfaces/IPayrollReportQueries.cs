using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Payrolls.Reports.DTOs;

namespace MicroERP.Application.Features.Payrolls.Reports.Interfaces;

public interface IPayrollReportQueries
{
    Task<Result<PayrollSummaryReportDto>>
        GetPayrollSummaryAsync( 
            PayrollSummaryFilterDto filter,
            CancellationToken cancellationToken = default);

    Task<Result<List<PayrollTrendDto>>>
    GetPayrollTrendAsync(
        PayrollTrendFilterDto filter,
        CancellationToken cancellationToken = default);


    Task<Result<PayrollDetailsDto>>
        GetPayrollDetailsAsync(
            PayrollDetailsFilterDto filter,
            CancellationToken cancellationToken = default);


    Task<Result<List<EmployeePayrollHistoryDto>>>
        GetEmployeePayrollHistoryAsync(
            EmployeePayrollHistoryFilterDto filter,
            CancellationToken cancellationToken = default);


    Task<Result<PayrollStatisticsDto>>
        GetPayrollStatisticsAsync(
            PayrollStatisticsFilterDto filter,
            CancellationToken cancellationToken = default);

    Task<Result<EmployeePayslipDto>> GetEmployeePayslipAsync(int payrollId,
      CancellationToken cancellationToken = default);


    Task<Result<List<SalaryComponentReportDto>>>
        GetSalaryComponentReportAsync(
            SalaryComponentReportFilterDto filter,
            CancellationToken cancellationToken = default);


    Task<Result<List<AttendanceDeductionReportDto>>>
        GetAttendanceDeductionReportAsync(
            AttendanceDeductionReportFilterDto filter,
            CancellationToken cancellationToken = default);


    Task<Result<List<AdjustmentReportDto>>>
        GetAdjustmentReportAsync(
            AdjustmentReportFilterDto filter,
            CancellationToken cancellationToken = default);


    Task<Result<BankTransferReportDto>>
        GetBankTransferReportAsync(
            BankTransferReportFilterDto filter,
            CancellationToken cancellationToken = default);
}