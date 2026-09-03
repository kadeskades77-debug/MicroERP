using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeAttendance.Holidays.DTOs
{
    public class CreateHolidayDto
    {
        public string Name { get; set; } = null!;

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public HolidayType Type { get; set; }

        public bool IsRecurring { get; set; }

        public string? Notes { get; set; }
    }
}
