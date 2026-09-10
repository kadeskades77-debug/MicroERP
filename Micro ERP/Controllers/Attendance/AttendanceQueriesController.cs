using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Departments.Interfaces;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Report;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using MicroERP.Application.Features.Employees.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.Attendance;

[Route("api/attendance/queries")]
[ApiController]
public class AttendanceQueriesController : ControllerBase
{
    private readonly IAttendanceQueries _queries;
    private readonly IAttendanceExcelExportService _excelExportService;
    private readonly IDepartmentQueries _department;
    private readonly IEmployeeQueries _employee;

    public AttendanceQueriesController(
        IAttendanceQueries queries, IAttendanceExcelExportService excelExportService, IDepartmentQueries department, IEmployeeQueries employee)
    {
        _queries = queries;
        _excelExportService = excelExportService;
        _department = department;
        _employee = employee;
    }



    // GET: api/attendance/queries/{id}
    [HttpGet("{id:int}")]
    [Authorize(Policy = HRPermissions.Attendance.View)]
    public async Task<IActionResult> GetById(int id,
        CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetByIdAsync(
                id,
                cancellationToken);


        if (!result.Success)
            return NotFound(result);


        return Ok(result);
    }


    // GET:
    // api/attendance/queries/all
    [HttpGet("all")]
    [Authorize(Policy = HRPermissions.Attendance.View)]
    public async Task<IActionResult> GetAll(
        [FromQuery] AttendanceFilterDto filter,
        [FromQuery] PagedRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetAllAsync(
                filter,
                request,
                cancellationToken);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }



    // GET:
    // api/attendance/queries/employee/{employeeId}
    [HttpGet("employee/{employeeId:int}")]
    [Authorize(Policy = HRPermissions.Attendance.View)]
    public async Task<IActionResult> GetEmployeeAttendance(int employeeId,
        [FromQuery] AttendanceFilterDto filter,
        CancellationToken cancellationToken)
    {
        filter.EmployeeId = employeeId;


        var result =
            await _queries.GetEmployeeAttendanceAsync(
                filter,
                cancellationToken);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }



    // GET:
    // api/attendance/queries/daily?date=2026-07-23
    [HttpGet("daily")]
    [Authorize(Policy = HRPermissions.Attendance.View)]
    public async Task<IActionResult> GetDailyAttendance(
        [FromQuery] DateOnly date,
        CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetDailyAttendanceAsync(
                date,
                cancellationToken);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }



    // GET:
    // api/attendance/queries/monthly-summary
    // ?employeeId=1&month=7&year=2026
    [HttpGet("monthly-summary")]
    [Authorize(Policy = HRPermissions.Attendance.View)]
    public async Task<IActionResult> GetMonthlySummary(
        [FromQuery] int employeeId,
        [FromQuery] int month,
        [FromQuery] int year,
        CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetEmployeeMonthlySummaryAsync(
                employeeId,
                month,
                year,
                cancellationToken);


        if (!result.Success)
            return NotFound(result);


        return Ok(result);
    }


    //=============== Dashboard ==================

    [HttpGet("dashboard")]
    [Authorize(Policy = HRPermissions.Attendance.View)]
    public async Task<IActionResult> GetDashboard(
    [FromQuery] DateOnly? date,
    CancellationToken cancellationToken)
    {
        var result = await _queries.GetDashboardAsync(
            date,
            cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("trend")]
    [Authorize(Policy = HRPermissions.Attendance.View)]
    public async Task<IActionResult> GetTrend(
    [FromQuery] DateOnly? from,
    [FromQuery] DateOnly? to,
    CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetAttendanceTrendAsync(
                from,
                to,
                cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }


    [HttpGet("department-dashboard")]
    [Authorize(Policy = HRPermissions.Attendance.View)]
    public async Task<IActionResult> GetDepartmentDashboard(
    [FromQuery] DateOnly? date,
    CancellationToken cancellationToken)
    {
        var result =
            await _queries.GetDepartmentDashboardAsync(
                date,
                cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

  

    //=============== Performance Dashboard ==================


    [HttpGet("top-performance")]
    [Authorize(Policy = HRPermissions.Attendance.View)]
    public async Task<IActionResult> GetTopPerformanceEmployees(
        [FromQuery] int year,
        [FromQuery] int month,
        [FromQuery] int top = 10,
        CancellationToken cancellationToken = default)
    {
        var result =
            await _queries.GetTopPerformanceEmployeesAsync(
                year,
                month,
                top,
                cancellationToken);


        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }



    [HttpGet("low-performance")]
    [Authorize(Policy = HRPermissions.Attendance.View)]
    public async Task<IActionResult> GetLowPerformanceEmployees(
        [FromQuery] int year,
        [FromQuery] int month,
        [FromQuery] int top = 10,
        CancellationToken cancellationToken = default)
    {
        var result =
            await _queries.GetLowPerformanceEmployeesAsync(
                year,
                month,
                top,
                cancellationToken);


        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    //================ Export ==============

    [HttpGet("export-daily")]
    [Authorize(Policy = HRPermissions.Attendance.View)]
    public async Task<IActionResult> ExportDailyAttendance(
    [FromQuery] DateOnly date,
    CancellationToken cancellationToken)
    {
        var result =
            await _excelExportService.ExportDailyAttendanceAsync(
                date,
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



        return File(
            result.Data,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"DailyAttendance_{date}.xlsx");
    }


    [HttpGet("export")]
    [Authorize(Policy = HRPermissions.Attendance.View)]
    public async Task<IActionResult> ExportAttendance(
    [FromQuery] AttendanceFilterDto filter,
    CancellationToken cancellationToken)
    {
       

        string? employeeName = null;
        string? departmentName = null;

    

        // =========================================================
        // Department Name
        // =========================================================

        if (filter.DepartmentId.HasValue)
        {
            var department =
                await _department.GetByIdAsync(
                    filter.DepartmentId.Value);

            departmentName =
                department?.NameEn;
        }

        // =========================================================
        // File Name
        // =========================================================

        var fileName =
            BuildAttendanceFileName(
                filter,
                employeeName,
                departmentName);

        fileName += ".xlsx";
        // =========================================================
        // Export
        // =========================================================

        if (filter.EmployeeId.HasValue)
        {
            var result =
                await _excelExportService
                    .ExportEmployeeAttendanceAsync(
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

            var employee =
                await _employee.GetByIdAsync(
                    filter.EmployeeId.Value);

            employeeName =
                employee?.User?.FullName;
            fileName =
            BuildAttendanceFileName(
                filter,
                employeeName,
                departmentName);

            fileName += ".xlsx";

            return File(
                result.Data,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }
        else
        {
            var result =
                await _excelExportService
                    .ExportAttendanceAsync(
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
            fileName =
            BuildAttendanceFileName(
                filter,
                null,
                departmentName);

            fileName += ".xlsx";

            return File(
                result.Data,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }
        // =========================================================
        // Return File
        // =========================================================

    }


    [HttpGet("export-performance")]
    [Authorize(Policy = HRPermissions.Attendance.View)]
    public async Task<IActionResult> ExportPerformanceAsync(
    [FromQuery] int year ,
    [FromQuery] int month,
    CancellationToken cancellationToken)
    {
    


        // =========================================================
        // Export
        // =========================================================

       
          var result =
                await _excelExportService
                      .ExportPerformanceAsync(year,month,
                  cancellationToken = default);
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


        var monthName =
            GetMonthName(month);

        var fileName = $"Attendance_Performance_From:{monthName}-{year}";
           

        fileName += ".xlsx";

        // =========================================================
        // Return File
        // =========================================================

        return File(
            result.Data,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }


    [HttpGet("export-EmployeePerformance")]
    [Authorize(Policy = HRPermissions.Attendance.View)]
    public async Task<IActionResult> ExportEmployeePerformanceAsync(
    [FromQuery] int employeeId,
    [FromQuery] int year ,
    [FromQuery] int month,
    CancellationToken cancellationToken)
    {
    


        // =========================================================
        // Export
        // =========================================================

       
          var result =
                await _excelExportService
                      .ExportEmployeePerformanceAsync(employeeId,year, month,
                  cancellationToken = default);
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
        var employee =
               await _employee.GetByIdAsync(
                   employeeId);
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

        // =========================================================
        // Return File
        // =========================================================

        return File(
            result.Data,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }



    [HttpGet("export-employee")]
    [Authorize(Policy = HRPermissions.Attendance.View)]
    public async Task<IActionResult> ExportEmployeeAttendance(
    [FromQuery] AttendanceFilterDto filter,
    CancellationToken cancellationToken)
    {

        var result =
            await _excelExportService
            .ExportEmployeeAttendanceAsync(
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

        return File(
            result.Data,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Employee_Attendance_{DateTime.Now:yyyyMMdd}.xlsx");
    }


    private string BuildAttendanceFileName(
    AttendanceFilterDto filter,
    string? employeeName,
    string? departmentName)
    {
        var parts = new List<string>
    {
        "Attendance"
    };

        if (!string.IsNullOrWhiteSpace(employeeName))
        {
            parts.Add(
                SanitizeFileName(employeeName));
        }

        if (!string.IsNullOrWhiteSpace(departmentName))
        {
            parts.Add(
                SanitizeFileName(departmentName));
        }

        if (filter.Status.HasValue)
        {
            parts.Add(
                filter.Status.Value.ToString());
        }

        if (filter.DateFrom.HasValue)
        {
            parts.Add(
                $"from-{filter.DateFrom:yyyy-MM-dd}");
        }

        if (filter.DateTo.HasValue)
        {
            parts.Add(
                $"to-{filter.DateTo:yyyy-MM-dd}");
        }

        return string.Join("_", parts);
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