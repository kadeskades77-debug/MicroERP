using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Leaves.EmployeeLeaves.DTOs
{
    public class LeaveDto
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; } = null!;


        public int LeaveTypeId { get; set; }

        public string LeaveType { get; set; } = null!;


        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }


        public int TotalDays { get; set; }


        public int SickDays { get; set; }

        public int EmergencyDays { get; set; }

        public int UnpaidDays { get; set; }


        public int StatusId { get; set; }

        public string Status { get; set; } = null!;


        public string? Reason { get; set; }


        public string? RejectionReason { get; set; }


        public bool HasWarning { get; set; }

        public string? WarningMessage { get; set; }


        public DateTime? ApprovedOn { get; set; }

        public DateTime? RejectedOn { get; set; }

        public string? ApprovedByUserName { get; set; }

        public string? RejectedByUserName { get; set; }
    }
}
