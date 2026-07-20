using Domin.Entities;
using MicroERP.Domin.Common;
using MicroERP.Domin.Enums;
namespace MicroERP.Domin.Entities
{
    public class EmployeeSpecialLeave : BaseEntity
    {
        public int EmployeeId { get; set; }

        public Employee Employee { get; set; } = null!;


        public SpecialLeaveType Type { get; set; }


        public DateOnly StartDate { get; set; }


        public DateOnly EndDate { get; set; }


        public int TotalDays { get; set; }


        public string? Reason { get; set; }

        public SpecialLeaveStatus Status { get; set; }


        public string? ApprovedByUserId { get; set; }
        public ApplicationUser? ApprovedByUser { get; set; }

        public DateTime? ApprovedOn { get; set; }


        public string? RejectedByUserId { get; set; }
        public ApplicationUser? RejectedByUser { get; set; }
        public DateTime? RejectedOn { get; set; }

        public string? RejectionReason { get; set; }

        public ICollection<LeaveAttachment> Attachments { get; set; }
    = new List<LeaveAttachment>();
    }
}
