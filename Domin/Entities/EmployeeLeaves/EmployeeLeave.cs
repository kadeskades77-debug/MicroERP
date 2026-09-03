using MicroERP.Domin.Common;
using MicroERP.Domin.Entities.Employees;
using MicroERP.Domin.Enums;
using MicroERP.Domin.Identity;

namespace MicroERP.Domin.Entities.EmployeeLeaves
{
    public class EmployeeLeave : BaseEntity
    {
        public int EmployeeId { get; set; }

        public Employee Employee { get; set; } = null!;

        public LeaveType LeaveType { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public int SickDays { get; set; }

        public int EmergencyDays { get; set; }

        public int AnnualDays { get; set; }

        public int UnpaidDays { get; set; }

        public int TotalDays { get; set; }

        public LeaveStatus Status { get; set; } = LeaveStatus.Pending;

        public string? Reason { get; set; }

        public string? RejectionReason { get; set; }
        public ApplicationUser? RejectedByUser { get; set; }
        public string? ApprovedByUserId { get; set; }
        public ApplicationUser? ApprovedByUser { get; set; }

        public string? RejectedByUserId { get; set; }

        public DateTime? ApprovedOn { get; set; }

        public DateTime? RejectedOn { get; set; }

        public ICollection<LeaveAttachment> Attachments { get; set; }
    = new List<LeaveAttachment>();
    }
}
