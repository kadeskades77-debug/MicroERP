using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;

public interface IAttendanceCorrectionService
{
    Task<Result<AttendanceCorrectionDto>> CreateAsync(
        CreateAttendanceCorrectionDto dto,
        CancellationToken cancellationToken = default);



    Task<Result<bool>> ApproveAsync(
        int correctionId,
        CancellationToken cancellationToken = default);



    Task<Result<bool>> RejectAsync(
        int correctionId,
        string rejectionReason,
        CancellationToken cancellationToken = default);



    Task<Result<List<AttendanceCorrectionDto>>> GetPendingAsync(
        CancellationToken cancellationToken = default);
}