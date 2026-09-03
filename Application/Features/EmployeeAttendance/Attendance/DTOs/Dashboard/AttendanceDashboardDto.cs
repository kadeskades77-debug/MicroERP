public class AttendanceDashboardDto
{
    public int TotalEmployees { get; set; }

    public int ExpectedEmployees { get; set; }

    public int PresentEmployees { get; set; }

    public int AbsentEmployees { get; set; }

    public int OnLeaveEmployees { get; set; }

    public int HolidayEmployees { get; set; }

    public int WeekendEmployees { get; set; }

    public int PartialAttendanceEmployees { get; set; }

    public int LateEmployees { get; set; }

    public int EarlyLeaveEmployees { get; set; }

    public int LostTimeEmployees { get; set; }

    public int MissingCheckInEmployees { get; set; }

    public int MissingCheckOutEmployees { get; set; }

    public int TotalWorkedMinutes { get; set; }

    public int TotalExpectedMinutes { get; set; }

    public int TotalLateMinutes { get; set; }

    public int TotalLostMinutes { get; set; }

    public decimal AverageLateMinutes { get; set; }

    public decimal AverageLostMinutes { get; set; }

    public decimal AttendanceRate { get; set; }

    public decimal AbsenceRate { get; set; }
}