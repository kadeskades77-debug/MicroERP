namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Dashboard;

public class AttendanceSummaryDto
{
    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; } = null!;

    public int Year { get; set; }

    public int Month { get; set; }

    // الأيام
    public int TotalDays { get; set; }

    public int PresentDays { get; set; }

    public int AbsentDays { get; set; }

    public int LeaveDays { get; set; }

    public int WeekendDays { get; set; }

    // الوقت
    public int ExpectedMinutes { get; set; }

    public int WorkedMinutes { get; set; }

    public int LateMinutes { get; set; }

    public int EarlyLeaveMinutes { get; set; }

    public int LostTimeMinutes { get; set; }

    public int WorkingDays { get; set; }

    // النسب
    public decimal AttendancePercentage { get; set; }

    public decimal AbsencePercentage { get; set; }

    public decimal PunctualityPercentage { get; set; }
}