using MicroERP.Application.Common.Excel.Interfaces;
using MicroERP.Application.Common.Excel.Models;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Overtime;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Services
{
    public class OvertimeExcelExportService : IOvertimeExcelExportService
    {
        private readonly IOvertimeReportQueries _queries;
        private readonly IExcelExportService _excelExporter;

        public OvertimeExcelExportService(IOvertimeReportQueries queries, IExcelExportService excelExporter)
        {
            _queries = queries;
            _excelExporter = excelExporter;
        }

        public async Task<byte[]> ExportOvertimeAsync(
        OvertimeReportFilterDto filter,
        CancellationToken cancellationToken = default)
        {
            // =========================================================
            // Get Report
            // =========================================================

            var result =
                await _queries.GetOvertimeReportAsync(
                    filter,
                    cancellationToken);

            if (!result.Success || result.Data == null)
            {
                throw new InvalidOperationException(
                    result.Message ??
                    "Failed to get overtime report.");
            }

            var data = result.Data;


            // =========================================================
            // Headers
            // =========================================================

            var headers =
                new List<string>
                {
            "#",
            "Employee",
            "Start Date/Time",
            "End Date/Time",
            "Total Minutes",
            "Total Hours",
            "Hourly Rate",
            "Multiplier",
            "Amount",
            "Type",
            "Status",
            "Source",
            "Reason",
            "Paid"
                };


            // =========================================================
            // Rows
            // =========================================================

            var rows =
                new List<ExcelSheetRow>();

            var recordNumber = 1;


            // =========================================================
            // Group By Department
            // =========================================================

            var departments =
                data.Overtimes
                    .GroupBy(x =>
                        string.IsNullOrWhiteSpace(x.Department)
                            ? "Without Department"
                            : x.Department)
                    .OrderBy(x => x.Key)
                    .ToList();


            foreach (var departmentGroup in departments)
            {
                var departmentName =
                    departmentGroup.Key;


                // =====================================================
                // Department Header
                // =====================================================

                rows.Add(
                    new ExcelSheetRow
                    {
                        Type =
                            ExcelRowType.Department,

                        Values =
                        [
                            $"Department: {departmentName}"
                        ]
                    });


                // =====================================================
                // Department Statistics
                // =====================================================

                var employeesCount =
                    departmentGroup
                        .Select(x => x.EmployeeId)
                        .Distinct()
                        .Count();

                var paidAmount =
                    departmentGroup
                        .Where(x => x.IsPaid)
                        .Sum(x => x.Amount);

                var unpaidAmount =
                    departmentGroup
                        .Where(x => !x.IsPaid)
                        .Sum(x => x.Amount);


                // Employees Count

                rows.Add(
                    new ExcelSheetRow
                    {
                        Type =
                            ExcelRowType.Summary,

                        Values =
                        [
                            "Employees Count",
                    employeesCount
                        ]
                    });


                // Paid Amount

                rows.Add(
                    new ExcelSheetRow
                    {
                        Type =
                            ExcelRowType.Summary,

                        Values =
                        [
                            "Paid Amount",
                    paidAmount
                        ]
                    });


                // Unpaid Amount

                rows.Add(
                    new ExcelSheetRow
                    {
                        Type =
                            ExcelRowType.Summary,

                        Values =
                        [
                            "Unpaid Amount",
                    unpaidAmount
                        ]
                    });


                // =====================================================
                // Department Details
                // =====================================================

                foreach (
                    var overtime in departmentGroup
                        .OrderBy(x => x.EmployeeName)
                        .ThenByDescending(x => x.StartDateTime))
                {
                    rows.Add(
                        new ExcelSheetRow
                        {
                            Type =
                                ExcelRowType.Data,

                            Values =
                            [
                                recordNumber++,

                        overtime.EmployeeName,

                        overtime.StartDateTime,

                        overtime.EndDateTime,

                        overtime.TotalMinutes,

                        overtime.TotalHours,

                        overtime.HourlyRate,

                        overtime.Multiplier,

                        overtime.Amount,

                        overtime.Type.ToString(),

                        overtime.Status.ToString(),

                        overtime.Source.ToString(),

                        overtime.Reason ??
                            string.Empty,

                        overtime.IsPaid
                            ? "Yes"
                            : "No"
                            ]
                        });
                }
            }


            // =========================================================
            // Grand Total
            // =========================================================

            rows.Add(
                new ExcelSheetRow
                {
                    Type =
                        ExcelRowType.Total,

                    Values =
                    [
                        "TOTAL",

                null,

                null,

                null,

                data.TotalMinutes,

                data.TotalHours,

                null,

                null,

                data.TotalAmount,

                null,

                null,

                null,

                null,

                null
                    ]
                });


            // =========================================================
            // Excel Sheet
            // =========================================================

            var sheet =
                new ExcelSheetData
                {
                    SheetName =
                        "Overtime",

                    Headers =
                        headers,

                    Rows =
                        rows
                };


            return _excelExporter.Export(sheet);
        }

        public async Task<byte[]> ExportEmployeeOvertimeAsync(
        OvertimeReportFilterDto filter,
        CancellationToken cancellationToken = default)
        {
            // =========================================================
            // Validate Employee
            // =========================================================

            if (!filter.EmployeeId.HasValue ||
                filter.EmployeeId.Value <= 0)
            {
                throw new ArgumentException(
                    "EmployeeId is required.");
            }


            // =========================================================
            // Get Report
            // =========================================================

            var result =
                await _queries.GetEmployeeOvertimeReportAsync(
                    filter,
                    cancellationToken);

            if (!result.Success || result.Data == null)
            {
                throw new Exception(
                    result.Message ??
                    "Failed to get employee overtime report.");
            }

            var data = result.Data;


            // =========================================================
            // Rows
            // =========================================================

            var rows =
                new List<ExcelSheetRow>();


            // =========================================================
            // Details
            // =========================================================

            foreach (var overtime in data.Overtimes)
            {
                rows.Add(
                    new ExcelSheetRow
                    {
                        Type =
                            ExcelRowType.Data,

                        Values =
                        [
                            overtime.StartDateTime,

                    overtime.EndDateTime,

                    overtime.TotalMinutes,

                    overtime.TotalHours,

                    overtime.HourlyRate,

                    overtime.Multiplier,

                    overtime.Amount,

                    overtime.Type.ToString(),

                    overtime.Status.ToString(),

                    overtime.Source.ToString(),

                    overtime.Reason ??
                        string.Empty,

                    overtime.IsPaid
                        ? "Yes"
                        : "No"
                        ]
                    });
            }


            // =========================================================
            // Total
            // =========================================================

            rows.Add(
                new ExcelSheetRow
                {
                    Type =
                        ExcelRowType.Total,

                    Values =
                    [
                        "TOTAL",

                null,

                data.TotalMinutes,

                data.TotalHours,

                null,

                null,

                data.TotalAmount,

                null,

                null,

                null,

                null,

                null
                    ]
                });


            // =========================================================
            // Excel Sheet
            // =========================================================

            var sheet =
                new ExcelSheetData
                {
                    SheetName =
                        "Employee Overtime",

                    // -------------------------------------------------
                    // Employee Information
                    // -------------------------------------------------

                    TopRows =
                    [
                        new object?[]
                {
                    "Employee",
                    data.EmployeeName
                },

                new object?[]
                {
                    "Department",
                    data.Department ??
                        "Without Department"
                }
                    ],

                    // -------------------------------------------------
                    // Section
                    // -------------------------------------------------

                    SectionTitle =
                        "Overtime Details",

                    // -------------------------------------------------
                    // Headers
                    // -------------------------------------------------

                    Headers =
                    [
                        "Start Date/Time",
                "End Date/Time",
                "Total Minutes",
                "Total Hours",
                "Hourly Rate",
                "Multiplier",
                "Amount",
                "Type",
                "Status",
                "Source",
                "Reason",
                "Paid"
                    ],

                    // -------------------------------------------------
                    // Rows
                    // -------------------------------------------------

                    Rows =
                        rows
                };


            return _excelExporter.Export(sheet);
        }

        public async Task<byte[]> ExportEmployeeOvertimeTypeAsync(
         OvertimeReportFilterDto filter,
         CancellationToken cancellationToken = default)
        {
            // =========================================================
            // Validate Employee
            // =========================================================

            if (!filter.EmployeeId.HasValue ||
                filter.EmployeeId.Value <= 0)
            {
                throw new ArgumentException(
                    "EmployeeId is required.");
            }


            // =========================================================
            // Get Report
            // =========================================================

            var result =
                await _queries.GetEmployeeOvertimeTypeReportAsync(
                    filter,
                    cancellationToken);

            if (!result.Success || result.Data == null)
            {
                throw new Exception(
                    result.Message ??
                    "Failed to get employee overtime type report.");
            }

            var data = result.Data;


            // =========================================================
            // Rows
            // =========================================================

            var rows =
                new List<ExcelSheetRow>();


            // =========================================================
            // Details
            // =========================================================

            foreach (var overtime in data.Overtimes)
            {
                rows.Add(
                    new ExcelSheetRow
                    {
                        Type =
                            ExcelRowType.Data,

                        Values =
                        [
                            overtime.StartDateTime,

                    overtime.EndDateTime,

                    overtime.TotalMinutes,

                    overtime.TotalHours,

                    overtime.HourlyRate,

                    overtime.Multiplier,

                    overtime.Amount,

                    overtime.Status.ToString(),

                    overtime.Source.ToString(),

                    overtime.Reason ??
                        string.Empty,

                    overtime.IsPaid
                        ? "Yes"
                        : "No"
                        ]
                    });
            }


            // =========================================================
            // Total
            // =========================================================

            rows.Add(
                new ExcelSheetRow
                {
                    Type =
                        ExcelRowType.Total,

                    Values =
                    [
                        "TOTAL",

                null,

                data.TotalMinutes,

                data.TotalHours,

                null,

                null,

                data.TotalAmount,

                null,

                null,

                null,

                null
                    ]
                });


            // =========================================================
            // Excel Sheet
            // =========================================================

            var sheet =
                new ExcelSheetData
                {
                    SheetName =
                        "Employee Overtime",

                    // -------------------------------------------------
                    // Employee Information
                    // -------------------------------------------------

                    TopRows =
                    [
                        new object?[]
                {
                    "Employee",
                    data.EmployeeName
                },

                new object?[]
                {
                    "Department",
                    data.Department ??
                        "Without Department"
                },

                new object?[]
                {
                    "Type",
                    data.Type.ToString()
                }
                    ],

                    SectionTitle =
                        "Overtime Details",

                    // -------------------------------------------------
                    // Headers
                    // -------------------------------------------------

                    Headers =
                    [
                        "Start Date/Time",
                "End Date/Time",
                "Total Minutes",
                "Total Hours",
                "Hourly Rate",
                "Multiplier",
                "Amount",
                "Status",
                "Source",
                "Reason",
                "Paid"
                    ],

                    Rows =
                        rows
                };


            return _excelExporter.Export(sheet);
        }

        public async Task<byte[]> ExportEmployeeOvertimeSourceAsync(
        OvertimeReportFilterDto filter,
        CancellationToken cancellationToken = default)
        {
            // =========================================================
            // Validate Employee
            // =========================================================

            if (!filter.EmployeeId.HasValue ||
                filter.EmployeeId.Value <= 0)
            {
                throw new ArgumentException(
                    "EmployeeId is required.");
            }


            // =========================================================
            // Get Report
            // =========================================================

            var result =
                await _queries.GetEmployeeOvertimeSourceReportAsync(
                    filter,
                    cancellationToken);

            if (!result.Success || result.Data == null)
            {
                throw new Exception(
                    result.Message ??
                    "Failed to get employee overtime source report.");
            }

            var data = result.Data;


            // =========================================================
            // Rows
            // =========================================================

            var rows =
                new List<ExcelSheetRow>();


            // =========================================================
            // Details
            // =========================================================

            foreach (var overtime in data.Overtimes)
            {
                rows.Add(
                    new ExcelSheetRow
                    {
                        Type =
                            ExcelRowType.Data,

                        Values =
                        [
                            overtime.StartDateTime,

                    overtime.EndDateTime,

                    overtime.TotalMinutes,

                    overtime.TotalHours,

                    overtime.HourlyRate,

                    overtime.Multiplier,

                    overtime.Amount,

                    overtime.Type.ToString(),

                    overtime.Status.ToString(),

                    overtime.Reason ??
                        string.Empty,

                    overtime.IsPaid
                        ? "Yes"
                        : "No"
                        ]
                    });
            }


            // =========================================================
            // Total
            // =========================================================

            rows.Add(
                new ExcelSheetRow
                {
                    Type =
                        ExcelRowType.Total,

                    Values =
                    [
                        "TOTAL",

                null,

                data.TotalMinutes,

                data.TotalHours,

                null,

                null,

                data.TotalAmount,

                null,

                null,

                null,

                null
                    ]
                });


            // =========================================================
            // Excel Sheet
            // =========================================================

            var sheet =
                new ExcelSheetData
                {
                    SheetName =
                        "Employee Overtime",

                    // -------------------------------------------------
                    // Employee Information
                    // -------------------------------------------------

                    TopRows =
                    [
                        new object?[]
                {
                    "Employee",
                    data.EmployeeName
                },

                new object?[]
                {
                    "Department",
                    data.Department ??
                        "Without Department"
                },

                new object?[]
                {
                    "Source",
                    data.Source.ToString()
                }
                    ],

                    SectionTitle =
                        "Overtime Details",

                    // -------------------------------------------------
                    // Headers
                    // -------------------------------------------------

                    Headers =
                    [
                        "Start Date/Time",
                "End Date/Time",
                "Total Minutes",
                "Total Hours",
                "Hourly Rate",
                "Multiplier",
                "Amount",
                "Type",
                "Status",
                "Reason",
                "Paid"
                    ],

                    Rows =
                        rows
                };


            return _excelExporter.Export(sheet);
        }
    }
}
