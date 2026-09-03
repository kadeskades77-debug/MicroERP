

using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Payrolls.Reports.DTOs
{
    public class AdjustmentReportDto
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; } = null!;

        public string Department { get; set; } = null!;

        public int Year { get; set; }

        public int Month { get; set; }

        public int? SalaryComponentId { get; set; }

        public string? SalaryComponentName { get; set; }

        public AdjustmentType Type { get; set; }

        public string Title { get; set; } = null!;

        public decimal Amount { get; set; }

        public string? Notes { get; set; }

        public bool IsApplied { get; set; }
    }
}
