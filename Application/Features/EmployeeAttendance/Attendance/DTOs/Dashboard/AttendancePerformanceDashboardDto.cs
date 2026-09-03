namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Dashboard
{
    public class AttendancePerformanceDashboardDto
    {
        // الموظفين
        public int TotalEmployees { get; set; }

        public int EvaluatedEmployees { get; set; }


        // المتوسط العام
        public decimal AverageScore { get; set; }


        // أعلى وأقل تقييم
        public int HighestScore { get; set; }

        public int LowestScore { get; set; }


        // الخصومات
        public decimal TotalPenaltyPoints { get; set; }


        // الحضور
        public int TotalAbsentDays { get; set; }

        public int TotalLateMinutes { get; set; }

        public int TotalLostTimeMinutes { get; set; }


        // توزيع الأداء
        public int ExcellentCount { get; set; }   // 90 - 100

        public int GoodCount { get; set; }        // 75 - 89

        public int AverageCount { get; set; }     // 60 - 74

        public int WeakCount { get; set; }        // أقل من 60
    }
}
