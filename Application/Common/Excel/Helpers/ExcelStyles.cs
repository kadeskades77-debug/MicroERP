using ClosedXML.Excel;

namespace MicroERP.Application.Common.Excel.Helpers;

public static class ExcelStyles
{
    public static void ApplyHeaderStyle(
        IXLRange range)
    {
        range.Style.Font.Bold = true;

        range.Style.Fill.BackgroundColor =
            XLColor.FromHtml("#1F4E78");

        range.Style.Font.FontColor =
            XLColor.White;

        range.Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;

        range.Style.Alignment.Vertical =
            XLAlignmentVerticalValues.Center;
    }


    public static void ApplyDetailLabelStyle(
     IXLCell cell)
    {
        cell.Style.Font.Bold = true;

        cell.Style.Fill.BackgroundColor =
            XLColor.FromHtml("#D9EAF7");

        cell.Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;

        cell.Style.Alignment.Vertical =
            XLAlignmentVerticalValues.Center;
    }


    public static void ApplyDetailValueStyle(
        IXLRange range)
    {
        range.Style.Fill.BackgroundColor =
            XLColor.FromHtml("#F7F9FB");

        range.Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;

        range.Style.Alignment.Vertical =
            XLAlignmentVerticalValues.Center;
    }


    public static void ApplyTotalStyle(
        IXLRange range)
    {
        range.Style.Font.Bold = true;

        range.Style.Fill.BackgroundColor =
            XLColor.FromHtml("#E2F0D9");

        range.Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;

        range.Style.Alignment.Vertical =
            XLAlignmentVerticalValues.Center;

        range.Style.Border.TopBorder =
            XLBorderStyleValues.Medium;
    }
}