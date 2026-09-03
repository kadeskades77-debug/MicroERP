using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Dashboard;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Report;

public interface IMyAttendanceQueries
{
    Task<Result<List<AttendanceDto>>> GetMyAttendanceAsync(
        AttendanceFilterDto filter,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result<AttendanceDto>> GetMyAttendanceByDateAsync(
        DateOnly date,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result<AttendanceSummaryDto>> GetMyMonthlySummaryAsync(
        int month,
        int year,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result<AttendancePerformanceDto>> GetMyPerformanceAsync(
        int month,
        int year,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result<List<AttendancePerformanceDto>>> GetMyPerformanceHistoryAsync(
        string userId,
        CancellationToken cancellationToken = default);
}