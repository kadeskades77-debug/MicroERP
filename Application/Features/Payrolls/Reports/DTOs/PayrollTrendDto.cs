

namespace MicroERP.Application.Features.Payrolls.Reports.DTOs
{
    public class PayrollTrendDto
    {
        public int PayrollPeriodId { get; set; }

        public string Period { get; set; } = null!;

        public decimal TotalGrossSalary { get; set; }

        public decimal TotalNetSalary { get; set; }

        public decimal TotalDeductions { get; set; }

        public decimal TotalOvertime { get; set; }
    }
}
