
using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Payrolls.DTOs
{
    public class PayrollPeriodDto
    {
        public int Id { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        public PayrollPeriodStatus Status { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }
    }
}
