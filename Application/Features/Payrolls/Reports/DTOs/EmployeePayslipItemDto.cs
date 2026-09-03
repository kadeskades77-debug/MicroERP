using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Payrolls.Reports.DTOs
{
    public class EmployeePayslipItemDto
    {
        public int Id { get; set; }

        public int? SalaryComponentId { get; set; }

        public string ItemName { get; set; } = null!;

        public string? Description { get; set; }

        public SalaryComponentType Type { get; set; }

        public decimal Amount { get; set; }

        public PayrollItemSource Source { get; set; }
    }
}
