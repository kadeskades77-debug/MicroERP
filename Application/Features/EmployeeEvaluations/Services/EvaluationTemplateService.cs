
using AutoMapper;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.EmployeeEvaluations.Interfaces;
using MicroERP.Domin.Entities.EmployeeEvaluations;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.EmployeeEvaluations.Services;

public class EvaluationTemplateService : IEvaluationTemplateService
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;
    private readonly IMapper _mapper;

    public EvaluationTemplateService(IApplicationDbContext context, IAuditService auditService, IMapper mapper)
    {
        _context = context;
        _auditService = auditService;
        _mapper = mapper;
    }

    public async Task<Result<EvaluationTemplateDto>> CreateAsync(
      CreateEvaluationTemplateDto dto,
      CancellationToken ct = default)
    {
        if (dto == null)
        {
            return Result<EvaluationTemplateDto>.Failure(
                "Evaluation template data is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return Result<EvaluationTemplateDto>.Failure(
                "Evaluation template name is required.");
        }

        if (dto.Criteria == null || dto.Criteria.Count == 0)
        {
            return Result<EvaluationTemplateDto>.Failure(
                "At least one evaluation criterion is required.");
        }

        var criteria = dto.Criteria;

        if (criteria.Any(x => string.IsNullOrWhiteSpace(x.Name)))
        {
            return Result<EvaluationTemplateDto>.Failure(
                "All evaluation criteria must have a name.");
        }

        if (criteria.Any(x => x.MaxScore <= 0))
        {
            return Result<EvaluationTemplateDto>.Failure(
                "MaxScore must be greater than zero.");
        }

        if (criteria.Any(x => x.Weight <= 0))
        {
            return Result<EvaluationTemplateDto>.Failure(
                "Criterion weight must be greater than zero.");
        }

        if (criteria.Any(x => x.Weight > 100))
        {
            return Result<EvaluationTemplateDto>.Failure(
                "Criterion weight cannot exceed 100.");
        }

        var totalWeight = criteria.Sum(x => x.Weight);

        if (totalWeight != 100)
        {
            return Result<EvaluationTemplateDto>.Failure(
                $"The total criteria weight must equal 100. Current total: {totalWeight}.");
        }

        var template = new EvaluationTemplate
        {
            Name = dto.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(dto.Description)
                ? null
                : dto.Description.Trim()
        };

        for (var i = 0; i < criteria.Count; i++)
        {
            var criterionDto = criteria[i];

            template.Criteria.Add(new EvaluationCriterion
            {
                Name = criterionDto.Name.Trim(),
                Description = string.IsNullOrWhiteSpace(criterionDto.Description)
                    ? null
                    : criterionDto.Description.Trim(),
                MaxScore = criterionDto.MaxScore,
                Weight = criterionDto.Weight,
                SortOrder = i + 1
            });
        }

        _context.EvaluationTemplates.Add(template);

        await _context.SaveChangesAsync(ct);

        var result = new EvaluationTemplateDto
        {
            Id = template.Id,
            Name = template.Name,
            Description = template.Description,
            IsActive = template.IsActive,
            Criteria = template.Criteria
                .OrderBy(x => x.SortOrder)
                .Select(x => new EvaluationCriterionDto
                {
                    Id = x.Id,
                    TemplateId = template.Id,
                    Name = x.Name,
                    Description = x.Description,
                    MaxScore = x.MaxScore,
                    Weight = x.Weight,
                    SortOrder = x.SortOrder
                })
                .ToList()
        };

        return Result<EvaluationTemplateDto>.Succeeded(result);
    }

    public async Task<Result<EvaluationTemplateDto>> GetByIdAsync(int id,
       CancellationToken ct = default)
    {
        var template =
            await _context.EvaluationTemplates
                .AsNoTracking()
                .Include(x => x.Criteria)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    ct);

        if (template == null)
        {
            return Result<EvaluationTemplateDto>.Failure(
                "Evaluation template not found.");
        }

        var result =
            _mapper.Map<EvaluationTemplateDto>(template);

        return Result<EvaluationTemplateDto>.Succeeded(result);
    }

    public async Task<Result<List<EvaluationTemplateDto>>> GetAllAsync(
    CancellationToken ct = default)
    {
        var query =
            _context.EvaluationTemplates
                .AsNoTracking()
                .Include(x => x.Criteria)
                .AsQueryable();


        var templates =
            await query
                .OrderBy(x => x.Name)
                .ToListAsync(ct);

        var result =
            _mapper.Map<List<EvaluationTemplateDto>>(templates);

        return Result<List<EvaluationTemplateDto>>.Succeeded(result);
    }

    public async Task<Result<EvaluationTemplateDto>> UpdateAsync(int id,
     UpdateEvaluationTemplateDto dto,
     CancellationToken ct = default)
    {
        // =========================================================
        // Validation
        // =========================================================

        if (dto == null)
        {
            return Result<EvaluationTemplateDto>.Failure(
                "Evaluation template data is required.");
        }


        // =========================================================
        // Get Template
        // =========================================================

        var template =
            await _context.EvaluationTemplates
                .Include(x => x.Criteria)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    ct);

        if (template == null)
        {
            return Result<EvaluationTemplateDto>.Failure(
                "Evaluation template not found.");
        }
        var isUsed =
        await _context.EmployeeEvaluations
        .AnyAsync(
            x => x.Template.Id == id,
            ct);

        if (isUsed)
        {
            return Result<EvaluationTemplateDto>.Failure(
                "Evaluation template cannot be modified because it is already used in evaluations.");
        }

        // =========================================================
        // Validate Template Name
        // =========================================================

        if (dto.Name != null &&
            string.IsNullOrWhiteSpace(dto.Name))
        {
            return Result<EvaluationTemplateDto>.Failure(
                "Evaluation template name cannot be empty.");
        }


        // =========================================================
        // Validate Criteria IDs
        // =========================================================

        if (dto.Criteria != null)
        {
            if (dto.Criteria.Count == 0)
            {
                return Result<EvaluationTemplateDto>.Failure(
                    "At least one evaluation criterion is required.");
            }


            var duplicateIds =
                dto.Criteria
                    .Where(x => x.Id > 0)
                    .GroupBy(x => x.Id)
                    .Where(x => x.Count() > 1)
                    .Select(x => x.Key)
                    .ToList();

            if (duplicateIds.Count > 0)
            {
                return Result<EvaluationTemplateDto>.Failure(
                    "Duplicate evaluation criterion IDs are not allowed.");
            }


            var templateCriterionIds =
                template.Criteria
                    .Select(x => x.Id)
                    .ToHashSet();


            var invalidIds =
                dto.Criteria
                    .Where(x =>
                        x.Id <= 0 ||
                        !templateCriterionIds.Contains(x.Id))
                    .Select(x => x.Id)
                    .Distinct()
                    .ToList();

            if (invalidIds.Count > 0)
            {
                return Result<EvaluationTemplateDto>.Failure(
                    "One or more evaluation criteria do not belong to this template.");
            }
        }


        // =========================================================
        // Old Values
        // =========================================================

        var oldValues = new
        {
            template.Name,
            template.Description,

            Criteria = template.Criteria
                .OrderBy(x => x.SortOrder)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Description,
                    x.MaxScore,
                    x.Weight,
                    x.SortOrder
                })
                .ToList()
        };


        // =========================================================
        // Partial Update - Template
        // =========================================================

        if (dto.Name != null)
        {
            template.Name =
                dto.Name.Trim();
        }


        if (dto.Description != null)
        {
            template.Description =
                string.IsNullOrWhiteSpace(dto.Description)
                    ? null
                    : dto.Description.Trim();
        }


        // =========================================================
        // Partial Update - Criteria
        // =========================================================

        if (dto.Criteria != null)
        {
            foreach (var criterionDto in dto.Criteria)
            {
                var criterion =
                    template.Criteria.First(
                        x => x.Id == criterionDto.Id);


                if (criterionDto.Name != null)
                {
                    if (string.IsNullOrWhiteSpace(
                        criterionDto.Name))
                    {
                        return Result<EvaluationTemplateDto>.Failure(
                            $"Criterion {criterion.Id} name cannot be empty.");
                    }

                    criterion.Name =
                        criterionDto.Name.Trim();
                }


                if (criterionDto.Description != null)
                {
                    criterion.Description =
                        string.IsNullOrWhiteSpace(
                            criterionDto.Description)
                            ? null
                            : criterionDto.Description.Trim();
                }


                if (criterionDto.MaxScore.HasValue)
                {
                    if (criterionDto.MaxScore.Value <= 0)
                    {
                        return Result<EvaluationTemplateDto>.Failure(
                            $"MaxScore for criterion {criterion.Id} must be greater than zero.");
                    }

                    criterion.MaxScore =
                        criterionDto.MaxScore.Value;
                }


                if (criterionDto.Weight.HasValue)
                {
                    if (criterionDto.Weight.Value <= 0)
                    {
                        return Result<EvaluationTemplateDto>.Failure(
                            $"Weight for criterion {criterion.Id} must be greater than zero.");
                    }

                    if (criterionDto.Weight.Value > 100)
                    {
                        return Result<EvaluationTemplateDto>.Failure(
                            $"Weight for criterion {criterion.Id} cannot exceed 100.");
                    }

                    criterion.Weight =
                        criterionDto.Weight.Value;
                }


                if (criterionDto.SortOrder.HasValue)
                {
                    if (criterionDto.SortOrder.Value <= 0)
                    {
                        return Result<EvaluationTemplateDto>.Failure(
                            $"SortOrder for criterion {criterion.Id} must be greater than zero.");
                    }

                    criterion.SortOrder =
                        criterionDto.SortOrder.Value;
                }
            }


            // =====================================================
            // Validate Final Weights
            // =====================================================

            var totalWeight =
                template.Criteria
                    .Sum(x => x.Weight);


            if (totalWeight != 100)
            {
                return Result<EvaluationTemplateDto>.Failure(
                    $"The total criteria weight must equal 100. Current total: {totalWeight}.");
            }
        }


        // =========================================================
        // New Values
        // =========================================================

        var newValues = new
        {
            template.Name,
            template.Description,

            Criteria = template.Criteria
                .OrderBy(x => x.SortOrder)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Description,
                    x.MaxScore,
                    x.Weight,
                    x.SortOrder
                })
                .ToList()
        };


        // =========================================================
        // Save
        // =========================================================

        await _context.SaveChangesAsync(ct);


        // =========================================================
        // Audit
        // =========================================================

        await _auditService.LogAsync(
            "Update",
            "EvaluationTemplate",
            template.Id.ToString(),
            oldValues,
            newValues);


        // =========================================================
        // Reload
        // =========================================================

        var result =
            await _context.EvaluationTemplates
                .AsNoTracking()
                .Include(x => x.Criteria)
                .FirstAsync(
                    x => x.Id == id,
                    ct);


        // =========================================================
        // Return
        // =========================================================

        return Result<EvaluationTemplateDto>.Succeeded(
            _mapper.Map<EvaluationTemplateDto>(result));
    }

    public async Task<Result<bool>> DeleteAsync(int id,
    CancellationToken ct = default)
    {
        // =========================================================
        // Get Template
        // =========================================================

        var template =
            await _context.EvaluationTemplates
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    ct);

        if (template == null)
        {
            return Result<bool>.Failure(
                "Evaluation template not found.");
        }


        // =========================================================
        // Old Values
        // =========================================================

        var oldValues = new
        {
            template.Name,
            template.Description,
            template.IsActive,
            template.IsDeleted
        };


        // =========================================================
        // Soft Delete
        // =========================================================

        template.IsDeleted = true;
        template.IsActive = false;


        // =========================================================
        // Save
        // =========================================================

        await _context.SaveChangesAsync(ct);


        // =========================================================
        // Audit
        // =========================================================

        var newValues = new
        {
            template.Name,
            template.Description,
            template.IsActive,
            template.IsDeleted
        };

        await _auditService.LogAsync(
            "Delete",
            "EvaluationTemplate",
            template.Id.ToString(),
            oldValues,
            newValues);


        // =========================================================
        // Return
        // =========================================================

        return Result<bool>.Succeeded(true);
    }
}
