using MicroERP.Domin.Enums;

namespace MicroERP.Application.Common.Excel.Models;

public class ExcelSheetData
{
    public string SheetName { get; set; } = "Sheet1";

    public int? BoldRowIndex { get; set; }

    public IReadOnlyList<IReadOnlyList<object?>> TopRows { get; set; }
        = Array.Empty<IReadOnlyList<object?>>();

    public string? SectionTitle { get; set; }

    public IReadOnlyList<string> Headers { get; set; }
        = Array.Empty<string>();

    public List<ExcelSheetRow> Rows { get; set; } = [];
}