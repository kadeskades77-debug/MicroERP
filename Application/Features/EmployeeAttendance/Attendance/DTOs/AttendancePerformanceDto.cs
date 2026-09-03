namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;

public class AttendancePerformanceDto
{
    public int Id { get; set; }


    public int EmployeeId { get; set; }


    public string EmployeeName { get; set; } = string.Empty;


    public int Year { get; set; }


    public int Month { get; set; }


    // التقييم النهائي
    public int AttendanceScore { get; set; }


    public int PresentDays { get; set; }


    public int AbsentDays { get; set; }

    public int EarlyLeaveDays { get; set; }

    public int AnnualLeaveDays { get; set; }

    public int SickLeaveDays { get; set; }

    public int UnpaidLeaveDays { get; set; }

    public int PartialAttendanceDays { get; set; }


    public int MissingCheckInCount { get; set; }


    public int MissingCheckOutCount { get; set; }


    public int TotalLateMinutes { get; set; }

    public int TotalLostTimeMinutes { get; set; }


    public int TotalEarlyLeaveMinutes { get; set; }


    public decimal TotalPenaltyPoints { get; set; }


    public string? Notes { get; set; }


    public DateTime CreatedOn { get; set; }
}