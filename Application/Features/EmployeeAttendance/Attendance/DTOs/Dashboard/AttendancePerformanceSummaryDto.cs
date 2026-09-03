namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Dashboard;

public class AttendancePerformanceSummaryDto
{
    public int EmployeeId { get; set; }


    public string EmployeeName { get; set; } = string.Empty;


    public int AttendanceScore { get; set; }


    // الحضور
    public int PresentDays { get; set; }

    public int AbsentDays { get; set; }

    public int PartialAttendanceDays { get; set; }


    // الإجازات
    public int AnnualLeaveDays { get; set; }

    public int SickLeaveDays { get; set; }

    public int UnpaidLeaveDays { get; set; }


    // البصمة
    public int TotalLateMinutes { get; set; }

    public int TotalLostTimeMinutes { get; set; }

    public int TotalEarlyLeaveMinutes { get; set; }

    public int EarlyLeaveDays { get; set; }


    // التقييم
    public decimal TotalPenaltyPoints { get; set; }
}