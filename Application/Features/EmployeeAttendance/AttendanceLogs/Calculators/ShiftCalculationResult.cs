

using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeAttendance.AttendanceLogs.Calculators
{
    public class ShiftCalculationResult
    {
        public ShiftNumber Shift { get; set; }

        public DateTime? CheckIn { get; set; }

        public DateTime? CheckOut { get; set; }

        public int WorkedMinutes { get; set; }

        public DateTime? OvertimeStart { get; set; }

        public int OvertimeMinutes { get; set; }

        public int LateMinutes { get; set; }

        public int EarlyLeaveMinutes { get; set; }

        public bool IsMissingCheckIn { get; set; }

        public bool IsMissingCheckOut { get; set; }
    }
}
