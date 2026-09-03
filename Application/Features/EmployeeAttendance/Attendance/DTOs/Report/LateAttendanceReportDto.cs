namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Report;

public class LateAttendanceReportDto
{
    public int EmployeeId { get; set; }


    public string EmployeeName { get; set; } = string.Empty;


    // عدد أيام التأخير
    public int LateDays { get; set; }


    // مجموع دقائق التأخير
    public int TotalLateMinutes { get; set; }


    // متوسط التأخير
    public int AverageLateMinutes { get; set; }
}