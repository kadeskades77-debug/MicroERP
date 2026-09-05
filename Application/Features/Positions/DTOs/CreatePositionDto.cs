using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Positions.DTOs;

public class CreatePositionDto
{
    public string Code { get; set; } = null!;
    public string NameAr { get; set; } = null!;
    public string NameEn { get; set; } = null!;
    public PositionAssignmentType AssignmentType { get; set; }
    public string? Description { get; set; }
}
