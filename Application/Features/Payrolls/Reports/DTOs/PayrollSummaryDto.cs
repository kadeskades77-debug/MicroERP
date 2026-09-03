

using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Payrolls.Reports.DTOs
{
    public class PayrollSummaryDto
    {
        public int PayrollId { get; set; }

        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; } = null!;

        public string Department { get; set; } = null!;

        public decimal GrossSalary { get; set; }

        public decimal TotalAllowances { get; set; }

        public decimal TotalOvertime { get; set; }

        public decimal TotalDeductions { get; set; }

        public decimal NetSalary { get; set; }

        public PayrollStatus Status { get; set; }
    }
}
