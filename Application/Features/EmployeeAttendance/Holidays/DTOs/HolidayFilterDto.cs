using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeAttendance.Holidays.DTOs
{
    public class HolidayFilterDto
    {
        public HolidayType? Type { get; set; }

        public int? Year { get; set; }

        public bool? IsRecurring { get; set; }
    }
}
