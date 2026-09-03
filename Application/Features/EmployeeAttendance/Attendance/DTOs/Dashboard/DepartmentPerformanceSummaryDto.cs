namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Dashboard;

public class DepartmentPerformanceSummaryDto
{
    public int DepartmentId { get; set; }

    public string DepartmentName { get; set; } = string.Empty;


    // عدد الموظفين
    public int EmployeesCount { get; set; }


    // الأداء
    public decimal AverageScore { get; set; }


    // الحضور
    public int TotalPresentDays { get; set; }

    public int TotalAbsentDays { get; set; }

    public int TotalPartialAttendanceDays { get; set; }


    // الإجازات
    public int TotalAnnualLeaveDays { get; set; }

    public int TotalSickLeaveDays { get; set; }

    public int TotalUnpaidLeaveDays { get; set; }


    // الوقت
    public int TotalLateMinutes { get; set; }

    public int TotalLostTimeMinutes { get; set; }

    public int TotalEarlyLeaveMinutes { get; set; }

    public int TotalEarlyLeaveDays { get; set; }


    // الخصومات
    public decimal TotalPenaltyPoints { get; set; }
}