using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeEvaluations.DTOs.EmployeeEvaluationDto;
using MicroERP.Domin.Entities.EmployeeEvaluations;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.EmployeeEvaluations.Queries;

public class EmployeeEvaluationQueries : IEmployeeEvaluationQueries
{
    private readonly IApplicationDbContext _context;

    public EmployeeEvaluationQueries(
        IApplicationDbContext context)
    {
        _context = context;
    }


    // =========================================================
    // PUBLIC QUERY METHODS
    // =========================================================

    // =========================================================
    // 1. Get Paged
    // =========================================================

    public async Task<Result<PagedResult<EmployeeEvaluationListDto>>>
        GetPagedAsync(
            EmployeeEvaluationFilterDto filter,
            CancellationToken ct = default)
    {
        var query =
            ApplyFilters(
                BuildQuery(),
                filter);

        var totalCount =
            await query.CountAsync(ct);

        var pageNumber =
            NormalizePageNumber(
                filter.PageNumber);

        var pageSize =
            NormalizePageSize(
                filter.PageSize);

        var result =
            await ApplyOrdering(
                SelectListDto(query))
            .Skip(
                (pageNumber - 1) *
                pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var pagedResult =
            new PagedResult<EmployeeEvaluationListDto>
            {
                Items = result,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

        return Result<
            PagedResult<EmployeeEvaluationListDto>>
            .Succeeded(pagedResult);
    }


    // =========================================================
    // 2. Get By Id
    // =========================================================

    public async Task<Result<EmployeeEvaluationDto>>
        GetByIdAsync(
            int id,
            CancellationToken ct = default)
    {
        var evaluation =
            await BuildQuery()
                .Include(x => x.Employee)
                    .ThenInclude(x => x.User)
                .Include(x => x.Evaluator)
                .Include(x => x.Period)
                .Include(x => x.Template)
                .Include(x => x.Items)
                    .ThenInclude(x => x.Criterion)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    ct);

        if (evaluation == null)
        {
            return Result<EmployeeEvaluationDto>.Failure(
                "Employee evaluation not found.");
        }

        return Result<EmployeeEvaluationDto>
            .Succeeded(
                MapEvaluationDto(evaluation));
    }


    // =========================================================
    // 3. Get By Employee And Month
    // =========================================================

    public async Task<Result<EmployeeEvaluationDto>>
        GetByEmployeeAndMonthAsync(
            int employeeId,
            int year,
            int month,
            CancellationToken ct = default)
    {
        var validation =
            ValidateYearMonth(
                year,
                month);

        if (!validation.Success)
        {
            return Result<EmployeeEvaluationDto>.Failure(
                validation.Message);
        }

        var evaluation =
            await BuildQuery()
                .Include(x => x.Employee)
                    .ThenInclude(x => x.User)
                .Include(x => x.Evaluator)
                .Include(x => x.Period)
                .Include(x => x.Template)
                .Include(x => x.Items)
                    .ThenInclude(x => x.Criterion)
                .FirstOrDefaultAsync(
                    x =>
                        x.EmployeeId == employeeId &&
                        x.Period.StartDate.Year == year &&
                        x.Period.StartDate.Month == month,
                    ct);

        if (evaluation == null)
        {
            return Result<EmployeeEvaluationDto>.Failure(
                "Employee evaluation not found for the specified month.");
        }

        return Result<EmployeeEvaluationDto>
            .Succeeded(
                MapEvaluationDto(evaluation));
    }

    public async Task<
    Result<EmployeeMonthlyEvaluationExcelResultDto>>
    GetEmployeeMonthlyReportAsync(
        EmployeeMonthlyEvaluationExcelFilterDto filter,
        CancellationToken ct = default)
    {
        // =========================================================
        // Validate Filter
        // =========================================================

        if (filter == null)
        {
            return Result<EmployeeMonthlyEvaluationExcelResultDto>
                .Failure(
                    "Monthly evaluation filter is required.");
        }

        // =========================================================
        // Resolve Year
        // =========================================================

        var year =
            filter.Year ??
            DateTime.Today.Year;

        // =========================================================
        // Validate Year / Month
        // =========================================================

        var validation =
            ValidateYearMonth(
                year,
                filter.Month);

        if (!validation.Success)
        {
            return Result<EmployeeMonthlyEvaluationExcelResultDto>
                .Failure(
                    validation.Message);
        }

        // =========================================================
        // Get Evaluation
        // =========================================================

        var evaluation =
            await BuildQuery()
                .Include(x => x.Employee)
                    .ThenInclude(x => x.User)

                .Include(x => x.Employee)
                    .ThenInclude(x => x.Department)

                .Include(x => x.Period)

                .Include(x => x.Items)
                    .ThenInclude(x => x.Criterion)

                .FirstOrDefaultAsync(
                    x =>
                        x.EmployeeId == filter.EmployeeId &&
                        x.Status == EmployeeEvaluationStatus.Approved &&
                        x.Period.StartDate.Year == year &&
                        x.Period.StartDate.Month == filter.Month,
                    ct);

        if (evaluation == null)
        {
            return Result<EmployeeMonthlyEvaluationExcelResultDto>
                .Failure(
                    "Approved employee evaluation not found for the specified month.");
        }

        // =========================================================
        // Header
        // =========================================================

        var header =
            new EmployeeMonthlyEvaluationExcelHeaderDto
            {
                EmployeeName =
                    evaluation.Employee.User.FullName,

                DepartmentName =
                    evaluation.Employee.Department.NameEn,

                PeriodName =
                    evaluation.Period.Name,

                TotalScore =
                    evaluation.TotalScore,

                FinalRate =
                    evaluation.FinalRate,

                Status =
                    evaluation.Status
            };

        // =========================================================
        // Items
        // =========================================================

        var items =
            evaluation.Items
                .OrderBy(x => x.Criterion.SortOrder)
                .Select(x =>
                    new EmployeeMonthlyEvaluationExcelDto
                    {
                        CriterionName =
                            x.Criterion.Name,

                        Score =
                            x.Score,

                        Notes =
                            x.Notes
                    })
                .ToList();

        // =========================================================
        // Result
        // =========================================================

        var result =
            new EmployeeMonthlyEvaluationExcelResultDto
            {
                Header = header,
                Items = items
            };

        return Result<EmployeeMonthlyEvaluationExcelResultDto>
            .Succeeded(result);
    }


    // =========================================================
    // 4. Get Ranking
    // =========================================================

    public async Task<Result<List<EmployeeEvaluationRankingDto>>>
        GetRankingAsync(
            EvaluationRankingFilterDto filter,
            CancellationToken ct = default)
    {
        if (filter == null)
        {
            return Result<List<EmployeeEvaluationRankingDto>>
                .Failure(
                    "Ranking filter is required.");
        }

        if (!Enum.IsDefined(
                typeof(EvaluationRankingType),
                filter.Type))
        {
            return Result<List<EmployeeEvaluationRankingDto>>
                .Failure(
                    "Invalid evaluation ranking type.");
        }

        var rankingCount =
            filter.RankingCount ?? 1;

        const int maxRankingCount = 100;

        if (rankingCount <= 0)
        {
            return Result<List<EmployeeEvaluationRankingDto>>
                .Failure(
                    "Ranking count must be greater than zero.");
        }

        if (rankingCount > maxRankingCount)
        {
            return Result<List<EmployeeEvaluationRankingDto>>
                .Failure(
                    $"Ranking count cannot exceed {maxRankingCount}.");
        }

        if (filter.Year.HasValue &&
            filter.Year.Value <= 0)
        {
            return Result<List<EmployeeEvaluationRankingDto>>
                .Failure(
                    "Year must be greater than zero.");
        }

        if (filter.Month.HasValue &&
            (filter.Month.Value < 1 ||
             filter.Month.Value > 12))
        {
            return Result<List<EmployeeEvaluationRankingDto>>
                .Failure(
                    "Month must be between 1 and 12.");
        }

        if (filter.Month.HasValue &&
            !filter.Year.HasValue)
        {
            return Result<List<EmployeeEvaluationRankingDto>>
                .Failure(
                    "Year is required when Month is specified.");
        }

        if (filter.DepartmentId.HasValue)
        {
            var departmentExists =
                await _context.Departments
                    .AsNoTracking()
                    .AnyAsync(
                        x => x.Id == filter.DepartmentId.Value,
                        ct);

            if (!departmentExists)
            {
                return Result<List<EmployeeEvaluationRankingDto>>
                    .Failure(
                        "Department not found.");
            }
        }

        if (filter.GroupByDepartment &&
            filter.DepartmentId.HasValue)
        {
            return Result<List<EmployeeEvaluationRankingDto>>
                .Failure(
                    "GroupByDepartment cannot be used with DepartmentId.");
        }

        var query =
            _context.EmployeeEvaluations
                .AsNoTracking()
                .Where(x =>
                    x.Status ==
                        EmployeeEvaluationStatus.Submitted
                    ||
                    x.Status ==
                        EmployeeEvaluationStatus.Approved);

        if (filter.Year.HasValue)
        {
            var year =
                filter.Year.Value;

            query =
                query.Where(x =>
                    x.Period.StartDate.Year == year);
        }

        if (filter.Month.HasValue)
        {
            var month =
                filter.Month.Value;

            query =
                query.Where(x =>
                    x.Period.StartDate.Month == month);
        }

        if (filter.DepartmentId.HasValue)
        {
            var departmentId =
                filter.DepartmentId.Value;

            query =
                query.Where(x =>
                    x.Employee.DepartmentId == departmentId);
        }

        var result =
            await query
                .Select(x => new
                {
                    x.EmployeeId,

                    EmployeeName =
                        x.Employee.User.FullName,

                    DepartmentId =
                        x.Employee.DepartmentId,

                    DepartmentName =
                        x.Employee.Department.NameEn,

                    Year =
                        x.Period.StartDate.Year,

                    Month =
                        x.Period.StartDate.Month,

                    TemplateName =
                        x.Template.Name,

                    x.TotalScore,

                    x.FinalRate,

                    x.Status
                })
                .ToListAsync(ct);

        if (result.Count == 0)
        {
            return Result<List<EmployeeEvaluationRankingDto>>
                .Succeeded(
                    new List<EmployeeEvaluationRankingDto>());
        }

        if (!filter.GroupByDepartment)
        {
            var ranked =
                filter.Type == EvaluationRankingType.Top
                    ? result
                        .OrderByDescending(x => x.TotalScore)
                        .ThenBy(x => x.EmployeeName)
                        .Take(rankingCount)
                        .ToList()

                    : result
                        .OrderBy(x => x.TotalScore)
                        .ThenBy(x => x.EmployeeName)
                        .Take(rankingCount)
                        .ToList();

            var output =
                ranked
                    .Select(
                        (x, index) =>
                            new EmployeeEvaluationRankingDto
                            {
                                EmployeeId =
                                    x.EmployeeId,

                                EmployeeName =
                                    x.EmployeeName,

                                DepartmentId =
                                    x.DepartmentId,

                                DepartmentName =
                                    x.DepartmentName,

                                Year =
                                    x.Year,

                                Month =
                                    x.Month,

                                TemplateName =
                                    x.TemplateName,

                                TotalScore =
                                    x.TotalScore,

                                FinalRate =
                                    x.FinalRate,

                                Status =
                                    x.Status,

                                Type =
                                    filter.Type,

                                Rank =
                                    index + 1
                            })
                    .ToList();

            return Result<List<EmployeeEvaluationRankingDto>>
                .Succeeded(output);
        }

        var grouped =
            result
                .GroupBy(x => new
                {
                    x.DepartmentId,
                    x.DepartmentName
                });

        var departmentResults =
            new List<EmployeeEvaluationRankingDto>();

        foreach (var department in grouped)
        {
            var rankedDepartment =
                filter.Type == EvaluationRankingType.Top
                    ? department
                        .OrderByDescending(x => x.TotalScore)
                        .ThenBy(x => x.EmployeeName)
                        .Take(rankingCount)

                    : department
                        .OrderBy(x => x.TotalScore)
                        .ThenBy(x => x.EmployeeName)
                        .Take(rankingCount);

            var rank = 1;

            foreach (var item in rankedDepartment)
            {
                departmentResults.Add(
                    new EmployeeEvaluationRankingDto
                    {
                        EmployeeId =
                            item.EmployeeId,

                        EmployeeName =
                            item.EmployeeName,

                        DepartmentId =
                            item.DepartmentId,

                        DepartmentName =
                            item.DepartmentName,

                        Year =
                            item.Year,

                        Month =
                            item.Month,

                        TemplateName =
                            item.TemplateName,

                        TotalScore =
                            item.TotalScore,

                        FinalRate =
                            item.FinalRate,

                        Status =
                            item.Status,

                        Type =
                            filter.Type,

                        Rank =
                            rank++
                    });
            }
        }

        departmentResults =
            departmentResults
                .OrderBy(x => x.DepartmentName)
                .ThenBy(x => x.Rank)
                .ToList();

        return Result<List<EmployeeEvaluationRankingDto>>
            .Succeeded(
                departmentResults);
    }


    // =========================================================
    // 5. Get All Employees For Excel
    // =========================================================

    public async Task<Result<List<EmployeeEvaluationExcelDto>>>
       GetForExcelAsync(
           EmployeeEvaluationExcelFilterDto filter,
           CancellationToken ct = default)
    {
        // =========================================================
        // Validate Filter
        // =========================================================
        
        if (filter == null)
        {
            return Result<List<EmployeeEvaluationExcelDto>>
                .Failure(
                    "Excel filter is required.");
        }

        // =========================================================
        // Validate Period
        // =========================================================

        var validation =
            ValidatePeriodFilter(
                filter.Year,
                filter.Period,
                filter.Month,
                filter.Quarter);

        if (!validation.Success)
        {
            return Result<List<EmployeeEvaluationExcelDto>>
                .Failure(
                    validation.Message);
        }

        // =========================================================
        // Build Query
        // =========================================================

        var query =
            BuildBaseQuery();

        // =========================================================
        // Year Filter
        // =========================================================

        query =
            ApplyYearFilter(
                query,
                filter.Year);

        // =========================================================
        // Department Filter
        // =========================================================

        query =
            ApplyDepartmentFilter(
                query,
                filter.DepartmentId);

        // =========================================================
        // Status Filter
        // =========================================================

        query =
            ApplyStatusFilter(
                query,
                filter.Status);

        // =========================================================
        // Execute Query
        // =========================================================

        var evaluations =
            await query
                .ToListAsync(ct);

        // =========================================================
        // Filter Report Period
        // =========================================================

        evaluations =
            FilterByReportPeriod(
                evaluations,
                filter.Period,
                filter.Month,
                filter.Quarter);

        // =========================================================
        // No Data
        // =========================================================

        if (evaluations.Count == 0)
        {
            return Result<List<EmployeeEvaluationExcelDto>>
                .Failure(
                    "No employee evaluations found for the specified criteria.");
        }

        // =========================================================
        // Build Excel Rows
        // =========================================================

        var result =
            BuildEmployeeExcelRows(
                evaluations,
                filter.Period);

        // =========================================================
        // No Result Rows
        // =========================================================

        if (result == null ||
            result.Count == 0)
        {
            return Result<List<EmployeeEvaluationExcelDto>>
                .Failure(
                    "No employee evaluation report data found.");
        }

        // =========================================================
        // Return Result
        // =========================================================

        return Result<List<EmployeeEvaluationExcelDto>>
            .Succeeded(result);
    }


    // =========================================================
    // 6. Get Employee History For Excel
    // =========================================================

    public async Task<
      Result<EmployeeEvaluationHistoryExcelResultDto>>
      GetEmployeeHistoryForExcelAsync(
          EmployeeEvaluationHistoryExcelFilterDto filter,
          CancellationToken ct = default)
    {
        // =========================================================
        // Validate Filter
        // =========================================================

        if (filter == null)
        {
            return Result<EmployeeEvaluationHistoryExcelResultDto>
                .Failure(
                    "History filter is required.");
        }

        // =========================================================
        // Validate Period
        // =========================================================

        var validation =
            ValidatePeriodFilter(
                filter.Year,
                filter.Period,
                filter.Quarter,
                filter.HalfYear);

        if (!validation.Success)
        {
            return Result<EmployeeEvaluationHistoryExcelResultDto>
                .Failure(
                    validation.Message);
        }

        // =========================================================
        // Resolve Year
        // =========================================================

        var year =
            filter.Year ??
            DateTime.Today.Year;

        // =========================================================
        // Employee
        // =========================================================

        var employee =
            await GetEmployeeHeaderAsync(
                filter.EmployeeId,
                ct);

        if (employee == null)
        {
            return Result<EmployeeEvaluationHistoryExcelResultDto>
                .Failure(
                    "Employee not found.");
        }

        // =========================================================
        // Base Query
        // Approved evaluations only
        // =========================================================

        var query =
            BuildBaseQuery()
                .Where(x =>
                    x.EmployeeId == filter.EmployeeId &&
                    x.Status == EmployeeEvaluationStatus.Approved);

        // =========================================================
        // Year Filter
        // =========================================================

        query =
            ApplyYearFilter(
                query,
                year);

        // =========================================================
        // Execute Query
        // =========================================================

        var evaluations =
            await query
                .ToListAsync(ct);

        // =========================================================
        // Filter Report Period
        // =========================================================

        evaluations =
            FilterByReportPeriod(
                evaluations,
                filter.Period,
                filter.Quarter,
                filter.HalfYear);

        // =========================================================
        // No Data
        // =========================================================

        if (evaluations.Count == 0)
        {
            return Result<EmployeeEvaluationHistoryExcelResultDto>
                .Failure(
                    "No approved employee evaluations found for the specified criteria.");
        }

        // =========================================================
        // Build Rows
        // =========================================================

        var rows =
            BuildEmployeeHistoryRows(
                evaluations,
                filter.Period);

        if (rows.Count == 0)
        {
            return Result<EmployeeEvaluationHistoryExcelResultDto>
                .Failure(
                    "No employee evaluation report data found.");
        }

        // =========================================================
        // Calculate Report Total Score
        // =========================================================

        var reportTotalScore =
            CalculatePeriodScore(
                evaluations);

        // =========================================================
        // Build Header
        // =========================================================

        var header =
            new EmployeeEvaluationExcelHeaderDto
            {
                EmployeeName =
                    employee.EmployeeName,

                DepartmentName =
                    employee.DepartmentName,

                PeriodLabel =
                    BuildPeriodLabel(
                        year,
                        filter.Period,
                        filter.Quarter,
                        filter.HalfYear),

                PeriodType =
                    filter.Period,

                TotalScore =
                    reportTotalScore,

                FinalRate =
                    CalculateFinalRate(
                        reportTotalScore)
            };

        // =========================================================
        // Result
        // =========================================================

        var result =
            new EmployeeEvaluationHistoryExcelResultDto
            {
                Header =
                    header,

                Evaluations =
                    rows
            };

        return Result<EmployeeEvaluationHistoryExcelResultDto>
            .Succeeded(result);
    }


    // =========================================================
    // 7. Get Department For Excel
    // =========================================================

    public async Task<
     Result<DepartmentEvaluationExcelResultDto>>
     GetDepartmentForExcelAsync(
         DepartmentEvaluationExcelFilterDto filter,
         CancellationToken ct = default)
    {
        var validation =
            ValidatePeriodFilter(
                filter.Year,
                filter.Period,
                filter.Month,
                filter.Quarter);

        if (!validation.Success)
        {
            return Result<DepartmentEvaluationExcelResultDto>
                .Failure(
                    validation.Message);
        }

        var department =
            await GetDepartmentHeaderAsync(
                filter.DepartmentId,
                ct);

        if (department == null)
        {
            return Result<DepartmentEvaluationExcelResultDto>
                .Failure(
                    "Department not found.");
        }

        var query =
            BuildBaseQuery()
                .Where(x =>
                    x.DepartmentId ==
                    filter.DepartmentId);

        query =
            ApplyYearFilter(
                query,
                filter.Year);

        query =
            ApplyStatusFilter(
                query,
                filter.Status);

        var evaluations =
            await query
                .ToListAsync(ct);

        evaluations =
            FilterByReportPeriod(
                evaluations,
                filter.Period,
                filter.Month,
                filter.Quarter);

        var employees =
            BuildDepartmentEmployeeRows(
                evaluations,
                filter.Period);

        // =========================================================
        // No Data
        // =========================================================

        if (employees.Count == 0)
        {
            return Result<DepartmentEvaluationExcelResultDto>
                .Failure(
                    "No department evaluations found for the specified criteria.");
        }

        var averageScore =
            employees.Average(
                x => x.TotalScore);

        var header =
            new DepartmentEvaluationExcelHeaderDto
            {
                DepartmentName =
                    department.DepartmentName,

                ManagerName =
                    department.ManagerName,

                PeriodLabel =
                    BuildPeriodLabel(
                        filter.Year,
                        filter.Period,
                        filter.Month,
                        filter.Quarter),

                PeriodType =
                    filter.Period,

                AverageScore =
                    Math.Round(
                        averageScore,
                        2),

                FinalRate =
                    CalculateFinalRate(
                        averageScore)
            };

        var result =
            new DepartmentEvaluationExcelResultDto
            {
                Header = header,
                Employees = employees
            };

        return Result<DepartmentEvaluationExcelResultDto>
            .Succeeded(result);
    }


    // =========================================================
    // PRIVATE QUERY HELPERS
    // =========================================================

    // =========================================================
    // Base Query
    // =========================================================

    private IQueryable<EmployeeEvaluation>
        BuildQuery()
    {
        return _context.EmployeeEvaluations
            .AsNoTracking();
    }


    // =========================================================
    // Apply Filters
    // =========================================================

    private static IQueryable<EmployeeEvaluation>
        ApplyFilters(
            IQueryable<EmployeeEvaluation> query,
            EmployeeEvaluationFilterDto filter)
    {
        if (filter.EmployeeId.HasValue)
        {
            query = query.Where(
                x =>
                    x.EmployeeId ==
                    filter.EmployeeId.Value);
        }

        if (!string.IsNullOrWhiteSpace(
                filter.EvaluatorId))
        {
            query = query.Where(
                x =>
                    x.EvaluatorId ==
                    filter.EvaluatorId);
        }

        if (filter.TemplateId.HasValue)
        {
            query = query.Where(
                x =>
                    x.TemplateId ==
                    filter.TemplateId.Value);
        }

        if (filter.Year.HasValue)
        {
            query = query.Where(
                x =>
                    x.Period.StartDate.Year ==
                    filter.Year.Value);
        }

        if (filter.Month.HasValue)
        {
            query = query.Where(
                x =>
                    x.Period.StartDate.Month ==
                    filter.Month.Value);
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(
                x =>
                    x.Status ==
                    filter.Status.Value);
        }

        if (filter.FinalRate.HasValue)
        {
            query = query.Where(
                x =>
                    x.FinalRate ==
                    filter.FinalRate.Value);
        }

        return query;
    }


    // =========================================================
    // List Ordering
    // =========================================================

    private static IQueryable<EmployeeEvaluationListDto>
        ApplyOrdering(
            IQueryable<EmployeeEvaluationListDto> query)
    {
        return query
            .OrderByDescending(x => x.Year)
            .ThenByDescending(x => x.Month)
            .ThenBy(x => x.EmployeeName);
    }


    // =========================================================
    // List Projection
    // =========================================================

    private IQueryable<EmployeeEvaluationListDto>
        SelectListDto(
            IQueryable<EmployeeEvaluation> query)
    {
        var users =
            _context.Users.AsNoTracking();

        return
            from evaluation in query

            join rejectedUser in users
                on evaluation.RejectedBy equals rejectedUser.Id
                into rejectedUsers

            from rejectedUser in rejectedUsers
                .DefaultIfEmpty()

            join approvedUser in users
                on evaluation.ApprovedBy equals approvedUser.Id
                into approvedUsers

            from approvedUser in approvedUsers
                .DefaultIfEmpty()

            select new EmployeeEvaluationListDto
            {
                Id =
                    evaluation.Id,

                EmployeeId =
                    evaluation.EmployeeId,

                EmployeeName =
                    evaluation.Employee.User.FullName,

                EvaluatorName =
                    evaluation.Evaluator.FullName,

                PeriodId =
                    evaluation.PeriodId,

                PeriodName =
                    evaluation.Period.Name,

                Year =
                    evaluation.Period.StartDate.Year,

                Month =
                    evaluation.Period.StartDate.Month,

                TemplateId =
                    evaluation.TemplateId,

                TemplateName =
                    evaluation.Template.Name,

                TotalScore =
                    evaluation.TotalScore,

                FinalRate =
                    evaluation.FinalRate,

                RejectedByName =
                    rejectedUser != null
                        ? rejectedUser.FullName
                        : null,

                RejectedOn =
                    evaluation.RejectedOn,

                RejectionReason =
                    evaluation.RejectionReason,

                ApprovedByName =
                    approvedUser != null
                        ? approvedUser.FullName
                        : null,

                ApprovedOn =
                    evaluation.ApprovedOn,

                Status =
                    evaluation.Status
            };
    }


    // =========================================================
    // Build Base Query For Reports
    // =========================================================

    private IQueryable<EmployeeEvaluationQueryModel>
        BuildBaseQuery()
    {
        return
            _context.EmployeeEvaluations
                .AsNoTracking()
                .Where(x =>
                    x.Status ==
                        EmployeeEvaluationStatus.Submitted
                    ||
                    x.Status ==
                        EmployeeEvaluationStatus.Approved)
                .Select(x =>
                    new EmployeeEvaluationQueryModel
                    {
                        EmployeeId =
                            x.EmployeeId,

                        EmployeeName =
                            x.Employee.User.FullName,

                        DepartmentId =
                            x.Employee.DepartmentId,

                        DepartmentName =
                            x.Employee.Department.NameEn,

                        Year =
                            x.Period.StartDate.Year,

                        Month =
                            x.Period.StartDate.Month,

                        PeriodName =
                            x.Period.Name,

                        TotalScore =
                            x.TotalScore,

                        FinalRate =
                            x.FinalRate,

                        Status =
                            x.Status
                    });
    }


    // =========================================================
    // Report Filters
    // =========================================================

    private static IQueryable<EmployeeEvaluationQueryModel>
        ApplyYearFilter(
            IQueryable<EmployeeEvaluationQueryModel> query,
            int year)
    {
        return query.Where(x =>
            x.Year == year);
    }


    private static IQueryable<EmployeeEvaluationQueryModel>
        ApplyDepartmentFilter(
            IQueryable<EmployeeEvaluationQueryModel> query,
            int? departmentId)
    {
        if (!departmentId.HasValue)
            return query;

        return query.Where(x =>
            x.DepartmentId ==
            departmentId.Value);
    }


    private static IQueryable<EmployeeEvaluationQueryModel>
        ApplyStatusFilter(
            IQueryable<EmployeeEvaluationQueryModel> query,
            EmployeeEvaluationStatus? status)
    {
        if (!status.HasValue)
            return query;

        return query.Where(x =>
            x.Status ==
            status.Value);
    }


    // =========================================================
    // Report Period Filtering
    // =========================================================

    private static List<EmployeeEvaluationQueryModel>
        FilterByReportPeriod(
            List<EmployeeEvaluationQueryModel> source,
            EvaluationReportPeriod period,
            int? month,
            int? quarter)
    {
        return period switch
        {
            EvaluationReportPeriod.Monthly =>
                FilterMonthly(
                    source,
                    month),

            EvaluationReportPeriod.Quarterly =>
                FilterQuarterly(
                    source,
                    quarter),

            EvaluationReportPeriod.HalfYearly =>
                source,

            EvaluationReportPeriod.Yearly =>
                source,

            _ =>
                new List<EmployeeEvaluationQueryModel>()
        };
    }


    private static List<EmployeeEvaluationQueryModel>
        FilterMonthly(
            List<EmployeeEvaluationQueryModel> source,
            int? month)
    {
        if (!month.HasValue)
            return source;

        return source
            .Where(x =>
                x.Month ==
                month.Value)
            .ToList();
    }


    private static List<EmployeeEvaluationQueryModel>
        FilterQuarterly(
            List<EmployeeEvaluationQueryModel> source,
            int? quarter)
    {
        if (!quarter.HasValue)
            return source;

        var startMonth =
            ((quarter.Value - 1) * 3) + 1;

        var endMonth =
            startMonth + 2;

        return source
            .Where(x =>
                x.Month >= startMonth &&
                x.Month <= endMonth)
            .ToList();
    }


    // =========================================================
    // Excel Row Builders
    // =========================================================

    private static List<EmployeeEvaluationExcelDto>
    BuildEmployeeExcelRows(
        List<EmployeeEvaluationQueryModel> evaluations,
        EvaluationReportPeriod period)
    {
        return evaluations
            .GroupBy(x => x.EmployeeId)
            .Select(group =>
            {
                var reportTotalScore =
                    CalculatePeriodScore(group);

                var first =
                    group.First();

                return new EmployeeEvaluationExcelDto
                {
                   
                    EmployeeName =
                        first.EmployeeName,

                    DepartmentName =
                        first.DepartmentName,

                    PeriodLabel =
                        BuildGroupedPeriodLabel(
                            group,
                            period),

                    TotalScore =
                        reportTotalScore,

                    FinalRate =
                        CalculateFinalRate(
                            reportTotalScore),

                    Status =
                        first.Status
                };
            })
            .ToList();
    }
    private static List<EmployeeEvaluationHistoryExcelDto>
      BuildEmployeeHistoryRows(
          List<EmployeeEvaluationQueryModel> source,
          EvaluationReportPeriod period)
    {
        return source
            .GroupBy(x =>
                GetPeriodGroupKey(
                    x,
                    period))
            .OrderBy(x => x.Key)
            .Select(g =>
            {
                var score =
                    CalculatePeriodScore(g);

                return new EmployeeEvaluationHistoryExcelDto
                {
                    PeriodLabel =
                        BuildGroupedPeriodLabel(
                            g,
                            period),

                    TotalScore =
                        score,

                    FinalRate =
                        CalculateFinalRate(score)
                };
            })
            .ToList();
    }

    private static List<DepartmentEmployeeEvaluationExcelDto>
        BuildDepartmentEmployeeRows(
            List<EmployeeEvaluationQueryModel> source,
            EvaluationReportPeriod period)
    {
        return source
            .GroupBy(x => new
            {
                x.EmployeeId,
                x.EmployeeName
            })
            .Select(g =>
            {
                var score =
                    CalculatePeriodScore(
                        g);


                return new DepartmentEmployeeEvaluationExcelDto
                {
                    EmployeeName =
                        g.Key.EmployeeName,

                    TotalScore =
                        score,

                    FinalRate =
                        CalculateFinalRate(score),

                    Status =
                        GetPeriodStatus(g)
                };
            })
            .OrderByDescending(x => x.TotalScore)
            .ThenBy(x => x.EmployeeName)
            .ToList();
    }


    // =========================================================
    // Score Helpers
    // =========================================================

    private static decimal CalculatePeriodScore(
        IEnumerable<EmployeeEvaluationQueryModel> source)
    {
        return Math.Round(
            source.Average(x => x.TotalScore),
            2);
    }


    private static EmployeeEvaluationStatus
        GetPeriodStatus(
            IEnumerable<EmployeeEvaluationQueryModel> source)
    {
        if (source.Any(x =>
                x.Status ==
                EmployeeEvaluationStatus.Approved))
        {
            return EmployeeEvaluationStatus.Approved;
        }

        return EmployeeEvaluationStatus.Submitted;
    }


    private static FinalRate CalculateFinalRate(
        decimal score)
    {
        return score switch
        {
            >= 90 => FinalRate.Excellent,
            >= 80 => FinalRate.VeryGood,
            >= 70 => FinalRate.Good,
            >= 60 => FinalRate.Acceptable,
            _ => FinalRate.Poor
        };
    }


    // =========================================================
    // Period Helpers
    // =========================================================

    private static int GetPeriodGroupKey(
        EmployeeEvaluationQueryModel item,
        EvaluationReportPeriod period)
    {
        return period switch
        {
            EvaluationReportPeriod.Monthly =>
                item.Month,

            EvaluationReportPeriod.Quarterly =>
                GetQuarter(item.Month),

            EvaluationReportPeriod.HalfYearly =>
                item.Month <= 6 ? 1 : 2,

            EvaluationReportPeriod.Yearly =>
                1,

            _ => 1
        };
    }


    private static int GetQuarter(
        int month)
    {
        return ((month - 1) / 3) + 1;
    }


    private static string BuildPeriodLabel(
        int year,
        EvaluationReportPeriod period,
        int? month,
        int? quarter)
    {
        return period switch
        {
            EvaluationReportPeriod.Monthly =>
                month.HasValue
                    ? $"{GetMonthName(month.Value)} {year}"
                    : year.ToString(),

            EvaluationReportPeriod.Quarterly =>
                quarter.HasValue
                    ? $"Q{quarter.Value} {year}"
                    : year.ToString(),

            EvaluationReportPeriod.HalfYearly =>
                year.ToString(),

            EvaluationReportPeriod.Yearly =>
                year.ToString(),

            _ => year.ToString()
        };
    }


    private static string BuildRowPeriodLabel(
        int year,
        int month,
        EvaluationReportPeriod period)
    {
        return period switch
        {
            EvaluationReportPeriod.Monthly =>
                $"{GetMonthName(month)} {year}",

            EvaluationReportPeriod.Quarterly =>
                $"Q{GetQuarter(month)} {year}",

            EvaluationReportPeriod.HalfYearly =>
                month <= 6
                    ? $"H1 {year}"
                    : $"H2 {year}",

            EvaluationReportPeriod.Yearly =>
                year.ToString(),

            _ => year.ToString()
        };
    }


    private static string BuildGroupedPeriodLabel(
        IEnumerable<EmployeeEvaluationQueryModel> group,
        EvaluationReportPeriod period)
    {
        var first =
            group.First();

        return period switch
        {
            EvaluationReportPeriod.Monthly =>
                $"{GetMonthName(first.Month)} {first.Year}",

            EvaluationReportPeriod.Quarterly =>
                $"Q{GetQuarter(first.Month)} {first.Year}",

            EvaluationReportPeriod.HalfYearly =>
                first.Month <= 6
                    ? $"H1 {first.Year}"
                    : $"H2 {first.Year}",

            EvaluationReportPeriod.Yearly =>
                first.Year.ToString(),

            _ => first.Year.ToString()
        };
    }


    // =========================================================
    // Header Queries
    // =========================================================

    private async Task<EmployeeHeaderQueryModel?>
        GetEmployeeHeaderAsync(
            int employeeId,
            CancellationToken ct)
    {
        return await _context.Employees
            .AsNoTracking()
            .Where(x =>
                x.Id == employeeId)
            .Select(x =>
                new EmployeeHeaderQueryModel
                {
                    EmployeeName =
                        x.User.FullName,

                    DepartmentName =
                        x.Department.NameEn
                })
            .FirstOrDefaultAsync(ct);
    }


    private async Task<DepartmentHeaderQueryModel?>
        GetDepartmentHeaderAsync(
            int departmentId,
            CancellationToken ct)
    {
        return await _context.Departments
            .AsNoTracking()
            .Where(x =>
                x.Id == departmentId)
            .Select(x =>
                new DepartmentHeaderQueryModel
                {
                    DepartmentName =
                        x.NameEn,

                    ManagerName =
                        _context.Employees
                            .Where(u =>
                                u.Id == x.ManagerEmployeeId)
                            .Select(u =>
                                u.User.FullName)
                            .FirstOrDefault()
                })
            .FirstOrDefaultAsync(ct);
    }


    // =========================================================
    // Validation Helpers
    // =========================================================

    private static Result<bool>
        ValidateYearMonth(
            int year,
            int month)
    {
        if (year < 1)
        {
            return Result<bool>.Failure(
                "Invalid year.");
        }

        if (month < 1 ||
            month > 12)
        {
            return Result<bool>.Failure(
                "Month must be between 1 and 12.");
        }

        return Result<bool>
            .Succeeded(true);
    }


    private static Result<bool>
      ValidatePeriodFilter(
          int year,
          EvaluationReportPeriod period,
          int? month,
          int? quarter)
    {
        // =========================================================
        // Year
        // =========================================================

        if (year <= 0)
        {
            return Result<bool>.Failure(
                "Year must be greater than zero.");
        }


        // =========================================================
        // Period
        // =========================================================

        if (!Enum.IsDefined(
            typeof(EvaluationReportPeriod),
            period))
        {
            return Result<bool>.Failure(
                "Invalid evaluation report period.");
        }


        // =========================================================
        // Month
        // =========================================================

        if (month.HasValue &&
            (month.Value < 1 ||
             month.Value > 12))
        {
            return Result<bool>.Failure(
                "Month must be between 1 and 12.");
        }


        // =========================================================
        // Quarter
        // =========================================================

        if (quarter.HasValue &&
            (quarter.Value < 1 ||
             quarter.Value > 4))
        {
            return Result<bool>.Failure(
                "Quarter must be between 1 and 4.");
        }
        // =========================================================
        // Quarter requires Quarterly Period
        // =========================================================

        if (quarter.HasValue &&
            period != EvaluationReportPeriod.Quarterly)
        {
            return Result<bool>.Failure(
                "Quarter can only be used with quarterly reports.");
        }

        // =========================================================
        // Monthly
        // =========================================================

        if (period == EvaluationReportPeriod.Monthly)
        {
            if (!month.HasValue)
            {
                return Result<bool>.Failure(
                    "Month is required for monthly reports.");
            }

            if (quarter.HasValue)
            {
                return Result<bool>.Failure(
                    "Quarter must be empty for monthly reports.");
            }
        }


        // =========================================================
        // Quarterly
        // =========================================================

        if (period == EvaluationReportPeriod.Quarterly)
        {
            if (!quarter.HasValue)
            {
                return Result<bool>.Failure(
                    "Quarter is required for quarterly reports.");
            }

            if (month.HasValue)
            {
                return Result<bool>.Failure(
                    "Month must be empty for quarterly reports.");
            }
        }


        // =========================================================
        // Half Yearly
        // =========================================================

        if (period == EvaluationReportPeriod.HalfYearly)
        {
            if (month.HasValue)
            {
                return Result<bool>.Failure(
                    "Month cannot be used with half-yearly reports.");
            }

            if (quarter.HasValue)
            {
                return Result<bool>.Failure(
                    "Quarter cannot be used with half-yearly reports.");
            }
        }


        // =========================================================
        // Yearly
        // =========================================================

        if (period == EvaluationReportPeriod.Yearly)
        {
            if (month.HasValue)
            {
                return Result<bool>.Failure(
                    "Month cannot be used with yearly reports.");
            }

            if (quarter.HasValue)
            {
                return Result<bool>.Failure(
                    "Quarter cannot be used with yearly reports.");
            }
        }


        return Result<bool>.Succeeded(true);
    }

    private static Result<bool>
     ValidatePeriodFilter(
         int? year,
         EvaluationReportPeriod period,
         int? quarter,
         int? periodNumber)
    {
        // =========================================================
        // Year
        // =========================================================

        if (year.HasValue &&
            year.Value <= 0)
        {
            return Result<bool>.Failure(
                "Year must be greater than zero.");
        }

        // =========================================================
        // Period
        // =========================================================

        if (!Enum.IsDefined(
            typeof(EvaluationReportPeriod),
            period))
        {
            return Result<bool>.Failure(
                "Invalid evaluation report period.");
        }

        // =========================================================
        // Quarter
        // =========================================================

        if (quarter.HasValue &&
            (quarter.Value < 1 ||
             quarter.Value > 4))
        {
            return Result<bool>.Failure(
                "Quarter must be between 1 and 4.");
        }

        if (quarter.HasValue &&
            period != EvaluationReportPeriod.Quarterly)
        {
            return Result<bool>.Failure(
                "Quarter can only be used with quarterly reports.");
        }

        if (period == EvaluationReportPeriod.Quarterly &&
            !quarter.HasValue)
        {
            return Result<bool>.Failure(
                "Quarter is required for quarterly reports.");
        }

        // =========================================================
        // Half Yearly
        // =========================================================

        if (period == EvaluationReportPeriod.HalfYearly)
        {
            if (periodNumber.HasValue &&
                (periodNumber.Value < 1 ||
                 periodNumber.Value > 2))
            {
                return Result<bool>.Failure(
                    "Half-year number must be 1 or 2.");
            }
        }

        // =========================================================
        // Yearly
        // =========================================================

        if (period == EvaluationReportPeriod.Yearly)
        {
            if (periodNumber.HasValue &&
                periodNumber.Value != 1)
            {
                return Result<bool>.Failure(
                    "Yearly period number must be 1 or empty.");
            }
        }

        // =========================================================
        // Period Number Restrictions
        // =========================================================

        if (period != EvaluationReportPeriod.HalfYearly &&
            period != EvaluationReportPeriod.Yearly &&
            periodNumber.HasValue)
        {
            return Result<bool>.Failure(
                "Period number can only be used with half-yearly or yearly reports.");
        }

        return Result<bool>.Succeeded(true);
    }


    // =========================================================
    // Pagination Helpers
    // =========================================================

    private static int NormalizePageNumber(
        int pageNumber)
    {
        return pageNumber < 1
            ? 1
            : pageNumber;
    }


    private static int NormalizePageSize(
        int pageSize)
    {
        if (pageSize < 1)
        {
            return 20;
        }

        return Math.Min(
            pageSize,
            100);
    }


    // =========================================================
    // Mapping
    // =========================================================

    private EmployeeEvaluationDto
        MapEvaluationDto(
            EmployeeEvaluation evaluation)
    {
        return new EmployeeEvaluationDto
        {
            Id =
                evaluation.Id,

            EmployeeId =
                evaluation.EmployeeId,

            EmployeeName =
                evaluation.Employee.User.FullName,

            EvaluatorName =
                evaluation.Evaluator.FullName,

            PeriodId =
                evaluation.PeriodId,

            PeriodName =
                evaluation.Period.Name,

            Year =
                evaluation.Period.StartDate.Year,

            Month =
                evaluation.Period.StartDate.Month,

            TemplateId =
                evaluation.TemplateId,

            TemplateName =
                evaluation.Template.Name,

            TotalScore =
                evaluation.TotalScore,

            FinalRate =
                evaluation.FinalRate,

            RejectedByName =
                _context.Users
                    .Where(x =>
                        x.Id ==
                        evaluation.RejectedBy)
                    .Select(x =>
                        x.FullName)
                    .FirstOrDefault(),

            RejectedOn =
                evaluation.RejectedOn,

            RejectionReason =
                evaluation.RejectionReason,

            ApprovedByName =
                _context.Users
                    .Where(x =>
                        x.Id ==
                        evaluation.ApprovedBy)
                    .Select(x =>
                        x.FullName)
                    .FirstOrDefault(),

            ApprovedOn =
                evaluation.ApprovedOn,

            Status =
                evaluation.Status,

            Items =
                evaluation.Items
                    .OrderBy(x =>
                        x.Criterion.SortOrder)
                    .Select(x =>
                        new EmployeeEvaluationItemDto
                        {
                            Id =
                                x.Id,

                            CriterionId =
                                x.CriterionId,

                            CriterionName =
                                x.Criterion.Name,

                            MaxScore =
                                x.Criterion.MaxScore,

                            Weight =
                                x.Criterion.Weight,

                            Score =
                                x.Score,

                            WeightedScore =
                                x.Criterion.MaxScore == 0
                                    ? 0
                                    : (x.Score /
                                       x.Criterion.MaxScore)
                                      * x.Criterion.Weight,

                            Notes =
                                x.Notes
                        })
                    .ToList()
        };
    }


    // =========================================================
    // Month Name
    // =========================================================

    private static string GetMonthName(
        int month)
    {
        return new DateTime(
                2026,
                month,
                1)
            .ToString("MMMM");
    }


    // =========================================================
    // Internal Query Models
    // =========================================================

    private sealed class EmployeeEvaluationQueryModel
    {
        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; } = null!;

        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; } = null!;

        public int Year { get; set; }

        public int Month { get; set; }

        public string PeriodName { get; set; } = null!;

        public decimal TotalScore { get; set; }

        public FinalRate FinalRate { get; set; }

        public EmployeeEvaluationStatus Status { get; set; }
    }


    private sealed class EmployeeHeaderQueryModel
    {
        public string EmployeeName { get; set; } = null!;

        public string DepartmentName { get; set; } = null!;
    }


    private sealed class DepartmentHeaderQueryModel
    {
        public string DepartmentName { get; set; } = null!;

        public string? ManagerName { get; set; }
    }
}