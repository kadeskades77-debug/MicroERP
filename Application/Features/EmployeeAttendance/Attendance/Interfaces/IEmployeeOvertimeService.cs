using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Overtime;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;

public interface IEmployeeOvertimeService
{
    Task<Result> CreateManualOvertimeAsync(
     CreateManualOvertimeDto dto,
     CancellationToken cancellationToken = default);

    Task<Result> UpdateOvertimeAsync(int id,
    UpdateOvertimeDto dto,
    CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(int id,
        CancellationToken cancellationToken = default);

    Task<Result> ApproveAsync(int id,
        CancellationToken cancellationToken = default);

    Task<Result> RejectAsync(int id,
        string rejectionReason,
        CancellationToken cancellationToken = default);

    Task<Result> CancelAsync(int id,
     CancelEmployeeOvertimeDto dto,
     CancellationToken cancellationToken = default);
}