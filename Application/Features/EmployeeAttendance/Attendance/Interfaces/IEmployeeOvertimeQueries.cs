using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Overtime;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;

public interface IEmployeeOvertimeQueries
{
    Task<Result<EmployeeOvertimeDto>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);


    Task<Result<List<EmployeeOvertimeDto>>> GetAllAsync(
        CancellationToken cancellationToken = default);


    Task<Result<List<EmployeeOvertimeDto>>> GetEmployeeHistoryAsync(
        int employeeId,
        CancellationToken cancellationToken = default);


    Task<Result<List<EmployeeOvertimeDto>>> GetPendingAsync(
        CancellationToken cancellationToken = default);


    Task<Result<List<EmployeeOvertimeDto>>> GetUnpaidAsync(
        CancellationToken cancellationToken = default);
}