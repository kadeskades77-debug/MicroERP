using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Dashboard;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;

public interface IAttendancePerformanceQueries
{
    Task<Result<AttendancePerformanceDto>>
        GetByEmployeeAndMonthAsync(
            int employeeId,
            int year,
            int month,
            CancellationToken cancellationToken = default);

    Task<Result<List<AttendancePerformanceDto>>> GetEmployeeHistoryAsync(
       int employeeId,
       CancellationToken cancellationToken = default);

    Task<Result<PagedResult<AttendancePerformanceDto>>>
        GetPagedAsync(
            AttendancePerformanceFilterDto filter,
            PagedRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<AttendancePerformanceDashboardDto>>
    GetDashboardAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default);

    Task<Result<List<TopPerformanceEmployeeDto>>>
      GetTopEmployeesAsync(
      int year,
      int month,
      int top = 10,
      CancellationToken cancellationToken = default);


    Task<Result<List<TopPerformanceEmployeeDto>>>
        GetLowestEmployeesAsync(
        int year,
        int month,
        int top = 10,
        CancellationToken cancellationToken = default);

    Task<Result<AttendancePerformanceStatisticsDto>>
        GetMonthlyStatisticsAsync(
            int year,
            int month,
            CancellationToken cancellationToken = default);

    Task<Result<List<AttendancePerformanceSummaryDto>>>
        GetEmployeePerformanceSummaryAsync(
            int year,
            int month,
            CancellationToken cancellationToken = default);

    Task<Result<List<DepartmentPerformanceSummaryDto>>>
    GetDepartmentPerformanceSummaryAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default);
  
}