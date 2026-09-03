using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Payrolls.Adjustments.DTOs;

public class PayrollAdjustmentFilterDto
{
    public int? EmployeeId { get; set; }

    public int? PayrollPeriodId { get; set; }

    public AdjustmentType? Type { get; set; }

    public bool? IsApplied { get; set; }
}