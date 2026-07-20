using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeSpecialLeaves.DTOs;

public class CreateSpecialLeaveDto
{
    public SpecialLeaveType Type { get; set; }

    public DateOnly StartDate { get; set; }

    public string? Reason { get; set; }

}