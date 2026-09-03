
using DocumentFormat.OpenXml.Bibliography;
using MicroERP.Application.Authorization.Permissions;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Report;
using MicroERP.Application.Features.Employees.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.Attendance;

[Route("api/my-attendance")]
[ApiController]
[Authorize(Policy = EmployeePermissions.Profile.MyAttendance)]
public class MyAttendanceController : BaseApiController
{
    private readonly IMyAttendanceQueries _queries;
    private readonly IAttendanceExcelExportService _attendanceExcelExport;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEmployeeQueries _employee;

    public MyAttendanceController(
        IMyAttendanceQueries queries,
        IAttendanceExcelExportService attendanceExcelExport,
        ICurrentUserService currentUserService,
        IEmployeeQueries employee)
    {
        _queries = queries;
        _attendanceExcelExport = attendanceExcelExport;
        _currentUserService = currentUserService;
        _employee = employee;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyAttendance(
        [FromQuery] AttendanceFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetMyAttendanceAsync(
                filter,
                GetCurrentUserId(),
                cancellationToken);

        return Ok(result);
    }

    [HttpGet("{date}")]
    public async Task<IActionResult> GetMyAttendanceByDate(
        DateOnly date,
        CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetMyAttendanceByDateAsync(
                date,
                GetCurrentUserId(),
                cancellationToken);

        return Ok(result);
    }

    [HttpGet("monthly-summary")]
    public async Task<IActionResult> GetMyMonthlySummary(
        [FromQuery] int month,
        [FromQuery] int year,
        CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetMyMonthlySummaryAsync(
                month,
                year,
                GetCurrentUserId(),
                cancellationToken);

        return Ok(result);
    }

    [HttpGet("performance")]
    public async Task<IActionResult> GetMyPerformance(
        [FromQuery] int month,
        [FromQuery] int year,
        CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetMyPerformanceAsync(
                month,
                year,
                GetCurrentUserId(),
                cancellationToken);

        return Ok(result);
    }

    [HttpGet("performance/history")]
    public async Task<IActionResult> GetMyPerformanceHistory(
        CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetMyPerformanceHistoryAsync(
                GetCurrentUserId(),
                cancellationToken);

        return Ok(result);
    }

    [HttpGet("export")]
    public async Task<IActionResult> ExportMyAttendance(
        [FromQuery] AttendanceFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result =
            await _attendanceExcelExport.ExportMyAttendanceAsync(
                filter,
                cancellationToken);

        if (!result.Success)
        {
            return BadRequest(
                result.Message);
        }

        if (result.Data == null ||
            result.Data.Length == 0)
        {
            return BadRequest(
                "Failed to generate employee monthly evaluation Excel file.");
        }

        // =========================================================
        // File Name
        // =========================================================


        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return BadRequest(
                "Failed to generate employee monthly evaluation Excel file.");
        }
        var employee =
               await _employee.GetByUserIdAsync(userId);
        if (employee == null)
        {
            return BadRequest(
                "Failed to generate employee monthly evaluation Excel file.");
        }

        var employeeName = SanitizeFileName(employee.User.FullName);
        var month =
           DateTime.UtcNow.Month;
        var year =
           DateTime.UtcNow.Year;
        if (filter.DateFrom.HasValue)
        {
            month = filter.DateFrom.Value.Month;
            year = filter.DateFrom.Value.Year;
        }
        else
        {
            month= DateTime.UtcNow.Month;
            year= DateTime.UtcNow.Year;
        }
        var monthName =
            GetMonthName(month);

        var fileName = $"Performance_{employeeName}_From:{monthName}-{year}";


        fileName += ".xlsx";
        return File(
            result.Data,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);

    }

    [HttpGet("performance/export")]
    public async Task<IActionResult> ExportMyPerformance(
    [FromQuery] int year,
    [FromQuery] int month,
    CancellationToken cancellationToken)
    {
        var result =
            await _attendanceExcelExport.ExportMyPerformanceAsync(
                year,
                month,
                cancellationToken);
        if (!result.Success)
        {
            return BadRequest(
                result.Message);
        }

        if (result.Data == null ||
            result.Data.Length == 0)
        {
            return BadRequest(
                "Failed to generate employee monthly evaluation Excel file.");
        }

        // =========================================================
        // File Name
        // =========================================================


        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return BadRequest(
                "Failed to generate employee monthly evaluation Excel file.");
        }
        var employee =
               await _employee.GetByUserIdAsync(userId);
        if (employee == null)
        {
            return BadRequest(
                "Failed to generate employee monthly evaluation Excel file.");
        }

        var employeeName = SanitizeFileName(employee.User.FullName);


        var monthName =
            GetMonthName(month);

        var fileName = $"Performance_{employeeName}_From:{monthName}-{year}";


        fileName += ".xlsx";
        return File(
            result.Data,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    private static string SanitizeFileName(string value)
    {
        foreach (var character in Path.GetInvalidFileNameChars())
        {
            value = value.Replace(
                character,
                '_');
        }

        return value.Trim();
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
}