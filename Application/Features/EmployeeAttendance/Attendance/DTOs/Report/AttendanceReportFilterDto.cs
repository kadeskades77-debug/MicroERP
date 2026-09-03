using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Report;

public class AttendanceReportFilterDto
{
    public int? EmployeeId { get; set; }


    public DateOnly? DateFrom { get; set; }


    public DateOnly? DateTo { get; set; }


    public AttendanceStatus? Status { get; set; }
}