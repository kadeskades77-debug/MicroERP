using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;

public interface IAttendancePerformanceService
{
    Task<Result<AttendancePerformanceDto>> CalculateAsync(
        int employeeId,
        int year,
        int month,
        CancellationToken cancellationToken = default);

    Task<Result<List<AttendancePerformanceDto>>>
    CalculateAllAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default);


}