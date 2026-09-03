using MicroERP.Application.Features.Payrolls.DTOs;
using MicroERP.Application.Features.Payrolls.Interfaces;
using MicroERP.Application.Features.Payrolls.Reports.DTOs;
using MicroERP.Application.Features.Payrolls.Reports.Interfaces;

namespace MicroERP.Application.Features.Payrolls.Reports.Helpers;

public static class PayrollReportHelper
{
    public static async Task<(PayrollPeriodDto Period, string FileName)>
        GetPeriodAndFileNameAsync(
            IPayrollQueries payroll,
            int payrollPeriodId,
            string reportName,
            string fileType,
            CancellationToken cancellationToken = default)
    {
        var periodResult =
            await payroll.GetPeriodByIdAsync(
                payrollPeriodId,
                cancellationToken);

        if (!periodResult.Success ||
            periodResult.Data == null)
        {
            throw new Exception(
                periodResult.Message ??
                "Payroll period not found.");
        }

        var period =
            periodResult.Data;

        var fileName =
            $"{reportName}-{period.Year}-{period.Month:00}{fileType}";

        return (
            period,
            fileName);
    }

    public static async Task<(DTOs.PayrollDetailsDto Data, string FileName)>
    GetPayrollDetailsFileInfoAsync(
        IPayrollReportQueries queries,
        int payrollId,
        CancellationToken cancellationToken = default)
    {
        var result =
            await queries.GetPayrollDetailsAsync(
                new PayrollDetailsFilterDto
                {
                    PayrollId = payrollId
                },
                cancellationToken);

        if (!result.Success ||
            result.Data == null)
        {
            throw new Exception(
                result.Message ??
                "Payroll not found.");
        }

        var data = result.Data;

        var employeeName =
            string.Join(
                "-",
                data.EmployeeName
                    .Split(
                        Path.GetInvalidFileNameChars(),
                        StringSplitOptions.RemoveEmptyEntries));

        var fileName =
            $"Payroll-Details-{employeeName}-{data.Period.Replace("/", "-")}.xlsx";

        return (data, fileName);
    }
}