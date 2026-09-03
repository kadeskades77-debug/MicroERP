using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.AttendanceTransactions.DTOs;

namespace MicroERP.Application.Features.EmployeeAttendance.AttendanceTransactions.Interfaces;

public interface IAttendanceTransactionService
{
    Task<Result<AttendanceTransactionDto>> CreateAsync(
        CreateAttendanceTransactionDto dto,
        CancellationToken cancellationToken = default);



    Task<Result<List<AttendanceTransactionDto>>> GetByAttendanceAsync(
        int attendanceRecordId,
        CancellationToken cancellationToken = default);
}