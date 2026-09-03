using MicroERP.Domin.Entities.EmployeeAttendance;
using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeAttendance.AttendanceLogs.Helpers;

public interface IShiftResolver
{
    ShiftNumber Resolve(
      WorkSchedule schedule,
      TimeOnly time,
      AttendanceTransactionType typ);
}