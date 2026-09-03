using MicroERP.Domin.Common;
using MicroERP.Domin.Enums;

namespace MicroERP.Domin.Entities.EmployeeAttendance
{
    public class Holiday : BaseEntity
    {
        public string Name { get; set; } = null!;

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public HolidayType Type { get; set; }

        public bool IsRecurring { get; set; }  //كل سنة يعاد تطبيقه تلقائيًا

        public string? Notes { get; set; }
    }
}
