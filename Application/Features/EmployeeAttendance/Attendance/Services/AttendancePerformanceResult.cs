namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Services;

public class AttendancePerformanceResult
{
    public int FinalScore { get; set; }


    public int PresentDays { get; set; }

    public int EarlyLeaveDays { get; set; }

    public int AbsentDays { get; set; }

    public int AnnualLeaveDays { get; set; }

    public int SickLeaveDays { get; set; }

    public int EmergencyLeaveDays { get; set; }

    public int UnpaidLeaveDays { get; set; }

    public int PartialAttendanceDays { get; set; }


    public int MissingCheckInCount { get; set; }


    public int MissingCheckOutCount { get; set; }


    public int TotalLateMinutes { get; set; }


    public int TotalEarlyLeaveMinutes { get; set; }

    public int TotalLostTimeMinutes { get; set; }


    public decimal TotalPenaltyPoints { get; set; }
}