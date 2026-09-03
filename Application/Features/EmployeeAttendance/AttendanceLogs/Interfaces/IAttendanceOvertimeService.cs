using MicroERP.Application.Features.EmployeeAttendance.AttendanceLogs.Calculators;
using MicroERP.Domin.Entities.EmployeeAttendance;

namespace MicroERP.Application.Features.EmployeeAttendance.AttendanceLogs.Interfaces;

public interface IAttendanceOvertimeService
{
    Task CreateAsync(
          AttendanceRecord attendance,
          ShiftCalculationResult firstShift,
          ShiftCalculationResult? secondShift,
          CancellationToken cancellationToken = default);

    Task CreateHolidayWeekendOvertimeAsync(
    AttendanceRecord attendance,
    CancellationToken cancellationToken = default);
}