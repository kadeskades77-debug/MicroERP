using Domin.Entities;
using MicroERP.Domin.Common;
using MicroERP.Domin.Enums;

namespace MicroERP.Domin.Entities
{
    public class EmployeeLeaveBalance : BaseEntity
    {
        public int EmployeeId { get; set; }

        public Employee Employee { get; set; } = null!;


        public int Year { get; set; }


        public LeaveType LeaveType { get; set; }


        public int TotalDays { get; set; }


        public int UsedDays { get; set; }

        public int RemainingDays =>
            TotalDays - UsedDays;
    }
}
