using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeEvaluations.DTOs.EmployeeEvaluationDto;
namespace MicroERP.Application.Features.EmployeeEvaluations.Queries;

public interface IEmployeeEvaluationQueries
{
    // =========================================================
    // Single Evaluation
    // =========================================================

    Task<Result<EmployeeEvaluationDto>> GetByIdAsync(int id,
        CancellationToken ct = default);


    // =========================================================
    // Paged Evaluations
    // =========================================================

    Task<Result<PagedResult<EmployeeEvaluationListDto>>> GetPagedAsync(
        EmployeeEvaluationFilterDto filter,
        CancellationToken ct = default);



    // =========================================================
    // Employee + Year + Month
    // =========================================================

    Task<Result<EmployeeEvaluationDto>>
        GetByEmployeeAndMonthAsync(
            int employeeId,
            int year,
            int month,
            CancellationToken ct = default);

    Task<Result<EmployeeMonthlyEvaluationExcelResultDto>>
    GetEmployeeMonthlyReportAsync(
        EmployeeMonthlyEvaluationExcelFilterDto filter,
        CancellationToken ct = default);


    // =========================================================
    // EmployeeEvaluationRank
    // =========================================================

    Task<Result<List<EmployeeEvaluationRankingDto>>>
    GetRankingAsync(
        EvaluationRankingFilterDto filter,
        CancellationToken ct = default);



    // =========================================================
    // Excel
    // =========================================================

    Task<Result<List<EmployeeEvaluationExcelDto>>>
     GetForExcelAsync(
         EmployeeEvaluationExcelFilterDto filter,
         CancellationToken ct = default);

    Task<Result<EmployeeEvaluationHistoryExcelResultDto>>
    GetEmployeeHistoryForExcelAsync(
        EmployeeEvaluationHistoryExcelFilterDto filter,
        CancellationToken ct = default);

    Task<Result<DepartmentEvaluationExcelResultDto>>
    GetDepartmentForExcelAsync(
        DepartmentEvaluationExcelFilterDto filter,
        CancellationToken ct = default);
}