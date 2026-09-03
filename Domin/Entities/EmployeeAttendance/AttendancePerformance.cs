using MicroERP.Domin.Common;
using MicroERP.Domin.Entities.Employees;
namespace MicroERP.Domin.Entities.EmployeeAttendance;

public class AttendancePerformance : BaseEntity
{
    public int EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;


    // =========================================================
    // Period
    // =========================================================

    public int Year { get; set; }

    public int Month { get; set; }


    // =========================================================
    // Attendance Score
    // =========================================================

    // Attendance evaluation score from 0 to 100
    public int AttendanceScore { get; set; }


    // =========================================================
    // Attendance Days
    // =========================================================

    public int PresentDays { get; set; }

    public int AbsentDays { get; set; }

    public int PartialAttendanceDays { get; set; }


    // =========================================================
    // Leave Days
    // =========================================================

    public int AnnualLeaveDays { get; set; }

    public int SickLeaveDays { get; set; }

    public int UnpaidLeaveDays { get; set; }

    public int EmergencyLeaveDays { get; set; }


    // =========================================================
    // Attendance Problems
    // =========================================================

    public int EarlyLeaveDays { get; set; }

    public int MissingCheckInCount { get; set; }

    public int MissingCheckOutCount { get; set; }


    // =========================================================
    // Lost Time
    // =========================================================

    public int TotalLateMinutes { get; set; }

    public int TotalEarlyLeaveMinutes { get; set; }

    public int TotalLostTimeMinutes { get; set; }


    // =========================================================
    // Penalties
    // =========================================================

    public decimal TotalPenaltyPoints { get; set; }


    // =========================================================
    // Notes
    // =========================================================

    public string? Notes { get; set; }
}