public class LateAttendanceFilterDto
{
    public int? EmployeeId { get; set; }


    public DateOnly? DateFrom { get; set; }


    public DateOnly? DateTo { get; set; }


    public int? MinimumLateMinutes { get; set; }
}