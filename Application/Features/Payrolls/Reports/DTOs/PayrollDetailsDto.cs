
using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Payrolls.Reports.DTOs
{
    public class PayrollDetailsDto
    {
        public int PayrollId { get; set; }

        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; } = null!;

        public string Department { get; set; } = null!;

        public string Period { get; set; } = null!;

        public PayrollStatus Status { get; set; }

        public decimal GrossSalary { get; set; }

        public decimal TotalAllowances { get; set; }

        public decimal TotalOvertime { get; set; }

        public decimal TotalDeductions { get; set; }

        public decimal NetSalary { get; set; }

        public DateTime? ApprovedOn { get; set; }

        public string? ApprovedByUserName { get; set; }

        public DateTime? PaidOn { get; set; }

        public string? BankName { get; set; }

        public string? BankAccountNumber { get; set; }

        public string? IBAN { get; set; }

        public string? PaidByUserName { get; set; }

        public List<PayrollItemDetailsDto> Items { get; set; } = [];
    }
}
