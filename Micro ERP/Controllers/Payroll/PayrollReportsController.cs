using MicroERP.Application.Features.Payrolls.Interfaces;
using MicroERP.Application.Features.Payrolls.Reports.DTOs;
using MicroERP.Application.Features.Payrolls.Reports.Helpers;
using MicroERP.Application.Features.Payrolls.Reports.Interfaces;
using MicroERP.Application.Features.Payrolls.Reports.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.Auth;

[Route("api/payroll-reports")]
[ApiController]
[Authorize(Policy = HRPermissions.Payroll.View)]
public class PayrollReportsController : ControllerBase
{
    #region Services
    private readonly IPayrollReportQueries _queries;
    private readonly IPayrollExcelExportService _excelExportService;
    private readonly IPayrollQueries _payroll;
    private readonly IEmployeePayslipPdfService _employeePayslipPdf;
    private readonly IPayrollSummaryPdfService _payrollSummaryPdfService;
    private readonly IPayrollStatisticsPdfService _payrollStatisticsPdfService;
    private readonly IPayrollDetailsPdfService _payrollDetailsPdfService;
    private readonly IEmployeePayrollHistoryPdfService _employeePayrollHistoryPdf;
    private readonly IAdjustmentPdfService _adjustmentPdf;
    #endregion

    public PayrollReportsController(
        IPayrollReportQueries queries, IPayrollExcelExportService excelExportService, IPayrollQueries payroll, IEmployeePayslipPdfService employeePayslipPdf, IPayrollSummaryPdfService payrollSummaryPdfService, IPayrollStatisticsPdfService payrollStatisticsPdfService, IPayrollDetailsPdfService payrollDetailsPdfService, IEmployeePayrollHistoryPdfService employeePayrollHistoryPdf, IAdjustmentPdfService adjustmentPdf)
    {
        _queries = queries;
        _excelExportService = excelExportService;
        _payroll = payroll;
        _employeePayslipPdf = employeePayslipPdf;
        _payrollSummaryPdfService = payrollSummaryPdfService;
        _payrollStatisticsPdfService = payrollStatisticsPdfService;
        _payrollDetailsPdfService = payrollDetailsPdfService;
        _employeePayrollHistoryPdf = employeePayrollHistoryPdf;
        _adjustmentPdf = adjustmentPdf;
    }



    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(
    [FromQuery] PayrollSummaryFilterDto filter,
     CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetPayrollSummaryAsync(
                filter,
                cancellationToken);


        if (!result.Success)
            return BadRequest(result);


        return Ok(result);
    }

    // =========================================================
    // Get Payroll Trend
    // =========================================================

    [HttpGet("trend")]
    public async Task<IActionResult> GetTrend(
        [FromQuery] PayrollTrendFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetPayrollTrendAsync(
                filter,
                cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("details")]
    public async Task<IActionResult> GetDetails(
    [FromQuery] PayrollDetailsFilterDto filter,
    CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetPayrollDetailsAsync(
                filter,
                cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetEmployeePayrollHistory(
    [FromQuery] EmployeePayrollHistoryFilterDto filter,
    CancellationToken cancellationToken)
    {
        var result = await _queries.GetEmployeePayrollHistoryAsync(
            filter,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("{payrollId:int}/payslip")]
    public async Task<IActionResult> GetEmployeePayslip(
    int payrollId,
    CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetEmployeePayslipAsync(
                payrollId,
                cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics(
    [FromQuery] PayrollStatisticsFilterDto filter,
    CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetPayrollStatisticsAsync(
                filter,
                cancellationToken);


        if (!result.Success)
            return BadRequest(result);


        return Ok(result);
    }

    [HttpGet("salary-components")]
    public async Task<IActionResult> GetSalaryComponents(
    [FromQuery] SalaryComponentReportFilterDto filter,
    CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetSalaryComponentReportAsync(
                filter,
                cancellationToken);


        if (!result.Success)
            return BadRequest(result);


        return Ok(result);
    }

    [HttpGet("adjustments")]
    public async Task<IActionResult> GetAdjustments(
    [FromQuery] AdjustmentReportFilterDto filter,
    CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetAdjustmentReportAsync(
                filter,
                cancellationToken);


        if (!result.Success)
            return BadRequest(result);


        return Ok(result);
    }

    [HttpGet("attendance-deductions")]
    public async Task<IActionResult> GetAttendanceDeductions(
    [FromQuery] AttendanceDeductionReportFilterDto filter,
    CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetAttendanceDeductionReportAsync(
                filter,
                cancellationToken);


        if (!result.Success)
            return BadRequest(result);


        return Ok(result);
    }

    [HttpGet("bank-transfer")]
    public async Task<IActionResult> GetBankTransferReport(
    [FromQuery] BankTransferReportFilterDto filter,
    CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetBankTransferReportAsync(
                filter,
                cancellationToken);


        if (!result.Success)
            return BadRequest(result);


        return Ok(result);
    }

    //======================== ExportPayroll ================

    [HttpGet("export/payroll-summary")]
    public async Task<IActionResult> ExportPayrollSummary(
    [FromQuery] PayrollSummaryFilterDto filter,
    CancellationToken cancellationToken)
    {
        var file =
            await _excelExportService
            .ExportPayrollSummaryAsync(
                filter,
                cancellationToken);

        var fileType = ".xlsx";
        var (period, fileName) =
        await PayrollReportHelper
            .GetPeriodAndFileNameAsync(
                _payroll,
                filter.PayrollPeriodId,
                "Payroll-Summary",
                fileType,
                cancellationToken);

              return File(
           file,
           "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
           fileName);
    }

    [HttpGet("export/payroll-details")]
    [HttpGet("details/export")]
    public async Task<IActionResult> ExportPayrollDetails(
    [FromQuery] PayrollDetailsFilterDto filter,
    CancellationToken cancellationToken)
    {
        var file =
            await _excelExportService
                .ExportPayrollDetailsAsync(
                    filter,
                    cancellationToken);

        var (_, fileName) =
            await PayrollReportHelper
                .GetPayrollDetailsFileInfoAsync(
                    _queries,
                    filter.PayrollId,
                    cancellationToken);

        return File(
            file,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    [HttpGet("export/employee-payroll-history")]
    public async Task<IActionResult> ExportEmployeePayrollHistory(
    [FromQuery] EmployeePayrollHistoryFilterDto filter,
    CancellationToken cancellationToken)
    {
        var file =
            await _excelExportService
            .ExportEmployeePayrollHistoryAsync(
                filter,
                cancellationToken);


        return File(
            file,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "Employee-Payroll-History.xlsx");
    }

    [HttpGet("export/payroll-statistics")]
    public async Task<IActionResult> ExportPayrollStatistics(
    [FromQuery] PayrollStatisticsFilterDto filter,
    CancellationToken cancellationToken)
    {
        var file =
            await _excelExportService
            .ExportPayrollStatisticsAsync(
                filter,
                cancellationToken);

        var fileType = ".xlsx";
        var (period, fileName) =
      await PayrollReportHelper
          .GetPeriodAndFileNameAsync(
              _payroll,
              filter.PayrollPeriodId,
               "Payroll-Statistics",
               fileType,
              cancellationToken);

        return File(
     file,
     "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
     fileName);

    }

    [HttpGet("export/salary-components")]
    public async Task<IActionResult> ExportSalaryComponents(
    [FromQuery] SalaryComponentReportFilterDto filter,
    CancellationToken cancellationToken)
    {
        var file =
            await _excelExportService
            .ExportSalaryComponentReportAsync(
                filter,
                cancellationToken);
        var fileType = ".xlsx";

        var (period, fileName) =
       await PayrollReportHelper
      .GetPeriodAndFileNameAsync(
          _payroll,
          filter.PayrollPeriodId,
           "Salary-Components",
           fileType,
          cancellationToken);

        return File(
     file,
     "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
     fileName);

    }

    [HttpGet("export/attendance-deductions")]
    public async Task<IActionResult> ExportAttendanceDeductions(
    [FromQuery] AttendanceDeductionReportFilterDto filter,
    CancellationToken cancellationToken)
    {
        var file =
            await _excelExportService
            .ExportAttendanceDeductionReportAsync(
                filter,
                cancellationToken);

        var fileType = ".xlsx";
        var (period, fileName) =
 await PayrollReportHelper
     .GetPeriodAndFileNameAsync(
         _payroll,
         filter.PayrollPeriodId,
          "Attendance-Deductions",
          fileType,
         cancellationToken);

        return File(
     file,
     "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
     fileName);
    }

    [HttpGet("export/adjustments")]
    public async Task<IActionResult> ExportAdjustments(
    [FromQuery] AdjustmentReportFilterDto filter,
    CancellationToken cancellationToken)
    {
        var file =
            await _excelExportService
            .ExportAdjustmentReportAsync(
                filter,
                cancellationToken);
        var fileType = ".xlsx";

        var (period, fileName) =
  await PayrollReportHelper
      .GetPeriodAndFileNameAsync(
          _payroll,
          filter.PayrollPeriodId,
           "Adjustments",
           fileType,
          cancellationToken);

        return File(
     file,
     "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
     fileName);

    }
   
    [HttpGet("export/bank-transfer")]
    public async Task<IActionResult> ExportBankTransfer(
    [FromQuery] BankTransferReportFilterDto filter,
    CancellationToken cancellationToken)
    {
        var file =
            await _excelExportService
            .ExportBankTransferReportAsync(
                filter,
                cancellationToken);
        var fileType = ".xlsx";

        var (period, fileName) =
 await PayrollReportHelper
     .GetPeriodAndFileNameAsync(
         _payroll,
         filter.PayrollPeriodId,
          "Bank-Transfers",
          fileType,
         cancellationToken);

        return File(
     file,
     "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
     fileName);
    }

    [HttpGet("{payrollId:int}/payslip/pdf")]
    public async Task<IActionResult> ExportPayslipPdf(
    int payrollId,
    CancellationToken cancellationToken)
    {
        var file =
            await _employeePayslipPdf.GenerateAsync(
                payrollId,
                cancellationToken);

        return File(
            file,
            "application/pdf",
            $"Payslip-{payrollId}.pdf");
    }

    [HttpGet("summary/pdf")]
    public async Task<IActionResult> ExportPayrollSummaryPdf(
    [FromQuery] PayrollSummaryFilterDto filter,
    CancellationToken cancellationToken)
    {
        var file =
            await _payrollSummaryPdfService.GenerateAsync(
                filter,
                cancellationToken);

        var fileName =
            $"Payroll-Summary-{DateTime.Now:yyyyMMdd}.pdf";

        return File(
            file,
            "application/pdf",
            fileName);
    }

    [HttpGet("statistics/export-pdf")]
    public async Task<IActionResult> ExportPayrollStatisticsPdf(
    [FromQuery] PayrollStatisticsFilterDto filter,
    CancellationToken cancellationToken)
    {
        var file =
            await _payrollStatisticsPdfService.GenerateAsync(
                filter,
                cancellationToken);

        return File(
            file,
            "application/pdf",
            $"PayrollStatistics_{DateTime.Now:yyyyMMdd}.pdf");
    }

    [HttpGet("details/export-pdf")]
    public async Task<IActionResult> ExportPayrollDetailsPdf(
    [FromQuery] PayrollDetailsFilterDto filter,
    CancellationToken cancellationToken)
    {
        var file =
            await _payrollDetailsPdfService.GenerateAsync(
                filter,
                cancellationToken);

        return File(
            file,
            "application/pdf",
            $"PayrollDetails_{filter.PayrollId}.pdf");
    }

    [HttpGet("employee/History/export-pdf")]
    public async Task<IActionResult> ExportEmployeePayrollHistoryPdf(
    [FromQuery] EmployeePayrollHistoryFilterDto filter,
    CancellationToken cancellationToken)
    {
        var file =
            await _employeePayrollHistoryPdf.GenerateAsync(
                filter,
                cancellationToken);

        return File(
            file,
            "application/pdf",
            $"Employee-Payroll-History_EmployeeId={filter.EmployeeId}.pdf");
    }

    [HttpGet("exportPdf/adjustments")]
    public async Task<IActionResult> ExportAdjustmentsPdf(
   [FromQuery] AdjustmentReportFilterDto filter,
   CancellationToken cancellationToken)
    {
        var file =
          await _adjustmentPdf.GenerateAsync(
              filter,
              cancellationToken);


        var fileType = ".pdf";

        var (period, fileName) =
  await PayrollReportHelper
      .GetPeriodAndFileNameAsync(
          _payroll,
          filter.PayrollPeriodId,
           "Adjustments",
           fileType,
          cancellationToken);

        return File(
     file,
     "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
     fileName);

    }

}