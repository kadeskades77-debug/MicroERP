namespace MicroERP.Application.Features.EmployeeAttendance.AttendanceLogs.Interfaces;

public interface IAttendanceLogProcessor
{
    Task ProcessPendingLogsAsync(
        CancellationToken cancellationToken = default);
}