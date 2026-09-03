using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Overtime
{
    public class EmployeeOvertimeReportItemDto
    {
        public int Id { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public int TotalMinutes { get; set; }

        public decimal TotalHours { get; set; }

        public decimal HourlyRate { get; set; }

        public decimal Multiplier { get; set; }

        public decimal Amount { get; set; }

        public OvertimeStatus Status { get; set; }

        public OvertimeType Type { get; set; }

        public OvertimeSource Source { get; set; }

        public string? Reason { get; set; }

        public bool IsPaid { get; set; }
    }
}
