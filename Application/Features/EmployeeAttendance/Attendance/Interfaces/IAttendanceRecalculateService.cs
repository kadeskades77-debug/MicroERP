using MicroERP.Application.Common.Models;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;

public interface IAttendanceRecalculateService
{
    Task<Result<bool>> RecalculateDayAsync(
        DateOnly date,
        CancellationToken cancellationToken = default);


    Task<Result<bool>> RecalculateMonthAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default);


    Task<Result<bool>> RecalculateEmployeeAsync(
        int employeeId,
        int year,
        int month,
        CancellationToken cancellationToken = default);


    Task<Result<bool>> RecalculateAllAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default);
}