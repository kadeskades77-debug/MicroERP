using MicroERP.Application.Features.Payrolls.Interfaces;
using MicroERP.Application.Features.Payrolls.Reports.DTOs;
using MicroERP.Application.Features.Payrolls.Reports.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MicroERP.Application.Features.Payrolls.Reports.Queries;

public class AdjustmentPdfService
    : IAdjustmentPdfService
{
    private readonly IPayrollReportQueries _queries;
    private readonly IPayrollQueries _payrollQueries;

    public AdjustmentPdfService(
        IPayrollReportQueries queries, IPayrollQueries payrollQueries)
    {
        _queries = queries;
        _payrollQueries = payrollQueries;
    }

    public async Task<byte[]> GenerateAsync(
        AdjustmentReportFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var result =
            await _queries.GetAdjustmentReportAsync(
                filter,
                cancellationToken);

        if (!result.Success || result.Data == null)
        {
            throw new InvalidOperationException(
                result.Message ??
                "Unable to generate adjustment report.");
        }

        var data = result.Data;

        if (data.Count == 0)
        {
            throw new InvalidOperationException(
                "No adjustment records found.");
        }

        var periodResult =
    await _payrollQueries.GetPeriodByIdAsync(
        filter.PayrollPeriodId,
        cancellationToken);

        if (!periodResult.Success ||
            periodResult.Data == null)
        {
            throw new InvalidOperationException(
                periodResult.Message ??
                "Payroll period not found.");
        }

        var period = periodResult.Data;

        return Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());

                page.Margin(30);

                page.DefaultTextStyle(
                    x => x.FontSize(9));

                page.Header()
           .Element(x =>
               ComposeHeader(
                   x,
                   filter,
                   data.First()));

                page.Content()
                    .Element(x =>
                        ComposeContent(
                            x,
                            data,
                            filter));

                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Page ");
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
            });
        })
        .GeneratePdf();
    }

    // =========================================================
    // HEADER
    // =========================================================

    private static void ComposeHeader(
     IContainer container,
     AdjustmentReportFilterDto filter,
     AdjustmentReportDto adjustment)
    {
        container
            .Background(Colors.Grey.Darken2)
            .Padding(12)
            .Column(column =>
            {
                column.Item()
                    .AlignCenter()
                    .Text("PAYROLL ADJUSTMENT REPORT")
                    .Bold()
                    .FontSize(17)
                    .FontColor(Colors.White);

                column.Item()
                    .PaddingTop(4)
                    .AlignCenter()
                    .Text(
                        $"Period: {adjustment.Year}-{adjustment.Month:00}")
                    .Bold()
                    .FontSize(10)
                    .FontColor(Colors.White);
            });
    }

    // =========================================================
    // CONTENT
    // =========================================================

    private static void ComposeContent(
        IContainer container,
        List<AdjustmentReportDto> data,
        AdjustmentReportFilterDto filter)
    {
        var isGroupedReport =
            !filter.EmployeeId.HasValue;

        container.Column(column =>
        {
            column.Spacing(12);

            if (isGroupedReport)
            {
                ComposeGroupedByEmployee(
                    column,
                    data);
            }
            else
            {
                var employee =
                    data.FirstOrDefault();

                if (employee != null)
                {
                    column.Item()
                        .ShowEntire()
                        .Element(x =>
                            ComposeEmployeeSection(
                                x,
                                employee.EmployeeName,
                                employee.Department,
                                data));
                }
            }

            // Summary
            column.Item()
                .ShowEntire()
                .Element(x =>
                    ComposeTotals(
                        x,
                        data));
        });
    }

    // =========================================================
    // GROUP BY EMPLOYEE
    // =========================================================

    private static void ComposeGroupedByEmployee(
        ColumnDescriptor column,
        List<AdjustmentReportDto> data)
    {
        var groups =
            data
                .GroupBy(x => new
                {
                    x.EmployeeId,
                    x.EmployeeName,
                    x.Department
                })
                .OrderBy(x => x.Key.EmployeeName);

        foreach (var group in groups)
        {
            var employeeData =
                group
                    .OrderBy(x => x.Id)
                    .ToList();

            column.Item()
                .ShowEntire()
                .Element(x =>
                    ComposeEmployeeSection(
                        x,
                        group.Key.EmployeeName,
                        group.Key.Department,
                        employeeData));
        }
    }

    // =========================================================
    // EMPLOYEE SECTION
    // =========================================================

    private static void ComposeEmployeeSection(
        IContainer container,
        string employeeName,
        string? department,
        List<AdjustmentReportDto> data)
    {
        container.Column(column =>
        {
            // Employee information box
            column.Item()
                .Background(Colors.Grey.Lighten3)
                .Border(1)
                .Padding(8)
                .Row(row =>
                {
                    row.RelativeItem()
                        .Text(text =>
                        {
                            text.Span("Employee: ")
                                .Bold();

                            text.Span(employeeName);
                        });

                    row.RelativeItem()
                        .Text(text =>
                        {
                            text.Span("Department: ")
                                .Bold();

                            text.Span(
                                string.IsNullOrWhiteSpace(department)
                                    ? "-"
                                    : department);
                        });
                });

            // Table
            column.Item()
                .PaddingTop(6)
                .Element(x =>
                    ComposeAdjustmentTable(
                        x,
                        data));
        });
    }

    // =========================================================
    // ADJUSTMENT TABLE
    // =========================================================

    private static void ComposeAdjustmentTable(
        IContainer container,
        List<AdjustmentReportDto> data)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(40);       // ID
                columns.RelativeColumn(1.4f);      // Component
                columns.RelativeColumn(1);        // Type
                columns.RelativeColumn(1.7f);      // Title
                columns.RelativeColumn(1);        // Amount
                columns.ConstantColumn(65);       // Applied
                columns.RelativeColumn(1.7f);      // Notes
            });

            // Header
            table.Header(header =>
            {
                AddHeaderCell(
                    header.Cell(),
                    "ID");

                AddHeaderCell(
                    header.Cell(),
                    "Component");

                AddHeaderCell(
                    header.Cell(),
                    "Type");

                AddHeaderCell(
                    header.Cell(),
                    "Title");

                AddHeaderCell(
                    header.Cell(),
                    "Amount");

                AddHeaderCell(
                    header.Cell(),
                    "Applied");

                AddHeaderCell(
                    header.Cell(),
                    "Notes");
            });

            // Rows
            foreach (var adjustment in
                data.OrderBy(x => x.Id))
            {
                AddBodyCell(
                    table,
                    adjustment.Id.ToString());

                AddBodyCell(
                    table,
                    adjustment.SalaryComponentName ?? "-");

                AddBodyCell(
                    table,
                    adjustment.Type.ToString());

                AddBodyCell(
                    table,
                    adjustment.Title);

                AddAmountCell(
                    table,
                    adjustment.Amount);

                AddStatusCell(
                    table,
                    adjustment.IsApplied
                        ? "Yes"
                        : "No");

                AddBodyCell(
                    table,
                    adjustment.Notes ?? "-");
            }
        });
    }

    // =========================================================
    // TOTALS
    // =========================================================

    private static void ComposeTotals(
        IContainer container,
        List<AdjustmentReportDto> data)
    {
        var totalAmount =
            data.Sum(x => x.Amount);

        var employeesCount =
            data
                .Select(x => x.EmployeeId)
                .Distinct()
                .Count();

        var appliedCount =
            data.Count(x => x.IsApplied);

        var notAppliedCount =
            data.Count(x => !x.IsApplied);

        container
            .PaddingTop(8)
            .Column(column =>
            {
                column.Item()
                    .Text("ADJUSTMENT SUMMARY")
                    .Bold()
                    .FontSize(12);

                column.Item()
                    .PaddingTop(6)
                    .Row(row =>
                    {
                        row.Spacing(6);

                        row.RelativeItem()
                            .Element(x =>
                                AddTotalCell(
                                    x,
                                    "RECORDS",
                                    data.Count.ToString("N0")));

                        row.RelativeItem()
                            .Element(x =>
                                AddTotalCell(
                                    x,
                                    "EMPLOYEES",
                                    employeesCount.ToString("N0")));

                        row.RelativeItem()
                            .Element(x =>
                                AddTotalCell(
                                    x,
                                    "TOTAL AMOUNT",
                                    totalAmount.ToString("N2")));

                        row.RelativeItem()
                            .Element(x =>
                                AddTotalCell(
                                    x,
                                    "APPLIED",
                                    appliedCount.ToString("N0")));

                        row.RelativeItem()
                            .Element(x =>
                                AddTotalCell(
                                    x,
                                    "NOT APPLIED",
                                    notAppliedCount.ToString("N0")));
                    });
            });
    }

    // =========================================================
    // HEADER CELL
    // =========================================================

    private static IContainer AddHeaderCell(
        IContainer container,
        string text)
    {
        container
            .Background(Colors.Grey.Darken2)
            .Border(1)
            .Padding(5)
            .AlignMiddle()
            .AlignCenter()
            .Text(text)
            .Bold()
            .FontColor(Colors.White)
            .FontSize(8);

        return container;
    }

    // =========================================================
    // BODY CELL
    // =========================================================

    private static void AddBodyCell(
     TableDescriptor table,
     string text)
    {
        table.Cell()
            .Border(1)
            .Padding(5)
            .AlignMiddle()
            .AlignCenter()
            .Text(text)
            .FontSize(8);
    }

    // =========================================================
    // AMOUNT CELL
    // =========================================================

    private static void AddAmountCell(
      TableDescriptor table,
      decimal amount)
    {
        table.Cell()
            .Border(1)
            .Padding(5)
            .AlignMiddle()
            .AlignCenter()
            .Text(amount.ToString("N2"))
            .FontSize(8);
    }

    // =========================================================
    // STATUS CELL
    // =========================================================

    private static void AddStatusCell(
        TableDescriptor table,
        string text)
    {
        table.Cell()
            .Border(1)
            .Padding(5)
            .AlignMiddle()
            .AlignCenter()
            .Text(text)
            .Bold()
            .FontSize(8);
    }

    // =========================================================
    // TOTAL CELL
    // =========================================================

    private static IContainer AddTotalCell(
        IContainer container,
        string label,
        string value)
    {
        container
            .Background(Colors.Grey.Lighten3)
            .Border(1)
            .Padding(9)
            .Column(column =>
            {
                column.Item()
                    .AlignCenter()
                    .Text(label)
                    .Bold()
                    .FontSize(8);

                column.Item()
                    .PaddingTop(4)
                    .AlignCenter()
                    .Text(value)
                    .Bold()
                    .FontSize(11);
            });

        return container;
    }
}