using MicroERP.Domin.Common;
using MicroERP.Domin.Enums;

namespace MicroERP.Domin.Entities.Payrolls
{
    public class PayrollPeriod : BaseEntity
    {
        public int Year { get; set; }

        public int Month { get; set; }

        public PayrollPeriodStatus Status { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public DateTime? ClosedOn { get; set; }

        public ICollection<Payroll> Payrolls { get; set; }
            = new List<Payroll>();
    }
}
