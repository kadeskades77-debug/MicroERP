

using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeEvaluations.DTOs.EmployeeEvaluationDto;

namespace MicroERP.Application.Features.EmployeeEvaluations.Queries
{
    namespace MicroERP.Application.Features.EmployeeEvaluations.Excel
    {
        public interface IEmployeeEvaluationExcelExportService
        {
            Task<Result<byte[]>>
         ExportAllEmployeesAsync(
         EmployeeEvaluationExcelFilterDto filter,
         CancellationToken ct = default);


            Task<Result<EmployeeEvaluationExcelFileResult>>
        ExportEmployeeHistoryAsync(
         EmployeeEvaluationHistoryExcelFilterDto filter,
         CancellationToken ct = default);

            Task<Result<EmployeeEvaluationExcelFileResult>>
          ExportEmployeeMonthlyAsync(
        EmployeeMonthlyEvaluationExcelFilterDto filter,
        CancellationToken ct = default);


            Task<Result<byte[]>>
             ExportDepartmentAsync(
            DepartmentEvaluationExcelFilterDto filter,
            CancellationToken ct = default);
        }
    }
}
