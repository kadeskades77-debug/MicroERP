using MicroERP.Domin.Entities.Payrolls;

namespace MicroERP.Application.Features.Payrolls.Services.Calculators
{
    public class PayrollCalculationResult
    {
        public decimal GrossSalary { get; set; }

        public decimal TotalAllowances { get; set; }

        public decimal TotalOvertime { get; set; }

        public decimal TotalDeductions { get; set; }

        public decimal NetSalary { get; set; }

        public List<PayrollItem> Items { get; set; } = [];
    }
}
