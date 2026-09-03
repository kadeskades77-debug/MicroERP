public class AttendanceTrendDto
{
    public DateOnly Date { get; set; }

    public int Present { get; set; }

    public int Absent { get; set; }

    public int OnLeave { get; set; }

    public int Holiday { get; set; }

    public int Weekend { get; set; }

    public int PartialAttendance { get; set; }

    public int MissingCheckIn { get; set; }

    public int MissingCheckOut { get; set; }
}