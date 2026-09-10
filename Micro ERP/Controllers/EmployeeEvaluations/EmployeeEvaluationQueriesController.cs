using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeEvaluations;
using MicroERP.Application.Features.EmployeeEvaluations.DTOs.EmployeeEvaluationDto;
using MicroERP.Application.Features.EmployeeEvaluations.Queries;
using MicroERP.Application.Features.EmployeeEvaluations.Queries.MicroERP.Application.Features.EmployeeEvaluations.Excel;
using MicroERP.Domin.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.EmployeeEvaluations;

[ApiController]
[Route("api/employee-evaluations")]
public class EmployeeEvaluationQueriesController : ControllerBase
{
    private readonly IEmployeeEvaluationQueries _queries;
    private readonly IEmployeeEvaluationExcelExportService _excelExportService;

    public EmployeeEvaluationQueriesController(
        IEmployeeEvaluationQueries queries,
        IEmployeeEvaluationExcelExportService excelExportService)
    {
        _queries = queries;
        _excelExportService = excelExportService;
    }


    // =========================================================
    // Get By Id
    // =========================================================

    [HttpGet("{id:int}")]
    [Authorize(
        Policy = HRPermissions.EmployeeEvaluation.View)]
    public async Task<ActionResult<Result<EmployeeEvaluationDto>>> GetById(
        int id,
        CancellationToken ct)
    {
        var result =
            await _queries.GetByIdAsync(
                id,
                ct);

        return Ok(result);
    }


    // =========================================================
    // Get Statistics
    // =========================================================

    [HttpGet("statistics")]
    [Authorize(
        Policy = HRPermissions.EmployeeEvaluation.View)]
    public async Task<ActionResult<Result<EmployeeEvaluationStatisticsDto>>> GetStatistics(
        int year,
        int month,
        CancellationToken ct)
    {
        var result =
            await _queries.GetStatisticsAsync(
                year,
                month,
                ct);

        return Ok(result);
    }

    // =========================================================
    // Paged
    // =========================================================

    [HttpGet]
    [Authorize(Policy = HRPermissions.EmployeeEvaluation.View)]
    public async Task<ActionResult<Result<PagedResult<EmployeeEvaluationListDto>>>>
        GetPaged(
            [FromQuery] EmployeeEvaluationFilterDto filter,
            CancellationToken ct)
    {
        var result =
            await _queries.GetPagedAsync(
                filter,
                ct);

        return Ok(result);
    }


    // =========================================================
    // Employee + Year + Month
    // =========================================================

    [HttpGet("employee/{employeeId:int}/month")]
    [Authorize(
        Policy = HRPermissions.EmployeeEvaluation.View)]
    public async Task<
        ActionResult<Result<EmployeeEvaluationDto>>>
        GetByEmployeeAndMonth(
            int employeeId,
            [FromQuery] int year,
            [FromQuery] int month,
            CancellationToken ct)
    {
        var result =
            await _queries.GetByEmployeeAndMonthAsync(
                employeeId,
                year,
                month,
                ct);

        return Ok(result);
    }


    // =========================================================
    // Ranking
    // =========================================================

    [HttpGet("ranking")]
    [Authorize(
        Policy = HRPermissions.EmployeeEvaluation.View)]
    public async Task<ActionResult<Result<List<EmployeeEvaluationRankingDto>>>>
        GetRanking([FromQuery] EvaluationRankingFilterDto filter,
            CancellationToken ct)
    {
        var result =
            await _queries.GetRankingAsync(
                filter,
                ct);

        return Ok(result);
    }


    // =========================================================
    // Excel - All Employees
    // =========================================================

    [HttpGet("export/excel")]
    [Authorize(
    Policy = HRPermissions.EmployeeEvaluation.Export)]
    public async Task<IActionResult> ExportAllEmployees(
    [FromQuery] EmployeeEvaluationExcelFilterDto filter,
    CancellationToken ct)
    {
        var result =
            await _excelExportService.ExportAllEmployeesAsync(
                filter,
                ct);

        if (!result.Success)
        {
            return BadRequest(
                result.Message);
        }

        if (result.Data == null ||
            result.Data.Length == 0)
        {
            return BadRequest(
                "Failed to generate employee evaluations Excel file.");
        }

        var fileName =
            BuildEmployeeEvaluationFileName(
                filter);

        return File(
            result.Data,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }


    // =========================================================
    // Excel - Employee History
    // =========================================================

    [HttpGet("employee/export/excel")]
    [Authorize(
    Policy = HRPermissions.EmployeeEvaluation.Export)]
    public async Task<IActionResult> ExportEmployeeHistory(
    [FromQuery] EmployeeEvaluationHistoryExcelFilterDto filter,
    CancellationToken ct)
    {
        var result =
            await _excelExportService.ExportEmployeeHistoryAsync(
                filter,
                ct);

        if (!result.Success)
        {
            return BadRequest(
                result.Message);
        }

        if (result.Data == null ||
            result.Data.File == null ||
            result.Data.File.Length == 0)
        {
            return BadRequest(
                "Failed to generate employee evaluation history Excel file.");
        }

        var employeeName =
            SanitizeFileName(
                result.Data.EmployeeName);

        var year =
            filter.Year ??
            DateTime.Today.Year;

        var periodLabel =
            BuildEmployeeHistoryFilePeriodLabel(
                filter,
                year);

        var fileName =
            $"Employee-{employeeName}-{periodLabel}-Evaluations.xlsx";

        return File(
            result.Data.File,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    // =========================================================
    // Excel - Employee Monthly Evaluation
    // =========================================================

    [HttpGet("employee/monthly/export/excel")]
    [Authorize(
     Policy = HRPermissions.EmployeeEvaluation.Export)]
    public async Task<IActionResult> ExportEmployeeMonthly(
     [FromQuery] EmployeeMonthlyEvaluationExcelFilterDto filter,
     CancellationToken ct)
    {
        var result =
            await _excelExportService.ExportEmployeeMonthlyAsync(
                filter,
                ct);

        if (!result.Success)
        {
            return BadRequest(
                result.Message);
        }

        if (result.Data == null ||
            result.Data.File == null ||
            result.Data.File.Length == 0)
        {
            return BadRequest(
                "Failed to generate employee monthly evaluation Excel file.");
        }

        var employeeName =
            SanitizeFileName(
                result.Data.EmployeeName);

        var year =
            filter.Year ??
            DateTime.Today.Year;

        var monthName =
            GetMonthName(filter.Month);

        var fileName =
            $"Employee-{employeeName}-{monthName}-{year}-Evaluation.xlsx";

        return File(
            result.Data.File,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }


    // =========================================================
    // Excel - Department
    // =========================================================

    [HttpGet(
        "department/{departmentId:int}/export/excel")]
    [Authorize(
        Policy = HRPermissions.EmployeeEvaluation.Export)]
    public async Task<IActionResult> ExportDepartment(
        int departmentId,
        [FromQuery] DepartmentEvaluationExcelFilterDto filter,
        CancellationToken ct)
    {
        filter.DepartmentId =
            departmentId;

        var result =
            await _excelExportService.ExportDepartmentAsync(
                filter,
                ct);

        if (!result.Success)
        {
            return BadRequest(
                result.Message);
        }

        if (result.Data == null ||
            result.Data.Length == 0)
        {
            return BadRequest(
                "Failed to generate department evaluation Excel file.");
        }

        return File(
            result.Data,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Department-{departmentId}-Evaluations.xlsx");
    }

    private static string BuildEmployeeEvaluationFileName(
    EmployeeEvaluationExcelFilterDto filter)
    {
        var periodLabel =
            filter.Period switch
            {
                EvaluationReportPeriod.Monthly =>
                    filter.Month.HasValue
                        ? $"{GetMonthName(filter.Month.Value)}-{filter.Year}"
                        : filter.Year.ToString(),

                EvaluationReportPeriod.Quarterly =>
                    filter.Quarter.HasValue
                        ? $"Q{filter.Quarter.Value}-{filter.Year}"
                        : filter.Year.ToString(),

                EvaluationReportPeriod.HalfYearly =>
                    filter.HalfYear.HasValue
                        ? $"H{filter.HalfYear.Value}-{filter.Year}"
                        : filter.Year.ToString(),

                EvaluationReportPeriod.Yearly =>
                    filter.Year.ToString(),

                _ =>
                    filter.Year.ToString()
            };

        return
            $"Employee-Evaluations-{periodLabel}.xlsx";
    }

    private static string GetMonthName(int month)
    {
        return month switch
        {
            1 => "January",
            2 => "February",
            3 => "March",
            4 => "April",
            5 => "May",
            6 => "June",
            7 => "July",
            8 => "August",
            9 => "September",
            10 => "October",
            11 => "November",
            12 => "December",
            _ => month.ToString()
        };
    }
    private static string SanitizeFileName(string fileName)
    {
        foreach (var invalidChar in Path.GetInvalidFileNameChars())
        {
            fileName =
                fileName.Replace(
                    invalidChar.ToString(),
                    string.Empty);
        }

        return fileName.Trim();
    }
    private static string BuildEmployeeHistoryFilePeriodLabel(
      EmployeeEvaluationHistoryExcelFilterDto filter,
      int year)
    {
        return filter.Period switch
        {
            EvaluationReportPeriod.Monthly =>
                $"Monthly-{year}",

            EvaluationReportPeriod.Quarterly =>
                filter.Quarter.HasValue
                    ? $"Q{filter.Quarter.Value}-{year}"
                    : $"Quarterly-{year}",

            EvaluationReportPeriod.HalfYearly =>
                filter.HalfYear.HasValue
                    ? $"H{filter.HalfYear.Value}-{year}"
                    : $"HalfYearly-{year}",

            EvaluationReportPeriod.Yearly =>
                $"Yearly-{year}",

            _ =>
                year.ToString()
        };
    }
}