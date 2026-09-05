using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Positions.DTOs;

public class PositionDto
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string NameAr { get; set; } = null!;

    public string NameEn { get; set; } = null!;

    public string? Description { get; set; }

    public PositionAssignmentType AssignmentType { get; set; }

    public bool IsActive { get; set; }
}
