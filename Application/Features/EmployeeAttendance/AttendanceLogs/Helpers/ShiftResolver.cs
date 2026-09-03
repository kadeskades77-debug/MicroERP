using MicroERP.Domin.Entities.EmployeeAttendance;
using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeAttendance.AttendanceLogs.Helpers;

public class ShiftResolver : IShiftResolver
{
    public ShiftNumber Resolve(
       WorkSchedule schedule,
       TimeOnly time,
       AttendanceTransactionType type)
    {
        // =========================================================
        // Check In
        // =========================================================

        if (type == AttendanceTransactionType.CheckIn)
        {
            // First Shift
            if (time >= schedule.FirstShiftStart &&
                time <= schedule.FirstShiftEnd)
            {
                return ShiftNumber.First;
            }

            // Second Shift
            if (schedule.SecondShiftStart.HasValue &&
                schedule.SecondShiftEnd.HasValue &&
                time >= schedule.SecondShiftStart.Value &&
                time <= schedule.SecondShiftEnd.Value)
            {
                return ShiftNumber.Second;
            }

            // دخول خارج الشفتات الرسمية
            return ShiftNumber.None;
        }

        // =========================================================
        // Check Out
        // =========================================================

        // الـ OUT يأخذ الشفت من آخر IN
        // ولا يتم تحديد الشفت من الوقت نفسه.
        if (type == AttendanceTransactionType.CheckOut)
        {
            return ShiftNumber.None;
        }

        return ShiftNumber.None;
    }
}