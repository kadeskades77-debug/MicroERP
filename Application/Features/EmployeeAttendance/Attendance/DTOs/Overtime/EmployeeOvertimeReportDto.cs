using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Overtime
{
    public class EmployeeOvertimeReportDto
    {
        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; } = null!;

        public string? Department { get; set; }

        public List<EmployeeOvertimeReportItemDto> Overtimes { get; set; } = [];

        public int TotalMinutes { get; set; }

        public decimal TotalHours { get; set; }

        public decimal TotalAmount { get; set; }
        public OvertimeType Type { get; set; }

        public OvertimeSource Source { get; set; }
        public decimal PaidAmount { get; set; }

        public decimal UnpaidAmount { get; set; }
    }
}
