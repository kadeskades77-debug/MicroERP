

using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Payrolls.Reports.DTOs
{
    public class PayrollItemDetailsDto
    {
        public int SalaryComponentId { get; set; }

        public string ComponentName { get; set; } = null!;

        public SalaryComponentType Type { get; set; }

        public decimal Amount { get; set; }
    }
}
