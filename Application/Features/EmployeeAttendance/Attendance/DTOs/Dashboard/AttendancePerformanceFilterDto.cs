namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Dashboard;

public class AttendancePerformanceFilterDto
{
    public int? EmployeeId { get; set; }


    public int? Year { get; set; }


    public int? Month { get; set; }


    public int? MinimumScore { get; set; }


    public int? MaximumScore { get; set; }
}