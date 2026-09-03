using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Payrolls.Adjustments.DTOs;

public class CreatePayrollAdjustmentDto
{
    public int EmployeeId { get; set; }

    public int PayrollPeriodId { get; set; }
    public int? SalaryComponentId { get; set; }

    public AdjustmentType Type { get; set; }

    public string Title { get; set; } = null!;

    public string? Notes { get; set; }

    public decimal Amount { get; set; }
}