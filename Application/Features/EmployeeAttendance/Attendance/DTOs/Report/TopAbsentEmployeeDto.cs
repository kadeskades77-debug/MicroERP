public class TopAbsentEmployeeDto
{
    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public string DepartmentName { get; set; } = string.Empty;

    public int AbsentDays { get; set; }

    public int PenaltyPoints { get; set; }
}