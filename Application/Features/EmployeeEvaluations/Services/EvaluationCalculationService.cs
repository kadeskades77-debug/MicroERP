using MicroERP.Application.Features.EmployeeEvaluations.Interfaces;
using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeEvaluations.Services;

public class EvaluationCalculationService : IEvaluationCalculationService
{
    public decimal CalculateTotalScore(
        IEnumerable<EmployeeEvaluationItemDto> items)
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items));

        decimal total = 0;

        foreach (var item in items)
        {
            if (item.MaxScore <= 0)
            {
                throw new InvalidOperationException(
                    $"Criterion '{item.CriterionName}' has an invalid MaxScore.");
            }

            if (item.Score < 0 || item.Score > item.MaxScore)
            {
                throw new InvalidOperationException(
                    $"Score for criterion '{item.CriterionName}' " +
                    $"must be between 0 and {item.MaxScore}.");
            }

            if (item.Weight < 0)
            {
                throw new InvalidOperationException(
                    $"Weight for criterion '{item.CriterionName}' cannot be negative.");
            }

            total += item.Score * item.Weight / item.MaxScore;
        }

        return Math.Round(total, 2);
    }

    public FinalRate CalculateFinalRate(decimal score)
    {
        if (score < 0)
            throw new ArgumentOutOfRangeException(
                nameof(score),
                "Score cannot be negative.");

        return score switch
        {
            < 60 => FinalRate.Poor,
            < 70 => FinalRate.Acceptable,
            < 80 => FinalRate.Good,
            < 90 => FinalRate.VeryGood,
            _ => FinalRate.Excellent
        };
    }
}