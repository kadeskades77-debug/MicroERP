using AutoMapper;
using DocumentFormat.OpenXml.ExtendedProperties;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using MicroERP.Application.Features.EmployeeEvaluations.Interfaces;
using MicroERP.Domin.Entities.EmployeeEvaluations;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.EmployeeEvaluations.Services;

public class EmployeeEvaluationService : IEmployeeEvaluationService
{
    private readonly IApplicationDbContext _context;
    private readonly IEvaluationCalculationService _evaluationCalculationService;
    private readonly IAuditService _auditService;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;
    private readonly IAttendancePerformanceQueries _attendancePerformanceQueries;
    public EmployeeEvaluationService(IApplicationDbContext context, IAuditService auditService, IMapper mapper, ICurrentUserService currentUser, IAttendancePerformanceQueries attendancePerformanceQueries, IEvaluationCalculationService evaluationCalculationService)
    {
        _context = context;
        _auditService = auditService;
        _mapper = mapper;
        _currentUser = currentUser;
        _attendancePerformanceQueries = attendancePerformanceQueries;
        _evaluationCalculationService = evaluationCalculationService;
    }


    public async Task<Result<EmployeeEvaluationDto>> CreateAsync(
      CreateEmployeeEvaluationDto dto,
      CancellationToken ct = default)
    {
        // =========================================================
        // Validation
        // =========================================================

        if (dto == null)
        {
            return Result<EmployeeEvaluationDto>.Failure(
                "Evaluation data is required.");
        }

        if (dto.Items == null || dto.Items.Count == 0)
        {
            return Result<EmployeeEvaluationDto>.Failure(
                "At least one evaluation item is required.");
        }


        // =========================================================
        // Current Evaluator
        // =========================================================

        var evaluatorId = _currentUser.UserId;

        if (string.IsNullOrWhiteSpace(evaluatorId))
        {
            return Result<EmployeeEvaluationDto>.Failure(
                "Authenticated evaluator not found.");
        }


        // =========================================================
        // Employee
        // =========================================================

        var employeeExists =
            await _context.Employees
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == dto.EmployeeId,
                    ct);

        if (!employeeExists)
        {
            return Result<EmployeeEvaluationDto>.Failure(
                "Employee not found.");
        }


        // =========================================================
        // Evaluation Period
        // =========================================================

        var period =
            await _context.EvaluationPeriods
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == dto.PeriodId,
                    ct);

        if (period == null)
        {
            return Result<EmployeeEvaluationDto>.Failure(
                "Evaluation period not found.");
        }


        if (period.Status != EvaluationPeriodStatus.Open)
        {
            return Result<EmployeeEvaluationDto>.Failure(
                "Evaluation period is not open.");
        }


        // =========================================================
        // Evaluation Template
        // =========================================================

        var template =
            await _context.EvaluationTemplates
                .AsNoTracking()
                .Include(x => x.Criteria)
                .FirstOrDefaultAsync(
                    x => x.Id == dto.TemplateId,
                    ct);

        if (template == null)
        {
            return Result<EmployeeEvaluationDto>.Failure(
                "Evaluation template not found.");
        }


        if (!template.IsActive)
        {
            return Result<EmployeeEvaluationDto>.Failure(
                "Evaluation template is not active.");
        }


        if (template.Criteria.Count == 0)
        {
            return Result<EmployeeEvaluationDto>.Failure(
                "Evaluation template has no criteria.");
        }


        // =========================================================
        // Duplicate Evaluation
        // =========================================================

        var evaluationExists =
            await _context.EmployeeEvaluations
                .AnyAsync(
                    x =>
                        x.EmployeeId == dto.EmployeeId &&
                        x.PeriodId == dto.PeriodId,
                    ct);

        if (evaluationExists)
        {
            return Result<EmployeeEvaluationDto>.Failure(
                "An evaluation already exists for this employee in this period.");
        }


        // =========================================================
        // Template Criteria
        // =========================================================

        var templateCriteria =
            template.Criteria
                .ToDictionary(x => x.Id);


        // =========================================================
        // Validate Duplicate Criteria
        // =========================================================

        var criterionIds =
            dto.Items
                .Select(x => x.CriterionId)
                .ToList();

        if (criterionIds.Count != criterionIds.Distinct().Count())
        {
            return Result<EmployeeEvaluationDto>.Failure(
                "A criterion cannot be added more than once.");
        }


        // =========================================================
        // Validate Criteria Belong To Template
        // =========================================================

        var invalidCriterion =
            dto.Items
                .FirstOrDefault(
                    x => !templateCriteria.ContainsKey(x.CriterionId));

        if (invalidCriterion != null)
        {
            return Result<EmployeeEvaluationDto>.Failure(
                $"Criterion {invalidCriterion.CriterionId} does not belong to the selected template.");
        }


        // =========================================================
        // Create Evaluation
        // =========================================================

        var evaluation =
            new EmployeeEvaluation
            {
                EmployeeId = dto.EmployeeId,

                EvaluatorId = evaluatorId,

                PeriodId = dto.PeriodId,

                TemplateId = dto.TemplateId,

                TotalScore = 0,

                FinalRate = FinalRate.Poor,

                Status = EmployeeEvaluationStatus.Draft
            };


        // =========================================================
        // Create Manual Criteria
        // =========================================================

        foreach (var itemDto in dto.Items)
        {
            var criterion =
                templateCriteria[itemDto.CriterionId];


            // -----------------------------------------------------
            // Attendance Criterion
            // -----------------------------------------------------

            if (criterion.Source ==
                EvaluationCriterionSource.AttendancePerformance)
            {
                return Result<EmployeeEvaluationDto>.Failure(
                    $"Criterion '{criterion.Name}' is calculated automatically and its score cannot be entered manually.");
            }


            // -----------------------------------------------------
            // Manual Score Validation
            // -----------------------------------------------------

            if (itemDto.Score < 0)
            {
                return Result<EmployeeEvaluationDto>.Failure(
                    $"Score for criterion '{criterion.Name}' cannot be negative.");
            }


            if (itemDto.Score > criterion.MaxScore)
            {
                return Result<EmployeeEvaluationDto>.Failure(
                    $"Score for criterion '{criterion.Name}' cannot exceed {criterion.MaxScore}.");
            }


            // -----------------------------------------------------
            // Add Manual Item
            // -----------------------------------------------------

            evaluation.Items.Add(
                new EmployeeEvaluationItem
                {
                    CriterionId = criterion.Id,

                    Score = itemDto.Score,

                    Notes = GetEvaluationMessage(criterion.Name, itemDto.Score)
                });
        }


        // =========================================================
        // Attendance Performance Criteria
        // =========================================================

        var attendanceCriteria =
            template.Criteria
                .Where(x =>
                    x.Source ==
                    EvaluationCriterionSource.AttendancePerformance)
                .ToList();


        if (attendanceCriteria.Count > 0)
        {
            var year = period.StartDate.Year;
            var month = period.StartDate.Month;


            var attendanceResult =
                await _attendancePerformanceQueries
                    .GetByEmployeeAndMonthAsync(
                        dto.EmployeeId,
                        year,
                        month,
                        ct);


            if (!attendanceResult.Success)
            {
                return Result<EmployeeEvaluationDto>.Failure(
                    attendanceResult.Message
                    ?? "Attendance performance not found.");
            }


            if (attendanceResult.Data == null)
            {
                return Result<EmployeeEvaluationDto>.Failure(
                    "Attendance performance data not found.");
            }

            var attendanceScore =
                attendanceResult.Data.AttendanceScore;



            foreach (var criterion in attendanceCriteria)
            {
                if (attendanceScore < 0)
                {
                    return Result<EmployeeEvaluationDto>.Failure(
                        $"Attendance score for criterion '{criterion.Name}' cannot be negative.");
                }


                if (attendanceScore > criterion.MaxScore)
                {
                    return Result<EmployeeEvaluationDto>.Failure(
                        $"Attendance score for criterion '{criterion.Name}' cannot exceed {criterion.MaxScore}.");
                }


                evaluation.Items.Add(
                    new EmployeeEvaluationItem
                    {
                        CriterionId = criterion.Id,

                        Score = attendanceScore,

                        Notes = GetEvaluationMessage(criterion.Name, attendanceScore)
                    });
            }
        }


        // =========================================================
        // Save
        // =========================================================

        _context.EmployeeEvaluations.Add(evaluation);

        await _context.SaveChangesAsync(ct);


        // =========================================================
        // Audit
        // =========================================================

        var newValues = new
        {
            evaluation.EmployeeId,
            evaluation.EvaluatorId,
            evaluation.PeriodId,
            evaluation.TemplateId,
            evaluation.Status,

            Items = evaluation.Items
                .Select(x => new
                {
                    x.CriterionId,
                    x.Score,
                    x.Notes
                })
                .ToList()
        };


        await _auditService.LogAsync(
            "Create",
            "EmployeeEvaluation",
            evaluation.Id.ToString(),
            null,
            newValues);


        // =========================================================
        // Reload
        // =========================================================

        var result =
            await _context.EmployeeEvaluations
                .AsNoTracking()
                .Include(x => x.Employee)
                    .ThenInclude(x => x.User)
                .Include(x => x.Evaluator)
                .Include(x => x.Period)
                .Include(x => x.Template)
                .Include(x => x.Items)
                    .ThenInclude(x => x.Criterion)
                .FirstAsync(
                    x => x.Id == evaluation.Id,
                    ct);


        // =========================================================
        // Return
        // =========================================================

        return Result<EmployeeEvaluationDto>.Succeeded(
            _mapper.Map<EmployeeEvaluationDto>(result));
    }


    public async Task<Result<EmployeeEvaluationDto>> UpdateAsync(
      int id,
      UpdateEmployeeEvaluationDto dto,
      CancellationToken ct = default)
    {
        // =========================================================
        // Validation
        // =========================================================

        if (dto == null)
        {
            return Result<EmployeeEvaluationDto>.Failure(
                "Evaluation data is required.");
        }

        if (dto.Items == null || dto.Items.Count == 0)
        {
            return Result<EmployeeEvaluationDto>.Failure(
                "At least one evaluation item is required.");
        }


        // =========================================================
        // Current User
        // =========================================================

        var evaluatorId = _currentUser.UserId;

        if (string.IsNullOrWhiteSpace(evaluatorId))
        {
            return Result<EmployeeEvaluationDto>.Failure(
                "Authenticated evaluator not found.");
        }


        // =========================================================
        // Get Evaluation
        // =========================================================

        var evaluation =
            await _context.EmployeeEvaluations
                .Include(x => x.Items)
                .Include(x => x.Template)
                    .ThenInclude(x => x.Criteria)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    ct);

        if (evaluation == null)
        {
            return Result<EmployeeEvaluationDto>.Failure(
                "Employee evaluation not found.");
        }


        // =========================================================
        // Ownership
        // =========================================================

        if (evaluation.EvaluatorId != evaluatorId)
        {
            return Result<EmployeeEvaluationDto>.Failure(
                "You are not allowed to modify this evaluation.");
        }


        // =========================================================
        // Validate Status
        // =========================================================

        if (evaluation.Status == EmployeeEvaluationStatus.Submitted)
        {
            return Result<EmployeeEvaluationDto>.Failure(
                "Submitted evaluation cannot be modified.");
        }

        if (evaluation.Status == EmployeeEvaluationStatus.Approved)
        {
            return Result<EmployeeEvaluationDto>.Failure(
                "Approved evaluation cannot be modified.");
        }


        // =========================================================
        // Validate Template
        // =========================================================

        if (evaluation.Template == null)
        {
            return Result<EmployeeEvaluationDto>.Failure(
                "Evaluation template not found.");
        }


        var templateCriteria =
            evaluation.Template.Criteria
                .ToDictionary(x => x.Id);


        // =========================================================
        // Duplicate Criteria In Request
        // =========================================================

        var criterionIds =
            dto.Items
                .Select(x => x.CriterionId)
                .ToList();

        if (criterionIds.Count != criterionIds.Distinct().Count())
        {
            return Result<EmployeeEvaluationDto>.Failure(
                "A criterion cannot be updated more than once.");
        }


        // =========================================================
        // Validate Criteria
        // =========================================================

        foreach (var itemDto in dto.Items)
        {
            if (!templateCriteria.TryGetValue(
                    itemDto.CriterionId,
                    out var criterion))
            {
                return Result<EmployeeEvaluationDto>.Failure(
                    $"Criterion {itemDto.CriterionId} does not belong to the evaluation template.");
            }


            // -----------------------------------------------------
            // Attendance Performance Criterion
            // -----------------------------------------------------

            if (criterion.Source ==
                EvaluationCriterionSource.AttendancePerformance)
            {
                return Result<EmployeeEvaluationDto>.Failure(
                    $"Criterion '{criterion.Name}' is calculated automatically and cannot be modified manually.");
            }


            // -----------------------------------------------------
            // Score Validation
            // -----------------------------------------------------

            if (itemDto.Score < 0)
            {
                return Result<EmployeeEvaluationDto>.Failure(
                    $"Score for criterion '{criterion.Name}' cannot be negative.");
            }


            if (itemDto.Score > criterion.MaxScore)
            {
                return Result<EmployeeEvaluationDto>.Failure(
                    $"Score for criterion '{criterion.Name}' cannot exceed {criterion.MaxScore}.");
            }
        }


        // =========================================================
        // Old Values
        // =========================================================

        var oldValues = new
        {
            evaluation.Status,

            Items = evaluation.Items
                .OrderBy(x => x.CriterionId)
                .Select(x => new
                {
                    x.CriterionId,
                    CriterionName =
                        x.Criterion?.Name,
                    x.Score,
                    x.Notes
                })
                .ToList()
        };


        // =========================================================
        // Partial Update
        // =========================================================

        foreach (var itemDto in dto.Items)
        {
            var item =
                evaluation.Items
                    .FirstOrDefault(
                        x => x.CriterionId == itemDto.CriterionId);

            if (item == null)
            {
                return Result<EmployeeEvaluationDto>.Failure(
                    $"Evaluation item for criterion {itemDto.CriterionId} was not found.");
            }


            // -----------------------------------------------------
            // Update Score
            // -----------------------------------------------------

            item.Score =
                itemDto.Score;


            // -----------------------------------------------------
            // Update Notes
            // -----------------------------------------------------

            item.Notes =
               GetEvaluationMessage(item.Criterion.Name, itemDto.Score);
        }




        // =========================================================
        // Save
        // =========================================================

        await _context.SaveChangesAsync(ct);


        // =========================================================
        // New Values
        // =========================================================

        var newValues = new
        {
            evaluation.Status,

            Items = evaluation.Items
                .OrderBy(x => x.CriterionId)
                .Select(x => new
                {
                    x.CriterionId,
                    CriterionName =
                        x.Criterion?.Name,
                    x.Score,
                    x.Notes
                })
                .ToList()
        };


        // =========================================================
        // Audit
        // =========================================================

        await _auditService.LogAsync(
            "Update",
            "EmployeeEvaluation",
            evaluation.Id.ToString(),
            oldValues,
            newValues);


        // =========================================================
        // Reload
        // =========================================================

        var result =
            await _context.EmployeeEvaluations
                .AsNoTracking()
                .Include(x => x.Employee)
                    .ThenInclude(x => x.User)
                .Include(x => x.Evaluator)
                .Include(x => x.Period)
                .Include(x => x.Template)
                .Include(x => x.Items)
                    .ThenInclude(x => x.Criterion)
                .FirstAsync(
                    x => x.Id == id,
                    ct);


        // =========================================================
        // Return
        // =========================================================

        return Result<EmployeeEvaluationDto>.Succeeded(
            _mapper.Map<EmployeeEvaluationDto>(result));
    }

    public async Task<Result<bool>> SubmitAsync(int id,
     CancellationToken ct = default)
    {
        // =========================================================
        // Current User
        // =========================================================

        var userId = _currentUser.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result<bool>.Failure(
                "Authenticated user not found.");
        }


        // =========================================================
        // Get Evaluation
        // =========================================================

        var evaluation =
      await _context.EmployeeEvaluations
          .Include(x => x.Items)
              .ThenInclude(x => x.Criterion)
          .Include(x => x.Period)
          .Include(x => x.Template)
              .ThenInclude(x => x.Criteria)
          .FirstOrDefaultAsync(
              x => x.Id == id,
              ct);

        if (evaluation == null)
        {
            return Result<bool>.Failure(
                "Employee evaluation not found.");
        }


        // =========================================================
        // Ownership
        // =========================================================

        if (evaluation.EvaluatorId != userId)
        {
            return Result<bool>.Failure(
                "You are not allowed to submit this evaluation.");
        }


        // =========================================================
        // Validate Status
        // =========================================================

        if (evaluation.Status != EmployeeEvaluationStatus.Draft &&
            evaluation.Status != EmployeeEvaluationStatus.InProgress &&
            evaluation.Status != EmployeeEvaluationStatus.Rejected)
        {
            return Result<bool>.Failure(
                "Only draft, in-progress, or rejected evaluations can be submitted.");
        }


        // =========================================================
        // Validate Period
        // =========================================================

        if (evaluation.Period == null)
        {
            return Result<bool>.Failure(
                "Evaluation period not found.");
        }

        if (evaluation.Period.Status != EvaluationPeriodStatus.Open)
        {
            return Result<bool>.Failure(
                "Evaluation period is not open.");
        }


        // =========================================================
        // Validate All Criteria
        // =========================================================

        var criteria =
            evaluation.Template?
                .Criteria;

        if (criteria == null || criteria.Count == 0)
        {
            return Result<bool>.Failure(
                "Evaluation template has no criteria.");
        }


        var criterionIds =
            criteria
                .Select(x => x.Id)
                .ToHashSet();

        var evaluationCriterionIds =
            evaluation.Items
                .Select(x => x.CriterionId)
                .ToHashSet();


        var missingCriteria =
            criterionIds
                .Except(evaluationCriterionIds)
                .ToList();

        if (missingCriteria.Count > 0)
        {
            return Result<bool>.Failure(
                "All evaluation criteria must be completed before submission.");
        }


        // =========================================================
        // Validate Scores
        // =========================================================

        foreach (var item in evaluation.Items)
        {
            if (item.Score < 0)
            {
                return Result<bool>.Failure(
                    $"Score for criterion '{item.Criterion.Name}' cannot be negative.");
            }

            if (item.Score > item.Criterion.MaxScore)
            {
                return Result<bool>.Failure(
                    $"Score for criterion '{item.Criterion.Name}' cannot exceed {item.Criterion.MaxScore}.");
            }
        }


        // =========================================================
        // Calculate Total Score
        // =========================================================

        var itemDtos =
            evaluation.Items
                .Select(x => new EmployeeEvaluationItemDto
                {
                    Id = x.Id,
                    CriterionId = x.CriterionId,
                    CriterionName = x.Criterion.Name,
                    MaxScore = x.Criterion.MaxScore,
                    Weight = x.Criterion.Weight,
                    Score = x.Score,
                    Notes = x.Notes
                })
                .ToList();


        var totalScore =
            _evaluationCalculationService
                .CalculateTotalScore(itemDtos);


        // =========================================================
        // Calculate Final Rate
        // =========================================================

        var finalRate =
            _evaluationCalculationService
                .CalculateFinalRate(totalScore);


        // =========================================================
        // Old Values
        // =========================================================

        var oldValues = new
        {
            evaluation.Status,
            evaluation.TotalScore,
            evaluation.FinalRate
        };


        // =========================================================
        // Submit
        // =========================================================

        evaluation.TotalScore =
            totalScore;

        evaluation.FinalRate =
            finalRate;

        evaluation.Status =
            EmployeeEvaluationStatus.Submitted;


        // =========================================================
        // Clear Previous Rejection
        // =========================================================

        evaluation.RejectedBy = null;
        evaluation.RejectedOn = null;
        evaluation.RejectionReason = null;


        // =========================================================
        // Save
        // =========================================================

        await _context.SaveChangesAsync(ct);


        // =========================================================
        // Audit
        // =========================================================

        var newValues = new
        {
            evaluation.Status,
            evaluation.TotalScore,
            evaluation.FinalRate
        };


        await _auditService.LogAsync(
            "Submit",
            "EmployeeEvaluation",
            evaluation.Id.ToString(),
            oldValues,
            newValues);


        // =========================================================
        // Return
        // =========================================================

        return Result<bool>.Succeeded(
            true,
            "Employee evaluation submitted successfully.");
    }

    public async Task<Result<bool>> ApproveAsync(int id,
      CancellationToken ct = default)
    {
        // =========================================================
        // Current User
        // =========================================================

        var userId = _currentUser.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result<bool>.Failure(
                "Authenticated user not found.");
        }


        // =========================================================
        // Get Evaluation
        // =========================================================

        var evaluation =
            await _context.EmployeeEvaluations
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    ct);

        if (evaluation == null)
        {
            return Result<bool>.Failure(
                "Employee evaluation not found.");
        }


        // =========================================================
        // Validate Status
        // =========================================================

        if (evaluation.Status != EmployeeEvaluationStatus.Submitted)
        {
            return Result<bool>.Failure(
                "Only submitted evaluations can be approved.");
        }


        // =========================================================
        // Old Values
        // =========================================================

        var oldValues = new
        {
            evaluation.Status,
            evaluation.ApprovedBy,
            evaluation.ApprovedOn,
            evaluation.RejectedBy,
            evaluation.RejectedOn,
            evaluation.RejectionReason
        };


        // =========================================================
        // Approve Evaluation
        // =========================================================

        evaluation.Status =
            EmployeeEvaluationStatus.Approved;

        evaluation.ApprovedBy =
            userId;

        evaluation.ApprovedOn =
            DateTime.UtcNow;


        // =========================================================
        // Clear Previous Rejection
        // =========================================================

        evaluation.RejectedBy = null;
        evaluation.RejectedOn = null;
        evaluation.RejectionReason = null;


        // =========================================================
        // Save
        // =========================================================

        await _context.SaveChangesAsync(ct);


        // =========================================================
        // Audit
        // =========================================================

        var newValues = new
        {
            evaluation.Status,
            evaluation.ApprovedBy,
            evaluation.ApprovedOn,
            evaluation.RejectedBy,
            evaluation.RejectedOn,
            evaluation.RejectionReason
        };


        await _auditService.LogAsync(
            "Approve",
            "EmployeeEvaluation",
            evaluation.Id.ToString(),
            oldValues,
            newValues);


        // =========================================================
        // Return
        // =========================================================

        return Result<bool>.Succeeded(
            true,
            "Employee evaluation approved successfully.");
    }

    public async Task<Result<bool>> RejectAsync(int id,string? reason = null,
      CancellationToken ct = default)
    {
        // =========================================================
        // Current User
        // =========================================================

        var userId = _currentUser.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result<bool>.Failure(
                "Authenticated user not found.");
        }


        // =========================================================
        // Validate Rejection Reason
        // =========================================================

        if (string.IsNullOrWhiteSpace(reason))
        {
            return Result<bool>.Failure(
                "Rejection reason is required.");
        }


        // =========================================================
        // Get Evaluation
        // =========================================================

        var evaluation =
            await _context.EmployeeEvaluations
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    ct);

        if (evaluation == null)
        {
            return Result<bool>.Failure(
                "Employee evaluation not found.");
        }


        // =========================================================
        // Validate Status
        // =========================================================

        if (evaluation.Status != EmployeeEvaluationStatus.Submitted)
        {
            return Result<bool>.Failure(
                "Only submitted evaluations can be rejected.");
        }


        // =========================================================
        // Old Values
        // =========================================================

        var oldValues = new
        {
            evaluation.Status,
            evaluation.RejectedBy,
            evaluation.RejectedOn,
            evaluation.RejectionReason,
            evaluation.ApprovedBy,
            evaluation.ApprovedOn
        };


        // =========================================================
        // Reject Evaluation
        // =========================================================

        evaluation.Status =
            EmployeeEvaluationStatus.Rejected;

        evaluation.RejectedBy =
            userId;

        evaluation.RejectedOn =
            DateTime.UtcNow;

        evaluation.RejectionReason =
            reason.Trim();

        // Clear previous approval data
        evaluation.ApprovedBy = null;
        evaluation.ApprovedOn = null;


        // =========================================================
        // Save
        // =========================================================

        await _context.SaveChangesAsync(ct);


        // =========================================================
        // Audit
        // =========================================================

        var newValues = new
        {
            evaluation.Status,
            evaluation.RejectedBy,
            evaluation.RejectedOn,
            evaluation.RejectionReason,
            evaluation.ApprovedBy,
            evaluation.ApprovedOn
        };


        await _auditService.LogAsync(
            "Reject",
            "EmployeeEvaluation",
            evaluation.Id.ToString(),
            oldValues,
            newValues);


        // =========================================================
        // Return
        // =========================================================

        return Result<bool>.Succeeded(
            true,
            "Employee evaluation rejected successfully.");
    }

    public async Task<Result<bool>> DeleteAsync(int id,
       CancellationToken ct = default)
    {
        // =========================================================
        // Current User
        // =========================================================

        var evaluatorId = _currentUser.UserId;

        if (string.IsNullOrWhiteSpace(evaluatorId))
        {
            return Result<bool>.Failure(
                "Authenticated evaluator not found.");
        }


        // =========================================================
        // Get Evaluation
        // =========================================================

        var evaluation =
            await _context.EmployeeEvaluations
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    ct);

        if (evaluation == null)
        {
            return Result<bool>.Failure(
                "Employee evaluation not found.");
        }


        // =========================================================
        // Ownership
        // =========================================================

        if (evaluation.EvaluatorId != evaluatorId)
        {
            return Result<bool>.Failure(
                "You are not allowed to delete this evaluation.");
        }


        // =========================================================
        // Validate Status
        // =========================================================

        if (evaluation.Status == EmployeeEvaluationStatus.Submitted)
        {
            return Result<bool>.Failure(
                "Submitted evaluation cannot be deleted.");
        }

        if (evaluation.Status == EmployeeEvaluationStatus.Approved)
        {
            return Result<bool>.Failure(
                "Approved evaluation cannot be deleted.");
        }


        // =========================================================
        // Old Values
        // =========================================================

        var oldValues = new
        {
            evaluation.EmployeeId,
            evaluation.EvaluatorId,
            evaluation.PeriodId,
            evaluation.TemplateId,
            evaluation.TotalScore,
            evaluation.FinalRate,
            evaluation.Status
        };


        // =========================================================
        // Soft Delete
        // =========================================================

        evaluation.IsDeleted = true;


        // =========================================================
        // Save
        // =========================================================

        await _context.SaveChangesAsync(ct);


        // =========================================================
        // Audit
        // =========================================================

        await _auditService.LogAsync(
            "Delete",
            "EmployeeEvaluation",
            evaluation.Id.ToString(),
            oldValues,
            new
            {
                IsDeleted = true
            });


        // =========================================================
        // Return
        // =========================================================
        return Result<bool>.Succeeded(
            true,
            "Employee evaluation has been deleted successfully.");
    }

    private string GetEvaluationMessage(
    string criterionName,
    decimal score)
    {
        if (score < 0 || score > 100)
            throw new ArgumentOutOfRangeException(
                nameof(score),
                "Score must be between 0 and 100.");

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

            _ => throw new ArgumentException(
                $"Unknown evaluation criterion: {criterionName}",
                nameof(criterionName))
        };
    }
}
