using MicroERP.Domin.Common;
using MicroERP.Domin.Enums;

namespace MicroERP.Domin.Entities.Policies
{
    public class LeavePolicy : BaseEntity
    {
        public LeaveType LeaveType { get; set; }

        public decimal DaysPerMonth { get; set; }

        public int MaximumDaysPerYear { get; set; }
        public bool AllowNegativeBalance { get; set; }

        public int MaxNegativeDays { get; set; }
        // هل هذه الإجازة تحتاج رصيد موظف؟
        public bool RequiresBalance { get; set; }
    }
}
