using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Leaves.EmployeeSpecialLeaves.DTOs;

public class SpecialLeaveReportFilterDto
{
    public int? EmployeeId { get; set; }

    public int? DepartmentId { get; set; }

    public SpecialLeaveType? Type { get; set; }

    public SpecialLeaveStatus? Status { get; set; }

    public int? Year { get; set; }

    public DateOnly? FromDate { get; set; }

    public DateOnly? ToDate { get; set; }
}