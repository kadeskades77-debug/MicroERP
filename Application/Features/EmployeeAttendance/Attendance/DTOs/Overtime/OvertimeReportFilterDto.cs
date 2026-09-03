

using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Overtime
{
    public class OvertimeReportFilterDto
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int? EmployeeId { get; set; }
        public int? DepartmentId { get; set; }

        public OvertimeType? Type { get; set; }

        public OvertimeStatus? Status { get; set; }

        public OvertimeSource? Source { get; set; }

        public bool? IsPaid { get; set; }
    }
}
