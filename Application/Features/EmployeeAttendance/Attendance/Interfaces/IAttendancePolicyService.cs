using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;

public interface IAttendancePolicyService
{
    Task<Result<List<AttendancePolicyDto>>> GetAllAsync(
        CancellationToken cancellationToken = default);


    Task<Result<AttendancePolicyDto>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);


    Task<Result<AttendancePolicyDto>> CreateAsync(
        CreateAttendancePolicyDto dto,
        CancellationToken cancellationToken = default);


    Task<Result<AttendancePolicyDto>> UpdateAsync(
        int id,
        UpdateAttendancePolicyDto dto,
        CancellationToken cancellationToken = default);


    Task<Result<bool>> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);
}