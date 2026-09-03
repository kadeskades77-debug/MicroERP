namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Dashboard
{
    public class AttendancePerformanceStatisticsDto
    {
        public int TotalEmployees { get; set; }

        public decimal AverageScore { get; set; }

        public int HighestScore { get; set; }

        public int LowestScore { get; set; }

        public int ExcellentEmployees { get; set; } // >= 90

        public int GoodEmployees { get; set; }      // 80 - 89

        public int AverageEmployees { get; set; }   // 70 - 79

        public int WeakEmployees { get; set; }      // < 70
    }
}
