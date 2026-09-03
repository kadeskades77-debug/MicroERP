namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Report;

public class AbsentAttendanceFilterDto
{
    public int? EmployeeId { get; set; }


    public DateOnly? DateFrom { get; set; }


    public DateOnly? DateTo { get; set; }
}