public class TopPerformanceEmployeeDto
{
    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public string DepartmentName { get; set; } = string.Empty;


    // النتيجة المحفوظة من AttendancePerformance
    public int AttendanceScore { get; set; }


    public int WorkingDays { get; set; }

    public int PresentDays { get; set; }

    public int AbsentDays { get; set; }

    public int LeaveDays { get; set; }


    public int AnnualLeaveDays { get; set; }

    public int SickLeaveDays { get; set; }

    public int UnpaidLeaveDays { get; set; }


    public int PartialAttendanceDays { get; set; }


    public int WorkedMinutes { get; set; }

    public int ExpectedMinutes { get; set; }


    public int TotalLateMinutes { get; set; }

    public int TotalLostMinutes { get; set; }

    public int TotalEarlyLeaveMinutes { get; set; }

    public int EarlyLeaveDays { get; set; }


    public decimal TotalPenaltyPoints { get; set; }


    public decimal AttendanceRate { get; set; }

    public decimal PunctualityRate { get; set; }
}