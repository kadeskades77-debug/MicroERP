
namespace MicroERP.Application.Features.Payrolls.Reports.Queries
{
    
    using MicroERP.Application.Common.Excel.Interfaces;
    using MicroERP.Application.Common.Excel.Models;
    using MicroERP.Application.Features.Payrolls.Reports.DTOs;
    using MicroERP.Application.Features.Payrolls.Reports.Interfaces;
    using MicroERP.Domin.Enums;

    public class PayrollExcelExportService
     : IPayrollExcelExportService
    {
        private readonly IPayrollReportQueries _queries;
        private readonly IExcelExportService _excelExport;


        public PayrollExcelExportService(
            IPayrollReportQueries queries,
            IExcelExportService excelExport)
        {
            _queries = queries;
            _excelExport = excelExport;
        }



        public async Task<byte[]> ExportPayrollSummaryAsync(
     PayrollSummaryFilterDto filter,
     CancellationToken cancellationToken = default)
        {
            var result =
                await _queries.GetPayrollSummaryAsync(
                    filter,
                    cancellationToken);

            if (!result.Success)
            {
                throw new InvalidOperationException(
                    result.Message);
            }

            var data = result.Data!;

            var rows =
                data.Payrolls
                    .Select(x => new ExcelSheetRow
                    {
                        Type = ExcelRowType.Data,

                        Values =
                        [
                            x.EmployeeName,
                    x.Department,
                    x.GrossSalary,
                    x.TotalAllowances,
                    x.TotalOvertime,
                    x.TotalDeductions,
                    x.NetSalary,
                    x.Status.ToString()
                        ]
                    })
                    .ToList();

            // =========================================================
            // Total Row
            // =========================================================

            rows.Add(
                new ExcelSheetRow
                {
                    Type = ExcelRowType.Total,

                    Values =
                    [
                        "TOTAL",
                "",
                data.TotalGrossSalary,
                data.TotalAllowances,
                data.TotalOvertime,
                data.TotalDeductions,
                data.TotalNetSalary,
                ""
                    ]
                });

            // =========================================================
            // Excel Sheet
            // =========================================================

            var sheet =
                new ExcelSheetData
                {
                    SheetName = "Payroll Summary",

                    Headers =
                    [
                        "Employee",
                "Department",
                "Gross Salary",
                "Allowances",
                "Overtime",
                "Deductions",
                "Net Salary",
                "Status"
                    ],

                    Rows = rows,

                    BoldRowIndex = rows.Count - 1
                };

            return _excelExport.Export(sheet);
        }


        public async Task<byte[]> ExportPayrollDetailsAsync(
      PayrollDetailsFilterDto filter,
      CancellationToken cancellationToken = default)
        {
            var result =
                await _queries.GetPayrollDetailsAsync(
                    filter,
                    cancellationToken);

            if (!result.Success)
            {
                throw new InvalidOperationException(
                    result.Message);
            }

            var data = result.Data!;

            var rows =
                data.Items
                    .Select(x => new ExcelSheetRow
                    {
                        Type = ExcelRowType.Data,

                        Values =
                        [
                            x.ComponentName,
                    x.Type.ToString(),
                    x.Amount
                        ]
                    })
                    .ToList();

            // =========================================================
            // Net Salary
            // =========================================================

            rows.Add(
                new ExcelSheetRow
                {
                    Type = ExcelRowType.Total,

                    Values =
                    [
                        "Net Salary",
                "",
                data.NetSalary
                    ]
                });

            var sheet =
                new ExcelSheetData
                {
                    SheetName = "Payroll Details",

                    TopRows =
                    [
                        new object?[]
                {
                    "Name",
                    data.EmployeeName
                },

                new object?[]
                {
                    "Department",
                    data.Department
                },

                new object?[]
                {
                    "Period",
                    data.Period
                },

                new object?[]
                {
                    "Status",
                    data.Status.ToString()
                }
                    ],

                    SectionTitle = "Payroll Items",

                    Headers =
                    [
                        "Component",
                "Component Type",
                "Amount"
                    ],

                    Rows = rows,

                    BoldRowIndex = rows.Count - 1
                };

            return _excelExport.Export(sheet);
        }


        public async Task<byte[]> ExportEmployeePayrollHistoryAsync(
         EmployeePayrollHistoryFilterDto filter,
         CancellationToken cancellationToken = default)
        {
            var result =
                await _queries.GetEmployeePayrollHistoryAsync(
                    filter,
                    cancellationToken);

            if (!result.Success)
            {
                throw new InvalidOperationException(
                    result.Message);
            }

            var data = result.Data!;

            if (data.Count == 0)
            {
                throw new InvalidOperationException(
                    "No payroll history found.");
            }

            var employee = data.First();

            var rows =
                data.Select(x => new ExcelSheetRow
                {
                    Type = ExcelRowType.Data,

                    Values =
                    [
                        $"{x.Month}/{x.Year}",
                x.GrossSalary,
                x.TotalAllowances,
                x.TotalOvertime,
                x.TotalDeductions,
                x.NetSalary,
                x.Status
                    ]
                })
                .ToList();

            // =========================================================
            // Total Row
            // =========================================================

            rows.Add(
                new ExcelSheetRow
                {
                    Type = ExcelRowType.Total,

                    Values =
                    [
                        "TOTAL",
                data.Sum(x => x.GrossSalary),
                data.Sum(x => x.TotalAllowances),
                data.Sum(x => x.TotalOvertime),
                data.Sum(x => x.TotalDeductions),
                data.Sum(x => x.NetSalary),
                ""
                    ]
                });

            // =========================================================
            // Excel Sheet
            // =========================================================

            var sheet =
                new ExcelSheetData
                {
                    SheetName = "Payroll History",

                    TopRows =
                    [
                        new object?[]
                {
                    "Employee Name",
                    employee.EmployeeName
                },

                new object?[]
                {
                    "Department",
                    employee.Department
                },

                new object?[]
                {
                    "Hire Date",
                    employee.HireDate
                }
                    ],

                    SectionTitle = "Payroll History",

                    Headers =
                    [
                        "Period",
                "Gross Salary",
                "Allowances",
                "Overtime",
                "Deductions",
                "Net Salary",
                "Status"
                    ],

                    Rows = rows,

                    BoldRowIndex = rows.Count - 1
                };

            return _excelExport.Export(sheet);
        }

        public async Task<byte[]> ExportPayrollStatisticsAsync(
       PayrollStatisticsFilterDto filter,
       CancellationToken cancellationToken = default)
        {
            var result =
                await _queries.GetPayrollStatisticsAsync(
                    filter,
                    cancellationToken);

            if (!result.Success)
            {
                throw new InvalidOperationException(
                    result.Message);
            }

            var data = result.Data!;

            var rows =
                new List<ExcelSheetRow>
                {
            new ExcelSheetRow
            {
                Type = ExcelRowType.Summary,

                Values =
                [
                    "Period",
                    data.Period
                ]
            },

            new ExcelSheetRow
            {
                Type = ExcelRowType.Summary,

                Values =
                [
                    "Gross Salary",
                    data.TotalGrossSalary
                ]
            },

            new ExcelSheetRow
            {
                Type = ExcelRowType.Summary,

                Values =
                [
                    "Allowances",
                    data.TotalAllowances
                ]
            },

            new ExcelSheetRow
            {
                Type = ExcelRowType.Summary,

                Values =
                [
                    "Overtime",
                    data.TotalOvertime
                ]
            },

            new ExcelSheetRow
            {
                Type = ExcelRowType.Summary,

                Values =
                [
                    "Deductions",
                    data.TotalDeductions
                ]
            },

            new ExcelSheetRow
            {
                Type = ExcelRowType.Summary,

                Values =
                [
                    "Net Salary",
                    data.TotalNetSalary
                ]
            },

            new ExcelSheetRow
            {
                Type = ExcelRowType.Summary,

                Values =
                [
                    "Average Net Salary",
                    data.AverageNetSalary
                ]
            },

            new ExcelSheetRow
            {
                Type = ExcelRowType.Summary,

                Values =
                [
                    "Highest Net Salary",
                    data.HighestNetSalary
                ]
            },

            new ExcelSheetRow
            {
                Type = ExcelRowType.Summary,

                Values =
                [
                    "Lowest Net Salary",
                    data.LowestNetSalary
                ]
            },

            new ExcelSheetRow
            {
                Type = ExcelRowType.Summary,

                Values =
                [
                    "Employees Count",
                    data.EmployeesCount
                ]
            },

            new ExcelSheetRow
            {
                Type = ExcelRowType.Summary,

                Values =
                [
                    "Paid",
                    data.PaidEmployeesCount
                ]
            },

                };

            var sheet =
                new ExcelSheetData
                {
                    SheetName = "Payroll Statistics",

                    Headers =
                    [
                        "Metric",
                "Value"
                    ],

                    Rows = rows
                };

            return _excelExport.Export(sheet);
        }

        public async Task<byte[]> ExportSalaryComponentReportAsync(
        SalaryComponentReportFilterDto filter,
        CancellationToken cancellationToken = default)
        {
            var result =
                await _queries.GetSalaryComponentReportAsync(
                    filter,
                    cancellationToken);

            if (!result.Success)
            {
                throw new InvalidOperationException(
                    result.Message);
            }

            var data = result.Data!;

            var rows =
                data
                    .Select(x => new ExcelSheetRow
                    {
                        Type = ExcelRowType.Data,

                        Values =
                        [
                            x.ComponentName,
                    x.Type.ToString(),
                    x.EmployeesCount,
                    x.TotalAmount
                        ]
                    })
                    .ToList();

            var sheet =
                new ExcelSheetData
                {
                    SheetName = "Salary Components",

                    Headers =
                    [
                        "Component Name",
                "Type",
                "Employees Count",
                "Total Amount"
                    ],

                    Rows = rows
                };

            return _excelExport.Export(sheet);
        }

        public async Task<byte[]> ExportAttendanceDeductionReportAsync(
      AttendanceDeductionReportFilterDto filter,
      CancellationToken cancellationToken = default)
        {
            var result =
                await _queries.GetAttendanceDeductionReportAsync(
                    filter,
                    cancellationToken);

            if (!result.Success)
            {
                throw new InvalidOperationException(
                    result.Message);
            }

            var data = result.Data!;

            var rows =
                data
                    .Select(x => new ExcelSheetRow
                    {
                        Type = ExcelRowType.Data,

                        Values =
                        [
                            x.EmployeeName,
                    x.Department,
                    x.AbsentDays,
                    x.LateMinutes,
                    x.EarlyLeaveMinutes,
                    x.LostTimeMinutes,
                    x.DeductionAmount
                        ]
                    })
                    .ToList();

            // =========================================================
            // Total Row
            // =========================================================

            rows.Add(
                new ExcelSheetRow
                {
                    Type = ExcelRowType.Total,

                    Values =
                    [
                        "TOTAL",
                "",
                data.Sum(x => x.AbsentDays),
                data.Sum(x => x.LateMinutes),
                data.Sum(x => x.EarlyLeaveMinutes),
                data.Sum(x => x.LostTimeMinutes),
                data.Sum(x => x.DeductionAmount)
                    ]
                });

            // =========================================================
            // Excel Sheet
            // =========================================================

            var sheet =
                new ExcelSheetData
                {
                    SheetName = "Attendance Deductions",

                    Headers =
                    [
                        "Employee",
                "Department",
                "Absent Days",
                "Late Minutes",
                "Early Leave Minutes",
                "Lost Time Minutes",
                "Deduction Amount"
                    ],

                    Rows = rows,

                    BoldRowIndex = rows.Count - 1
                };

            return _excelExport.Export(sheet);
        }

        public async Task<byte[]> ExportAdjustmentReportAsync(
      AdjustmentReportFilterDto filter,
      CancellationToken cancellationToken = default)
        {
            var result =
                await _queries.GetAdjustmentReportAsync(
                    filter,
                    cancellationToken);

            if (!result.Success)
            {
                throw new InvalidOperationException(
                    result.Message);
            }

            var data = result.Data!;

            var rows =
                data
                    .Select(x => new ExcelSheetRow
                    {
                        Type = ExcelRowType.Data,

                        Values =
                        [
                            x.EmployeeName,
                    x.Department,
                    x.SalaryComponentName ?? "",
                    x.Type.ToString(),
                    x.Title,
                    x.Amount,
                    x.Notes ?? "",
                    x.IsApplied ? "Yes" : "No"
                        ]
                    })
                    .ToList();

            var sheet =
                new ExcelSheetData
                {
                    SheetName = "Adjustments",

                    Headers =
                    [
                        "Employee",
                "Department",
                "Salary Component",
                "Type",
                "Title",
                "Amount",
                "Notes",
                "Applied"
                    ],

                    Rows = rows
                };

            return _excelExport.Export(sheet);
        }

        public async Task<byte[]> ExportBankTransferReportAsync(
     BankTransferReportFilterDto filter,
     CancellationToken cancellationToken = default)
        {
            var result =
                await _queries.GetBankTransferReportAsync(
                    filter,
                    cancellationToken);

            if (!result.Success)
            {
                throw new InvalidOperationException(
                    result.Message);
            }

            var data = result.Data!;

            var rows =
                data.Employees
                    .Select(x => new ExcelSheetRow
                    {
                        Type = ExcelRowType.Data,

                        Values =
                        [
                            x.EmployeeName,
                    x.BankName ?? "",
                    x.AccountNumber ?? "",
                    x.IBAN ?? "",
                    x.Amount,
                    x.Status.ToString()
                        ]
                    })
                    .ToList();

            var sheet =
                new ExcelSheetData
                {
                    SheetName = "Bank Transfer",

                    Headers =
                    [
                        "Employee",
                "Bank Name",
                "Account Number",
                "IBAN",
                "Amount",
                "Status"
                    ],

                    Rows = rows
                };

            return _excelExport.Export(sheet);
        }
    }
}
