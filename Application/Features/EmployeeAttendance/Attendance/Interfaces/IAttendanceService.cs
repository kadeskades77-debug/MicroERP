using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;

public interface IAttendanceService
{
  
    Task<Result<AttendanceDto>> CreateAsync(CreateAttendanceDto dto,
        CancellationToken cancellationToken = default);


    Task<Result<AttendanceDto>> UpdateAsync(int id,UpdateAttendanceDto dto,
        CancellationToken cancellationToken = default);


    Task<Result<bool>> DeleteAsync(int id,
        CancellationToken cancellationToken = default);
}