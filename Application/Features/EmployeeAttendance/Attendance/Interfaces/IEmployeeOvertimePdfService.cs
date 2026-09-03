
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Overtime;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces
{
    public interface IEmployeeOvertimePdfService
    {
        Task<byte[]> GenerateAsync(
    OvertimeReportFilterDto filter,
    CancellationToken cancellationToken = default);
    }
}
