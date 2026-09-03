using MicroERP.Application.Features.Departments.Interfaces;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Overtime;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using MicroERP.Application.Features.Employees.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Micro_ERP.Controllers.Attendance
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = HRPermissions.Overtime.View)]
    public class OvertimeReportsController : Controller
    {
        private readonly IOvertimeReportQueries _overtimeQueries;
        private readonly IOvertimeExcelExportService _overtimeExcelExportService;
        private readonly IDepartmentQueries _department;
        private readonly IEmployeeQueries _employee;
        private readonly IEmployeeOvertimePdfService _overtimePdfService;
        public OvertimeReportsController(
            IOvertimeReportQueries overtimeQueries,
            IOvertimeExcelExportService overtimeExcelExportService,
            IDepartmentQueries department,
            IEmployeeQueries employee,
            IEmployeeOvertimePdfService overtimePdfService)
        {
            _overtimeQueries = overtimeQueries;
            _overtimeExcelExportService = overtimeExcelExportService;
            _department = department;
            _employee = employee;
            _overtimePdfService = overtimePdfService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOvertimeReport(
        [FromQuery] OvertimeReportFilterDto filter,
        CancellationToken cancellationToken)
        {
            if (filter.EmployeeId.HasValue)
            {
                var result =
                    await _overtimeQueries.GetEmployeeOvertimeReportAsync(
                        filter,
                        cancellationToken);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }

            var reportResult =
                await _overtimeQueries.GetOvertimeReportAsync(
                    filter,
                    cancellationToken);

            if (!reportResult.Success)
            {
                return BadRequest(reportResult);
            }

            return Ok(reportResult);
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportOvertime(
        [FromQuery] OvertimeReportFilterDto filter,
        CancellationToken cancellationToken)
        {
            byte[] file;
            var typefile = ".xlsx";
            // =========================================================
            // Export
            // =========================================================

            if (filter.EmployeeId.HasValue)
            {
                file =
                    await _overtimeExcelExportService
                        .ExportEmployeeOvertimeAsync(
                            filter,
                            cancellationToken);
            }
            else
            {
                file =
                    await _overtimeExcelExportService
                        .ExportOvertimeAsync(
                            filter,
                            cancellationToken);
            }


            // =========================================================
            // Department Name
            // =========================================================

            string? departmentName = null;

            if (filter.DepartmentId.HasValue)
            {
                departmentName =
                    (await _department.GetByIdAsync(
                        filter.DepartmentId.Value))
                    ?.NameEn;
            }


            // =========================================================
            // Employee Name
            // =========================================================

            string? employeeName = null;

            if (filter.EmployeeId.HasValue)
            {
                employeeName =
                    (await _employee.GetByIdAsync(
                        filter.EmployeeId.Value))
                    ?.User.FullName;
            }


            // =========================================================
            // File Name
            // =========================================================

            var fileName =
                BuildOvertimeFileName(
                    filter,
                    departmentName,
                    employeeName,
                    typefile);


            return File(
                file,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        [HttpGet("export/Pdf")]
        public async Task<IActionResult> ExportOvertimePdf(
        [FromQuery] OvertimeReportFilterDto filter,
        CancellationToken cancellationToken)
        {
            byte[] file;
            var typefile = ".pdf";
            // =========================================================
            // Export
            // =========================================================


            file =
                    await _overtimePdfService
                        .GenerateAsync(
                            filter,
                            cancellationToken);
           

            // =========================================================
            // Department Name
            // =========================================================

            string? departmentName = null;

            if (filter.DepartmentId.HasValue)
            {
                departmentName =
                    (await _department.GetByIdAsync(
                        filter.DepartmentId.Value))
                    ?.NameEn;
            }


            // =========================================================
            // Employee Name
            // =========================================================

            string? employeeName = null;

            if (filter.EmployeeId.HasValue)
            {
                employeeName =
                    (await _employee.GetByIdAsync(
                        filter.EmployeeId.Value))
                    ?.User.FullName;
            }


            // =========================================================
            // File Name
            // =========================================================

            var fileName =
                BuildOvertimeFileName(
                    filter,
                    departmentName,
                    employeeName,
                    typefile);


            return File(
                file,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }


        [HttpGet("export/employee")]
        public async Task<IActionResult> ExportEmployeeOvertime(
        [FromQuery] OvertimeReportFilterDto filter,
        CancellationToken cancellationToken)
        {

            byte[] file;
            var typefile = ".xlsx";
            if (filter.EmployeeId.HasValue)
            {
                file =
                    await _overtimeExcelExportService
                        .ExportEmployeeOvertimeAsync(
                            filter,
                            cancellationToken);
            }
            else if (filter.Type.HasValue && filter.EmployeeId.HasValue)
            {
                file =
                 await _overtimeExcelExportService
                     .ExportEmployeeOvertimeTypeAsync(
                         filter,
                         cancellationToken);
            }
            else return BadRequest("EmployeeId is required.");
            if (filter.Source.HasValue&& filter.EmployeeId.HasValue)
            {
                file =
                 await _overtimeExcelExportService
                     .ExportEmployeeOvertimeSourceAsync(
                         filter,
                         cancellationToken);
            }

            string? employeeName = null;

            if (filter.EmployeeId.HasValue)
            {
                var employee =
                      (await _employee.GetByIdAsync(
                          filter.EmployeeId.Value));
                employeeName = employee?.User?.FullName;

            }

            var fileName =
                BuildOvertimeFileName(
                    filter,
                    null,
                     employeeName,
                    typefile);


            fileName += ".xlsx";

            return File(
                file,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }


        private static string BuildOvertimeFileName(
        OvertimeReportFilterDto filter,
        string? departmentName,
        string? employeeName,
        string typefile)
        {
            var parts =
                new List<string>
                {
            "Overtime-Report"
                };


            if (!string.IsNullOrWhiteSpace(departmentName))
            {
                parts.Add(
                    $"Department-{departmentName}");
            }


            if (!string.IsNullOrWhiteSpace(employeeName))
            {
                parts.Add(
                    $"Employee-{employeeName}");
            }


            if (filter.Type.HasValue)
            {
                parts.Add(
                    $"Type-{filter.Type.Value}");
            }


            if (filter.Source.HasValue)
            {
                parts.Add(
                    $"Source-{filter.Source.Value}");
            }


            if (filter.Status.HasValue)
            {
                parts.Add(
                    $"Status-{filter.Status.Value}");
            }


            if (filter.IsPaid.HasValue)
            {
                parts.Add(
                    filter.IsPaid.Value
                        ? "Paid-Yes"
                        : "Paid-No");
            }


            if (filter.FromDate.HasValue)
            {
                parts.Add(
                    $"From-{filter.FromDate.Value:yyyy-MM-dd}");
            }


            if (filter.ToDate.HasValue)
            {
                parts.Add(
                    $"To-{filter.ToDate.Value:yyyy-MM-dd}");
            }


            return string.Join("-", parts) + typefile;
        }

    }
}
