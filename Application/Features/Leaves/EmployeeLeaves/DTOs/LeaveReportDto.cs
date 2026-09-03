using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Leaves.EmployeeLeaves.DTOs
{
    public class LeaveReportDto
    {
        public int LeaveId { get; set; }

        public int EmployeeId { get; set; }

        public string EmployeeCode { get; set; } = string.Empty;

        public string EmployeeName { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public LeaveType LeaveType { get; set; }

        public LeaveStatus Status { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public int TotalDays { get; set; }

        public int SickDays { get; set; }

        public int EmergencyDays { get; set; }

        public int AnnualDays { get; set; }

        public int UnpaidDays { get; set; }

        public string? Reason { get; set; }
    }
}
