using MicroERP.Domin.Entities.EmployeeEvaluations;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Persistence.Seed;

public static class EmployeeEvaluationSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context)
    {
        // =========================================================
        // Template
        // =========================================================

        var template =
            await context.EvaluationTemplates
                .Include(x => x.Criteria)
                .FirstOrDefaultAsync();

        if (template == null)
            return;

        if (!template.IsActive)
            return;

        if (template.Criteria == null ||
            template.Criteria.Count == 0)
            return;


        // =========================================================
        // Attendance Criterion
        // =========================================================

        var attendanceCriterion =
            template.Criteria
                .FirstOrDefault(x =>
                    x.Source ==
                    EvaluationCriterionSource.AttendancePerformance);

        if (attendanceCriterion == null)
            return;


        // =========================================================
        // Employees
        // =========================================================

        var employees =
            await context.Employees
                .AsNoTracking()
                .Select(x => x.Id)
                .ToListAsync();

        if (employees.Count == 0)
            return;


        // =========================================================
        // Evaluation Periods
        // =========================================================

        var periods =
            await context.EvaluationPeriods
                .AsNoTracking()
                .Where(x =>
                    x.StartDate.Day == 1&& x.Status== EvaluationPeriodStatus.Open)
                .OrderBy(x => x.StartDate)
                .ToListAsync();

        if (periods.Count == 0)
            return;


        // =========================================================
        // Attendance Performance
        // =========================================================

        var attendancePerformances =
            await context.AttendancePerformances
                .AsNoTracking()
                .ToListAsync();


        // =========================================================
        // Evaluator
        // =========================================================
        //
        // EvaluatorId هو string مربوط بالمستخدم.
        // نستخدم أول User موجود في النظام للسيدر.
        // =========================================================

        var evaluatorId =
            await context.Users
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

        if (string.IsNullOrWhiteSpace(evaluatorId))
            return;


        // =========================================================
        // Existing Evaluations
        // =========================================================

        var existingEvaluations =
            await context.EmployeeEvaluations
                .AsNoTracking()
                .Select(x => new
                {
                    x.EmployeeId,
                    x.PeriodId
                })
                .ToListAsync();


        var existingKeys =
            existingEvaluations
                .Select(x =>
                    (x.EmployeeId, x.PeriodId))
                .ToHashSet();


        // =========================================================
        // Random
        // =========================================================

        var random =
            new Random();


        var evaluations =
            new List<EmployeeEvaluation>();


        // =========================================================
        // Create Evaluations
        // =========================================================

        foreach (var period in periods)
        {
            var year =
                period.StartDate.Year;

            var month =
                period.StartDate.Month;


            foreach (var employeeId in employees)
            {
                // -------------------------------------------------
                // Prevent Duplicate
                // -------------------------------------------------

                if (existingKeys.Contains(
                    (employeeId, period.Id)))
                {
                    continue;
                }


                // -------------------------------------------------
                // Attendance Performance
                // -------------------------------------------------

                var attendance =
                    attendancePerformances
                        .FirstOrDefault(x =>
                            x.EmployeeId == employeeId &&
                            x.Year == year &&
                            x.Month == month);


                // -------------------------------------------------
                // If Attendance Performance does not exist
                // -------------------------------------------------

                if (attendance == null)
                    continue;


                // -------------------------------------------------
                // Evaluation
                // -------------------------------------------------

                var evaluation =
                    new EmployeeEvaluation
                    {
                        EmployeeId =
                            employeeId,

                        EvaluatorId =
                            evaluatorId,

                        PeriodId =
                            period.Id,

                        TemplateId =
                            template.Id,

                        TotalScore = 0,

                        FinalRate =
                            FinalRate.Poor,

                        Status =
                            EmployeeEvaluationStatus.Approved,

                        ApprovedBy =
                            evaluatorId,

                        ApprovedOn =
                            DateTime.UtcNow
                    };


                // -------------------------------------------------
                // Criteria
                // -------------------------------------------------

                foreach (var criterion in template.Criteria)
                {
                    decimal score;


                    // =============================================
                    // Attendance Criterion
                    // =============================================

                    if (criterion.Source ==
                        EvaluationCriterionSource.AttendancePerformance)
                    {
                        score =
                            attendance.AttendanceScore;
                    }


                    // =============================================
                    // Manual Criteria
                    // =============================================

                    else
                    {
                        score =
                            random.Next(
                                75,
                                101);
                    }


                    // =============================================
                    // Safety
                    // =============================================

                    if (score < 0)
                        score = 0;

                    if (score > criterion.MaxScore)
                        score = criterion.MaxScore;


                    // =============================================
                    // Add Item
                    // =============================================

                    evaluation.Items.Add(
                        new EmployeeEvaluationItem
                        {
                            CriterionId =
                                criterion.Id,

                            Score =
                                score,

                            Notes =
                                GetEvaluationMessage(
                                    criterion.Name,
                                    score)
                        });
                }


                // -------------------------------------------------
                // Calculate Total Score
                // -------------------------------------------------

                evaluation.TotalScore =
                    CalculateTotalScore(
                        evaluation.Items,
                        template.Criteria);


                // -------------------------------------------------
                // Final Rate
                // -------------------------------------------------

                evaluation.FinalRate =
                    CalculateFinalRate(
                        evaluation.TotalScore);


                evaluations.Add(
                    evaluation);

                existingKeys.Add(
                    (employeeId, period.Id));
            }
        }


        // =========================================================
        // Nothing To Add
        // =========================================================

        if (evaluations.Count == 0)
            return;


        // =========================================================
        // Save
        // =========================================================

        await context.EmployeeEvaluations
            .AddRangeAsync(evaluations);

        await context.SaveChangesAsync();
    }


    // =============================================================
    // Calculate Weighted Total Score
    // =============================================================

    private static decimal CalculateTotalScore(
     IEnumerable<EmployeeEvaluationItem> items,
     IEnumerable<EvaluationCriterion> criteria)
    {
        var criteriaDictionary =
            criteria.ToDictionary(x => x.Id);

        decimal total = 0;

        foreach (var item in items)
        {
            if (!criteriaDictionary.TryGetValue(
                    item.CriterionId,
                    out var criterion))
            {
                continue;
            }

            if (criterion.MaxScore <= 0)
            {
                throw new InvalidOperationException(
                    $"Criterion '{criterion.Name}' has an invalid MaxScore.");
            }

            if (item.Score < 0 ||
                item.Score > criterion.MaxScore)
            {
                throw new InvalidOperationException(
                    $"Score for criterion '{criterion.Name}' " +
                    $"must be between 0 and {criterion.MaxScore}.");
            }

            if (criterion.Weight < 0)
            {
                throw new InvalidOperationException(
                    $"Weight for criterion '{criterion.Name}' cannot be negative.");
            }

            total +=
                item.Score *
                criterion.Weight /
                criterion.MaxScore;
        }

        return Math.Round(
            total,
            2);
    }


    // =============================================================
    // Final Rate
    // =============================================================

    private static FinalRate CalculateFinalRate(
        decimal score)
    {
        return score switch
        {
            >= 90 =>
                FinalRate.Excellent,

            >= 80 =>
                FinalRate.VeryGood,

            >= 70 =>
                FinalRate.Good,

            >= 60 =>
                FinalRate.Acceptable,

            _ =>
                FinalRate.Poor
        };
    }


    // =============================================================
    // Evaluation Message
    // =============================================================

    private static string GetEvaluationMessage(
        string criterionName,
        decimal score)
    {
        if (score < 0 ||
            score > 100)
        {
            return string.Empty;
        }


        return criterionName switch
        {
            "Quality of Work" => score switch
            {
                < 60 => "Poor quality of work.",
                < 70 => "Acceptable quality of work.",
                < 80 => "Good quality of work.",
                < 90 => "Very good quality of work.",
                _ => "Excellent quality of work."
            },

            "Commitment and Discipline" => score switch
            {
                < 60 => "Poor commitment and discipline.",
                < 70 => "Acceptable commitment and discipline.",
                < 80 => "Good commitment and discipline.",
                < 90 => "Very good commitment and discipline.",
                _ => "Excellent commitment and discipline."
            },

            "Productivity" => score switch
            {
                < 60 => "Poor productivity.",
                < 70 => "Acceptable productivity.",
                < 80 => "Good productivity.",
                < 90 => "Very good productivity.",
                _ => "Excellent productivity."
            },

            "Teamwork and Cooperation" => score switch
            {
                < 60 => "Poor teamwork and cooperation.",
                < 70 => "Acceptable teamwork and cooperation.",
                < 80 => "Good teamwork and cooperation.",
                < 90 => "Very good teamwork and cooperation.",
                _ => "Excellent teamwork and cooperation."
            },

            "Initiative and Responsibility" => score switch
            {
                < 60 => "Poor initiative and responsibility.",
                < 70 => "Acceptable initiative and responsibility.",
                < 80 => "Good initiative and responsibility.",
                < 90 => "Very good initiative and responsibility.",
                _ => "Excellent initiative and responsibility."
            },

            _ => string.Empty
        };
    }
}