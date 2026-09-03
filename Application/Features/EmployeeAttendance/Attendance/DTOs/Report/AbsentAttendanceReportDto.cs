namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Report;

public class AbsentAttendanceReportDto
{
    public int EmployeeId { get; set; }


    public string EmployeeName { get; set; } = string.Empty;


    // عدد أيام الغياب
    public int AbsentDays { get; set; }


    // مجموع أيام الفترة
    public int TotalDays { get; set; }


    // نسبة الغياب
    public decimal AbsencePercentage { get; set; }


    // نقاط الخصم
    public int PenaltyPoints { get; set; }
}
