

using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Payrolls.DTOs
{
    public class PayrollItemDto
    {
        public int SalaryComponentId { get; set; }

        public string ComponentName { get; set; } = null!;


        public SalaryComponentType Type { get; set; }


        public decimal Amount { get; set; }
    }
}
