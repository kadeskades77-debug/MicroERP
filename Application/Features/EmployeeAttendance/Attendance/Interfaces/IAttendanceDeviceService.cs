using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;

public interface IAttendanceDeviceService
{
    Task<Result<AttendanceDeviceDto>> CreateAsync(
        CreateAttendanceDeviceDto dto,
        CancellationToken cancellationToken = default);


    Task<Result<AttendanceDeviceDto>> UpdateAsync(
        int id,
        UpdateAttendanceDeviceDto dto,
        CancellationToken cancellationToken = default);

    Task<Result> ActivateAsync(int id,
    CancellationToken cancellationToken = default);

    Task<Result> DeactivateAsync(int id,
   CancellationToken cancellationToken = default);


    Task<Result<bool>> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);
}