using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Positions.DTOs;

public class UpdatePositionDto
{
    public string? Code { get; set; }
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? Description { get; set; }
    public PositionAssignmentType? AssignmentType { get; set; }
}
