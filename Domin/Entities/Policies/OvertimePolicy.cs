using MicroERP.Domin.Common;

namespace MicroERP.Domin.Entities.Policies
{ 
    public class OvertimePolicy : BaseEntity
    {
        public string Name { get; set; } = string.Empty;


        public decimal NormalMultiplier { get; set; }


        public decimal WeekendMultiplier { get; set; }

        // أقل دقائق حضور في الجمعة والإجازة الرسمية لاعتبارها عمل
        public int MinimumWeekendHolidayWorkMinutes { get; set; } = 60;

        public decimal HolidayMultiplier { get; set; }


        public int WorkingHoursPerDay { get; set; }


        public int MaxHoursPerDay { get; set; }


        public bool RequireApproval { get; set; }


        public bool IsDefault { get; set; }
    }
}
