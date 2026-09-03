using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Payrolls.Adjustments.DTOs;

public class PayrollAdjustmentDto
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; } = null!;

    public int? PayrollPeriodId { get; set; }

    public string? Period { get; set; }

    public AdjustmentType Type { get; set; }

    public string Title { get; set; } = null!;

    public string? Notes { get; set; }

    public decimal Amount { get; set; }

    public bool IsApplied { get; set; }

    public DateTime CreatedOn { get; set; }
}