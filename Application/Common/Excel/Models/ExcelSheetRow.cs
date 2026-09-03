using MicroERP.Domin.Enums;

namespace MicroERP.Application.Common.Excel.Models
{
    public class ExcelSheetRow
    {
        public ExcelRowType Type { get; set; }

        public IReadOnlyList<object?> Values { get; set; } = [];
    }
}
