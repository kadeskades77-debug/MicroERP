

using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Payrolls.Reports.DTOs
{
    public class SalaryComponentReportDto
    {
        public int SalaryComponentId { get; set; }

        public string ComponentName { get; set; } = null!;

        public SalaryComponentType Type { get; set; }

        public int EmployeesCount { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
