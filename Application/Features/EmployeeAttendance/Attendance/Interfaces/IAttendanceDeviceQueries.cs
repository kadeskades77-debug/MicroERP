using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;

public interface IAttendanceDeviceQueries
{
    Task<Result<List<AttendanceDeviceDto>>> GetAllAsync(
        CancellationToken cancellationToken = default);


    Task<Result<AttendanceDeviceDto>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);
}