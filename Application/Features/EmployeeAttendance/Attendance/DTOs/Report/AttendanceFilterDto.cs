using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Report;

public class AttendanceFilterDto
{
    public int? EmployeeId { get; set; }

    public DateOnly? DateFrom { get; set; }

    public DateOnly? DateTo { get; set; }

    public int? DepartmentId { get; set; }

    public AttendanceStatus? Status { get; set; }
}