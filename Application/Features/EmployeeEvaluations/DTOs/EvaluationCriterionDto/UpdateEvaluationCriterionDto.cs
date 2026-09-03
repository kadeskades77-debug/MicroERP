namespace MicroERP.Application.Features.EmployeeEvaluations;

public class UpdateEvaluationCriterionDto
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public decimal? MaxScore { get; set; }

    public decimal? Weight { get; set; }

    public int? SortOrder { get; set; }
}
