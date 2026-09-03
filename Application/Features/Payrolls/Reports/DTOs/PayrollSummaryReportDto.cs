

namespace MicroERP.Application.Features.Payrolls.Reports.DTOs
{
    public class PayrollSummaryReportDto
    {
        public int Year { get; set; }

        public int Month { get; set; }

        public List<PayrollSummaryDto> Payrolls { get; set; } = [];

        public int EmployeesCount { get; set; }

        public decimal TotalGrossSalary { get; set; }

        public decimal TotalAllowances { get; set; }

        public decimal TotalOvertime { get; set; }

        public decimal TotalDeductions { get; set; }

        public decimal TotalNetSalary { get; set; }
    }
}
