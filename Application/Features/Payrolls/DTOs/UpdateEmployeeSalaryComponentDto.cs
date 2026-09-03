

namespace MicroERP.Application.Features.Payrolls.DTOs
{
    public class UpdateEmployeeSalaryComponentDto
    {
        public decimal? Amount { get; set; }

        public bool? IsActiveComponent { get; set; }

        public DateOnly? EffectiveFrom { get; set; }

        public DateOnly? EffectiveTo { get; set; }
    }
}
