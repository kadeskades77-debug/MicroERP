public class TopLateEmployeeDto
{
    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public string DepartmentName { get; set; } = string.Empty;

    public int LateDays { get; set; }

    public int TotalLateMinutes { get; set; }

    public int TotalLostMinutes { get; set; }

    public int AverageLateMinutes { get; set; }
    public int AverageLostMinutes { get; set; }
}