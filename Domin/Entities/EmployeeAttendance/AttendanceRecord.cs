using MicroERP.Domin.Common;
using MicroERP.Domin.Entities.Employees;
using MicroERP.Domin.Enums;

namespace MicroERP.Domin.Entities.EmployeeAttendance
{
    public class AttendanceRecord : BaseEntity
    {
        public int EmployeeId { get; set; }

        public Employee Employee { get; set; } = null!;


        public DateOnly Date { get; set; }


        public int WorkedMinutes { get; set; }

        public int ExpectedMinutes { get; set; }

        public int LateMinutes { get; set; }


        public int EarlyLeaveMinutes { get; set; }

        public int LostTimeMinutes { get; set; }

        public AttendanceStatus Status { get; set; }

        public bool IsMissingCheckIn { get; set; }

        public bool IsMissingCheckOut { get; set; }

        public bool HasPermission { get; set; } = true;

        public string? Notes { get; set; }

        public ICollection<AttendanceTransaction> Transactions { get; set; }
    = new List<AttendanceTransaction>();
    }
}
