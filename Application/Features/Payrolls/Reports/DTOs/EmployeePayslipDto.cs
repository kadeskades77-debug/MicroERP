using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Payrolls.Reports.DTOs
{
    public class EmployeePayslipDto
    {
        public int PayrollId { get; set; }

        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; } = null!;

        public string Department { get; set; } = null!;

        public int Year { get; set; }

        public int Month { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public decimal GrossSalary { get; set; }

        public decimal TotalAllowances { get; set; }

        public decimal TotalOvertime { get; set; }

        public decimal TotalDeductions { get; set; }

        public decimal NetSalary { get; set; }

        public PayrollStatus Status { get; set; }

        public DateTime? ApprovedOn { get; set; }

        public DateTime? PaidOn { get; set; }

        public string? BankName { get; set; }

        public string? BankAccountNumber { get; set; }

        public string? IBAN { get; set; }

        public List<EmployeePayslipItemDto> Items { get; set; } = [];
    }
}
