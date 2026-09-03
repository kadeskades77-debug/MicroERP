using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Payrolls.DTOs;

public class PayrollFilterDto
{
    public int? PeriodId { get; set; }

    public int? EmployeeId { get; set; }

    public PayrollStatus? Status { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}