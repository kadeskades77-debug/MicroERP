

namespace MicroERP.Application.Features.Payrolls.Reports.DTOs
{
    public class AttendanceDeductionReportDto
    {
        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; } = null!;

        public string Department { get; set; } = null!;

        public int AbsentDays { get; set; }

        public int LateMinutes { get; set; }

        public int EarlyLeaveMinutes { get; set; }

        public int LostTimeMinutes { get; set; }

        public decimal DeductionAmount { get; set; }
    }
}
