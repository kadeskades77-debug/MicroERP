

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Overtime
{
    public class OvertimeReportResultDto
    {
        public int RecordsCount { get; set; }

        public int EmployeesCount { get; set; }

        public int TotalMinutes { get; set; }

        public decimal TotalHours { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal PaidAmount { get; set; }

        public decimal UnpaidAmount { get; set; }

        public List<OvertimeReportDto> Overtimes { get; set; } = [];
    }
}
