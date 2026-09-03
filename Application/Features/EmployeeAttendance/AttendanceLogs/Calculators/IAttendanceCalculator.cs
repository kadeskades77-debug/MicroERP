using MicroERP.Domin.Entities.EmployeeAttendance;

namespace MicroERP.Application.Features.EmployeeAttendance.AttendanceLogs.Calculators;

public interface IAttendanceCalculator
{
    Task CalculateAsync(
        AttendanceRecord attendance,
        CancellationToken cancellationToken = default);
}