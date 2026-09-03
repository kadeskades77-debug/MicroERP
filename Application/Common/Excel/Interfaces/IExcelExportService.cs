using MicroERP.Application.Common.Excel.Models;

namespace MicroERP.Application.Common.Excel.Interfaces;

public interface IExcelExportService
{
    byte[] Export(
        ExcelSheetData sheet);

    byte[] Export(
        IEnumerable<ExcelSheetData> sheets);
}