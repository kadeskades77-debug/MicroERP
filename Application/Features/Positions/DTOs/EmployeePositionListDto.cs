namespace MicroERP.Application.Features.Positions.DTOs;

public class EmployeePositionListDto
{
    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; } = null!;

    public int PositionId { get; set; }

    public string PositionName { get; set; } = null!;
}