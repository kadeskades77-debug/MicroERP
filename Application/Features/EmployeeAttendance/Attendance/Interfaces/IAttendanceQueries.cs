using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Dashboard;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Report;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;

public interface IAttendanceQueries
{
    Task<Result<AttendanceDto>> GetByIdAsync(int id,
        CancellationToken cancellationToken = default);


    Task<Result<PagedResult<AttendanceDto>>> GetAllAsync(AttendanceFilterDto filter,
        PagedRequest request,
        CancellationToken cancellationToken = default);


    Task<Result<List<AttendanceDto>>> GetEmployeeAttendanceAsync(AttendanceFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<Result<List<AttendanceDto>>> GetDailyAttendanceAsync(
    DateOnly date,
    CancellationToken cancellationToken = default);


    Task<Result<AttendanceSummaryDto>> GetEmployeeMonthlySummaryAsync(
        int employeeId,
        int month,
        int year,
        CancellationToken cancellationToken = default);

    //======================== Dashboard =================

    Task<Result<AttendanceDashboardDto>> GetDashboardAsync(
    DateOnly? date = null,
    CancellationToken cancellationToken = default);

    Task<Result<List<AttendanceTrendDto>>> GetAttendanceTrendAsync(
    DateOnly? from = null,
    DateOnly? to = null,
    CancellationToken cancellationToken = default);

    Task<Result<List<DepartmentAttendanceDashboardDto>>>
    GetDepartmentDashboardAsync(
        DateOnly? date = null,
        CancellationToken cancellationToken = default);


    Task<Result<List<TopPerformanceEmployeeDto>>> GetTopPerformanceEmployeesAsync(
    int year,
    int month,
    int top = 10,
    CancellationToken cancellationToken = default);

    Task<Result<List<TopPerformanceEmployeeDto>>> GetLowPerformanceEmployeesAsync(
        int year,
        int month,
        int top = 10,
        CancellationToken cancellationToken = default);
}