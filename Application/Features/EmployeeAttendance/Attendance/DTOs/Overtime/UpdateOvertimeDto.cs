using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Overtime
{
    public class UpdateOvertimeDto 
    {

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public decimal? HourlyRate { get; set; }

        public decimal? Multiplier { get; set; }

        public decimal? Amount { get; set; }

        public OvertimeType? Type { get; set; }

        public string? Reason { get; set; }

    }
}
