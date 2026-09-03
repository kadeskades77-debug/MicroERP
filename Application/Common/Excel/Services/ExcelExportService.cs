using ClosedXML.Excel;
using MicroERP.Application.Common.Excel.Helpers;
using MicroERP.Application.Common.Excel.Interfaces;
using MicroERP.Application.Common.Excel.Models;
using MicroERP.Domin.Enums;

namespace MicroERP.Application.Common.Excel.Services;

public class ExcelExportService : IExcelExportService
{
    public byte[] Export(
        ExcelSheetData sheet)
    {
        return Export(new[] { sheet });
    }



    public byte[] Export(
        IEnumerable<ExcelSheetData> sheets)
    {
        using var workbook = new XLWorkbook();



        foreach (var sheet in sheets)
        {
            CreateWorksheet(
                workbook,
                sheet);
        }



        using var stream = new MemoryStream();

        workbook.SaveAs(stream);

        return stream.ToArray();
    }



    private static void CreateWorksheet(
     XLWorkbook workbook,
     ExcelSheetData sheetData)
    {
        var sheet =
            workbook.Worksheets.Add(
                sheetData.SheetName);

        var currentRow = 1;

        // =========================================================
        // Top Details
        // =========================================================

        foreach (var row in sheetData.TopRows)
        {
            for (int column = 0;
                 column < row.Count;
                 column++)
            {
                var cell =
                    sheet.Cell(
                        currentRow,
                        column + 1);

                SetCellValue(
                    cell,
                    row[column]);
            }

            if (row.Count > 0)
            {
                ExcelStyles.ApplyDetailLabelStyle(
                    sheet.Cell(
                        currentRow,
                        1));
            }

            if (row.Count > 1)
            {
                ExcelStyles.ApplyDetailValueStyle(
                    sheet.Range(
                        currentRow,
                        2,
                        currentRow,
                        row.Count));
            }

            currentRow++;
        }


        // =========================================================
        // Section Title
        // =========================================================

        if (!string.IsNullOrWhiteSpace(
            sheetData.SectionTitle))
        {
            sheet.Cell(
                currentRow,
                1)
                .Value =
                sheetData.SectionTitle;

            sheet.Range(
                currentRow,
                1,
                currentRow,
                sheetData.Headers.Count)
                .Merge();

            ExcelStyles.ApplyHeaderStyle(
                sheet.Range(
                    currentRow,
                    1,
                    currentRow,
                    sheetData.Headers.Count));

            currentRow++;
        }


        // =========================================================
        // Headers
        // =========================================================

        for (int column = 0;
             column < sheetData.Headers.Count;
             column++)
        {
            sheet.Cell(
                currentRow,
                column + 1)
                .Value =
                sheetData.Headers[column];
        }

        ExcelStyles.ApplyHeaderStyle(
            sheet.Range(
                currentRow,
                1,
                currentRow,
                sheetData.Headers.Count));

        var headerRow =
            currentRow;

        currentRow++;


        // =========================================================
        // Rows
        // =========================================================

        foreach (var rowData in sheetData.Rows)
        {
            var values =
                rowData.Values;

            switch (rowData.Type)
            {
                // -------------------------------------------------
                // Department
                // -------------------------------------------------

                case ExcelRowType.Department:

                    WriteRow(
                        sheet,
                        currentRow,
                        values);

                    sheet.Range(
                        currentRow,
                        1,
                        currentRow,
                        sheetData.Headers.Count)
                        .Merge();

                    ExcelStyles.ApplyHeaderStyle(
                        sheet.Range(
                            currentRow,
                            1,
                            currentRow,
                            sheetData.Headers.Count));

                    break;


                // -------------------------------------------------
                // Summary
                // -------------------------------------------------

                case ExcelRowType.Summary:

                    WriteRow(
                        sheet,
                        currentRow,
                        values);

                    ExcelStyles.ApplyDetailValueStyle(
                        sheet.Range(
                            currentRow,
                            1,
                            currentRow,
                            sheetData.Headers.Count));

                    break;


                // -------------------------------------------------
                // Total
                // -------------------------------------------------

                case ExcelRowType.Total:

                    WriteRow(
                        sheet,
                        currentRow,
                        values);

                    ExcelStyles.ApplyTotalStyle(
                        sheet.Range(
                            currentRow,
                            1,
                            currentRow,
                            sheetData.Headers.Count));

                    break;


                // -------------------------------------------------
                // Data
                // -------------------------------------------------

                case ExcelRowType.Data:

                    WriteRow(
                        sheet,
                        currentRow,
                        values);

                    break;
            }

            currentRow++;
        }


        // =========================================================
        // General Formatting
        // =========================================================

        var usedRange =
            sheet.RangeUsed();

        if (usedRange != null)
        {
            usedRange.Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            usedRange.Style.Alignment.Vertical =
                XLAlignmentVerticalValues.Center;
        }


        sheet.SheetView
            .FreezeRows(headerRow);


        sheet.RangeUsed()
            ?.SetAutoFilter();


        sheet.Columns()
            .AdjustToContents();
    }

    private static void WriteRow(
    IXLWorksheet sheet,
    int rowNumber,
    IReadOnlyList<object?> values)
    {
        for (int column = 0;
             column < values.Count;
             column++)
        {
            var cell =
                sheet.Cell(
                    rowNumber,
                    column + 1);

            SetCellValue(
                cell,
                values[column]);

            cell.Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            cell.Style.Alignment.Vertical =
                XLAlignmentVerticalValues.Center;
        }
    }

    private static void SetCellValue(
    IXLCell cell,
    object? value)
    {
        switch (value)
        {
            case null:
                cell.Value = string.Empty;
                break;

            case string s:
                cell.Value = s;
                break;

            case int i:
                cell.Value = i;
                break;

            case long l:
                cell.Value = l;
                break;

            case decimal d:
                cell.Value = d;
                break;

            case double d:
                cell.Value = d;
                break;

            case float f:
                cell.Value = f;
                break;

            case bool b:
                cell.Value = b;
                break;

            case DateTime dt:
                cell.Value = dt;
                cell.Style.DateFormat.Format =
                    ExcelConstants.DateTimeFormat;
                break;

            case DateOnly date:
                cell.Value = date.ToDateTime(TimeOnly.MinValue);
                cell.Style.DateFormat.Format =
                    ExcelConstants.DateFormat;
                break;

            case TimeOnly time:
                cell.Value = DateTime.Today.Add(time.ToTimeSpan());
                cell.Style.DateFormat.Format =
                    ExcelConstants.TimeFormat;
                break;

            default:
                cell.Value = value.ToString();
                break;
        }
    }

}