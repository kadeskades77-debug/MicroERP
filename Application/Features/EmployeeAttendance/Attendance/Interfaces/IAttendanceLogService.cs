using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;

public interface IAttendanceLogService
{
    Task<Result<AttendanceLogDto>> ReceiveAsync(
        CreateAttendanceLogDto dto,
        CancellationToken cancellationToken = default);
}