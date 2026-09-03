

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces
{
    public interface IAttendanceAbsentService
    {
        Task CreateAbsentRecordsAsync(
        DateOnly date,
        CancellationToken cancellationToken = default);
    }
}
