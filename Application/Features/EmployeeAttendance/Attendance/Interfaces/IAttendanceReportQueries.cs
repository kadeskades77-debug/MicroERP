using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Report;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;

public interface IAttendanceReportQueries
{
    Task<Result<List<AttendanceReportDto>>> GetAttendanceReportAsync(
        AttendanceReportFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<Result<List<LateAttendanceReportDto>>>
    GetLateReportAsync(
        LateAttendanceFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<Result<List<AbsentAttendanceReportDto>>>
    GetAbsentReportAsync(
        AbsentAttendanceFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<Result<List<TopLateEmployeeDto>>> GetTopLateEmployeesAsync(
     DateOnly? from = null,
     DateOnly? to = null,
     int top = 10,
     CancellationToken cancellationToken = default);

    Task<Result<List<TopAbsentEmployeeDto>>> GetTopAbsentEmployeesAsync(
    DateOnly? from = null,
    DateOnly? to = null,
    int top = 10,
    CancellationToken cancellationToken = default);
}