namespace MicroERP.Application.Features.Positions.DTOs;

public class PositionListDto
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string NameAr { get; set; } = null!;
    public string NameEn { get; set; } = null!;
    public bool IsActive { get; set; }
}
