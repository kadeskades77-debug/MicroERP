using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Payrolls.Adjustments.DTOs;

public class UpdatePayrollAdjustmentDto
{
    public AdjustmentType? Type { get; set; }

    public string? Title { get; set; }

    public decimal? Amount { get; set; }

    public string? Notes { get; set; }
}