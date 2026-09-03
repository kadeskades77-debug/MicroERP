

using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Payrolls.DTOs
{
    public class PayrollDetailsDto
    {
        public int Id { get; set; }


        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; } = null!;


        public string Period { get; set; } = null!;


        public decimal GrossSalary { get; set; }

        public decimal TotalAllowances { get; set; }

        public decimal TotalDeductions { get; set; }

        public decimal NetSalary { get; set; }


        public PayrollStatus Status { get; set; }

        public DateTime? ApprovedOn { get; set; }

        public string? ApprovedByUserName { get; set; }

        public DateTime? PaidOn { get; set; }

        public string? PaidByUserName { get; set; }

        public List<PayrollItemDto> Items { get; set; }
            = new();
    }
}
