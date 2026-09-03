using AutoMapper;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.Payrolls.EmployeeLoans.DTOs;
using MicroERP.Application.Features.Payrolls.EmployeeLoans.Interfaces;
using MicroERP.Domin.Entities.Payrolls;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace MicroERP.Application.Features.Payrolls.EmployeeLoans.Services;

public class EmployeeLoanService : IEmployeeLoanService
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IAuditService _auditService;

    public EmployeeLoanService(
        IApplicationDbContext context,
        IMapper mapper,
        IAuditService auditService)
    {
        _context = context;
        _mapper = mapper;
        _auditService = auditService;
    }

    public async Task<Result<EmployeeLoanDto>> CreateAsync(
     CreateEmployeeLoanDto dto,
     CancellationToken cancellationToken = default)
    {
        // =========================================================
        // Employee
        // =========================================================

        var employeeExists =
            await _context.Employees
                .AnyAsync(
                    x => x.Id == dto.EmployeeId &&
                         x.IsActive,
                    cancellationToken);

        if (!employeeExists)
        {
            return Result<EmployeeLoanDto>.Failure(
                "Active employee not found.");
        }


        // =========================================================
        // Validation
        // =========================================================

        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            return Result<EmployeeLoanDto>.Failure(
                "Loan title is required.");
        }

        if (dto.TotalAmount <= 0)
        {
            return Result<EmployeeLoanDto>.Failure(
                "Total loan amount must be greater than zero.");
        }

        if (dto.InstallmentAmount <= 0)
        {
            return Result<EmployeeLoanDto>.Failure(
                "Installment amount must be greater than zero.");
        }

        if (dto.InstallmentAmount > dto.TotalAmount)
        {
            return Result<EmployeeLoanDto>.Failure(
                "Installment amount cannot be greater than total loan amount.");
        }


        // =========================================================
        // Calculate Installments
        // =========================================================

        var newStartDate =
            new DateOnly(
                dto.StartDate.Year,
                dto.StartDate.Month,
                1);

        var numberOfInstallments =
            (int)Math.Ceiling(
                dto.TotalAmount /
                dto.InstallmentAmount);

        if (numberOfInstallments <= 0)
        {
            return Result<EmployeeLoanDto>.Failure(
                "Invalid number of installments.");
        }

        var newEndDate =
            newStartDate.AddMonths(
                numberOfInstallments - 1);


        // =========================================================
        // Check Overlapping Loan
        // =========================================================

        var hasOverlappingLoan =
            await _context.EmployeeLoans
                .AsNoTracking()
                .AnyAsync(
                    x =>
                        x.EmployeeId == dto.EmployeeId &&
                        x.Status == LoanStatus.Active &&
                        x.Installments.Any(
                            i =>
                                i.DueDate >= newStartDate &&
                                i.DueDate <= newEndDate),
                    cancellationToken);

        if (hasOverlappingLoan)
        {
            return Result<EmployeeLoanDto>.Failure(
                "Employee already has an active loan with overlapping installments.");
        }


        // =========================================================
        // Transaction
        // =========================================================

        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            // =====================================================
            // Create Loan
            // =====================================================

            var loan =
                new EmployeeLoan
                {
                    EmployeeId =
                        dto.EmployeeId,

                    Title =
                        dto.Title.Trim(),

                    Notes =
                        string.IsNullOrWhiteSpace(dto.Notes)
                            ? null
                            : dto.Notes.Trim(),

                    TotalAmount =
                        decimal.Round(
                            dto.TotalAmount,
                            2,
                            MidpointRounding.AwayFromZero),

                    InstallmentAmount =
                        decimal.Round(
                            dto.InstallmentAmount,
                            2,
                            MidpointRounding.AwayFromZero),

                    NumberOfInstallments =
                        numberOfInstallments,

                    PaidInstallments =
                        0,

                    PaidAmount =
                        0,

                    RemainingAmount =
                        decimal.Round(
                            dto.TotalAmount,
                            2,
                            MidpointRounding.AwayFromZero),

                    StartDate =
                        newStartDate,

                    Status =
                        LoanStatus.Active
                };


            await _context.EmployeeLoans.AddAsync(
                loan,
                cancellationToken);


            // =====================================================
            // Generate Installments
            // =====================================================

            var remainingAmount =
                loan.TotalAmount;

            var installments =
                new List<EmployeeLoanInstallment>(
                    numberOfInstallments);


            for (var i = 1;
                 i <= numberOfInstallments;
                 i++)
            {
                var installmentAmount =
                    i == numberOfInstallments
                        ? remainingAmount
                        : Math.Min(
                            loan.InstallmentAmount,
                            remainingAmount);

                installmentAmount =
                    decimal.Round(
                        installmentAmount,
                        2,
                        MidpointRounding.AwayFromZero);

                if (installmentAmount <= 0)
                    break;


                var dueDate =
                    newStartDate.AddMonths(i - 1);


                var installment =
                    new EmployeeLoanInstallment
                    {
                        EmployeeLoan =
                            loan,

                        InstallmentNumber =
                            i,

                        DueDate =
                            dueDate,

                        Amount =
                            installmentAmount,

                        Status =
                            LoanInstallmentStatus.Pending,

                        PayrollAdjustmentId =
                            null,

                        PaidAt =
                            null
                    };


                installments.Add(
                    installment);


                remainingAmount -=
                    installmentAmount;
            }


            // =====================================================
            // Validate Installments
            // =====================================================

            if (installments.Count == 0)
            {
                throw new InvalidOperationException(
                    "No loan installments were generated.");
            }


            var generatedTotal =
                decimal.Round(
                    installments.Sum(x => x.Amount),
                    2,
                    MidpointRounding.AwayFromZero);


            if (generatedTotal != loan.TotalAmount)
            {
                throw new InvalidOperationException(
                    "Generated installments total does not match loan total amount.");
            }


            loan.NumberOfInstallments =
                installments.Count;


            await _context.EmployeeLoanInstallments
                .AddRangeAsync(
                    installments,
                    cancellationToken);


            // =====================================================
            // Save
            // =====================================================

            await _context.SaveChangesAsync(
                cancellationToken);


            // =====================================================
            // Commit
            // =====================================================

            await transaction.CommitAsync(
                cancellationToken);


            // =====================================================
            // Audit
            // =====================================================

            await _auditService.LogAsync(
                "Create",
                "EmployeeLoan",
                loan.Id.ToString(),
                null,
                new
                {
                    loan.EmployeeId,
                    loan.Title,
                    loan.TotalAmount,
                    loan.InstallmentAmount,
                    loan.NumberOfInstallments,
                    loan.StartDate,
                    loan.Status
                });


            // =====================================================
            // Reload
            // =====================================================

            var result =
                await _context.EmployeeLoans
                    .AsNoTracking()
                    .Include(x => x.Employee)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Installments)
                    .FirstAsync(
                        x => x.Id == loan.Id,
                        cancellationToken);


            return Result<EmployeeLoanDto>.Succeeded(
                _mapper.Map<EmployeeLoanDto>(result));
        }
        catch
        {
            // لا نحاول Rollback بعد Commit
            if (transaction.GetDbTransaction().Connection != null)
            {
                await transaction.RollbackAsync(
                    cancellationToken);
            }

            throw;
        }
    }

    public async Task<Result<EmployeeLoanDto>> UpdateAsync(int id,
        UpdateEmployeeLoanDto dto,
        CancellationToken cancellationToken = default)
    {
        // =========================================================
        // Get Loan
        // =========================================================

        var loan =
            await _context.EmployeeLoans
                .Include(x => x.Employee)
                    .ThenInclude(x => x.User)
                .Include(x => x.Installments)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (loan == null)
        {
            return Result<EmployeeLoanDto>.Failure(
                "Employee loan not found.");
        }


        // =========================================================
        // Validate Status
        // =========================================================

        if (loan.Status == LoanStatus.Completed)
        {
            return Result<EmployeeLoanDto>.Failure(
                "Completed loan cannot be modified.");
        }

        if (loan.Status == LoanStatus.Cancelled)
        {
            return Result<EmployeeLoanDto>.Failure(
                "Cancelled loan cannot be modified.");
        }


        // =========================================================
        // Validation
        // =========================================================

        if (dto.Title != null &&
            string.IsNullOrWhiteSpace(dto.Title))
        {
            return Result<EmployeeLoanDto>.Failure(
                "Loan title cannot be empty.");
        }


        // =========================================================
        // Old Values
        // =========================================================

        var oldValues = new
        {
            loan.Title,
            loan.Notes,
            loan.TotalAmount,
            loan.InstallmentAmount,
            loan.NumberOfInstallments,
            loan.StartDate,
            loan.Status
        };


        // =========================================================
        // Partial Update
        // =========================================================

        if (dto.Title != null)
        {
            loan.Title =
                dto.Title.Trim();
        }


        if (dto.Notes != null)
        {
            loan.Notes =
                string.IsNullOrWhiteSpace(dto.Notes)
                    ? null
                    : dto.Notes.Trim();
        }


        // =========================================================
        // New Values
        // =========================================================

        var newValues = new
        {
            loan.Title,
            loan.Notes,
            loan.TotalAmount,
            loan.InstallmentAmount,
            loan.NumberOfInstallments,
            loan.StartDate,
            loan.Status
        };


        // =========================================================
        // Save
        // =========================================================

        await _context.SaveChangesAsync(
            cancellationToken);


        // =========================================================
        // Audit
        // =========================================================

        await _auditService.LogAsync(
            "Update",
            "EmployeeLoan",
            loan.Id.ToString(),
            oldValues,
            newValues);


        // =========================================================
        // Reload
        // =========================================================

        var result =
            await _context.EmployeeLoans
                .AsNoTracking()
                .Include(x => x.Employee)
                    .ThenInclude(x => x.User)
                .Include(x => x.Installments)
                .FirstAsync(
                    x => x.Id == id,
                    cancellationToken);


        return Result<EmployeeLoanDto>.Succeeded(
            _mapper.Map<EmployeeLoanDto>(result));
    }

    public async Task<Result<EmployeeLoanDto>> GetByIdAsync(
      int id,
      CancellationToken cancellationToken = default)
    {
        // =========================================================
        // Get Loan
        // =========================================================

        var loan =
       await _context.EmployeeLoans
           .AsNoTracking()

           .Include(x => x.Employee)
               .ThenInclude(x => x.User)

           .Include(x => x.CancelledByUser)

           .Include(x => x.SuspendedByUser)

           .Include(x => x.Installments)

           .FirstOrDefaultAsync(
               x => x.Id == id,
               cancellationToken);

        // =========================================================
        // Not Found
        // =========================================================

        if (loan == null)
        {
            return Result<EmployeeLoanDto>.Failure(
                "Employee loan not found.");
        }

        // =========================================================
        // Map
        // =========================================================

        var dto =
            _mapper.Map<EmployeeLoanDto>(
                loan);

        // =========================================================
        // Order Installments
        // =========================================================

        dto.Installments =
            dto.Installments
                .OrderBy(x => x.DueDate)
                .ThenBy(x => x.InstallmentNumber)
                .ToList();

        // =========================================================
        // Result
        // =========================================================

        return Result<EmployeeLoanDto>.Succeeded(
            dto);
    }

    public async Task<Result<PagedResult<EmployeeLoanDto>>> GetAllAsync(
      EmployeeLoanFilterDto filter,
      PagedRequest request,
      CancellationToken cancellationToken = default)
    {
        // =========================================================
        // Normalize Request
        // =========================================================

        var pageNumber =
            request.PageNumber <= 0
                ? 1
                : request.PageNumber;

        var pageSize =
            request.PageSize <= 0
                ? 20
                : Math.Min(request.PageSize, 100);


        // =========================================================
        // Validate Month / Year
        // =========================================================

        if (filter.StartMonth is < 1 or > 12)
        {
            return Result<PagedResult<EmployeeLoanDto>>.Failure(
                "Start month must be between 1 and 12.");
        }

        if (filter.InstallmentMonth is < 1 or > 12)
        {
            return Result<PagedResult<EmployeeLoanDto>>.Failure(
                "Installment month must be between 1 and 12.");
        }


        // =========================================================
        // Query
        // =========================================================

        var query =
        _context.EmployeeLoans
            .AsNoTracking()

            // Employee + User
            .Include(x => x.Employee)
                .ThenInclude(x => x.User)

            // Cancellation User
            .Include(x => x.CancelledByUser)

            // Suspension User
            .Include(x => x.SuspendedByUser)

            // Installments
            .Include(x => x.Installments)

            .AsQueryable();


        // =========================================================
        // Employee
        // =========================================================

        if (filter.EmployeeId.HasValue)
        {
            query =
                query.Where(x =>
                    x.EmployeeId ==
                    filter.EmployeeId.Value);
        }


        // =========================================================
        // Loan Status
        // =========================================================

        if (filter.Status.HasValue)
        {
            query =
                query.Where(x =>
                    x.Status ==
                    filter.Status.Value);
        }


        // =========================================================
        // Loan Title
        // =========================================================

        if (!string.IsNullOrWhiteSpace(filter.Title))
        {
            var title =
                filter.Title.Trim();

            query =
                query.Where(x =>
                    x.Title.Contains(title));
        }


        // =========================================================
        // Loan Start Year
        // =========================================================

        if (filter.StartYear.HasValue)
        {
            query =
                query.Where(x =>
                    x.StartDate.Year ==
                    filter.StartYear.Value);
        }


        // =========================================================
        // Loan Start Month
        // =========================================================

        if (filter.StartMonth.HasValue)
        {
            query =
                query.Where(x =>
                    x.StartDate.Month ==
                    filter.StartMonth.Value);
        }


        // =========================================================
        // Installment Year
        // =========================================================

        if (filter.InstallmentYear.HasValue)
        {
            query =
                query.Where(x =>
                    x.Installments.Any(i =>
                        i.DueDate.Year ==
                        filter.InstallmentYear.Value));
        }


        // =========================================================
        // Installment Month
        // =========================================================

        if (filter.InstallmentMonth.HasValue)
        {
            query =
                query.Where(x =>
                    x.Installments.Any(i =>
                        i.DueDate.Month ==
                        filter.InstallmentMonth.Value));
        }


        // =========================================================
        // Installment Status
        // =========================================================

        if (filter.InstallmentStatus.HasValue)
        {
            query =
                query.Where(x =>
                    x.Installments.Any(i =>
                        i.Status ==
                        filter.InstallmentStatus.Value));
        }


        // =========================================================
        // Total Count
        // =========================================================

        var totalCount =
            await query.CountAsync(
                cancellationToken);


        // =========================================================
        // Pagination
        // =========================================================

        var loans =
            await query
                .OrderByDescending(x => x.StartDate)
                .ThenByDescending(x => x.Id)
                .Skip(
                    (pageNumber - 1) *
                    pageSize)
                .Take(pageSize)
                .ToListAsync(
                    cancellationToken);


        // =========================================================
        // Map
        // =========================================================

        var items =
            _mapper.Map<List<EmployeeLoanDto>>(
                loans);


        // =========================================================
        // Result
        // =========================================================

        var result =
            new PagedResult<EmployeeLoanDto>
            {
                Items = items,

                TotalCount =
                    totalCount,

                PageNumber =
                    pageNumber,

                PageSize =
                    pageSize
            };


        return Result<PagedResult<EmployeeLoanDto>>.Succeeded(
            result);
    }


    public async Task<Result> CancelAsync(int id, string userId, string? reason,
     CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result.Failure(
                "User ID is required.");
        }


        // =========================================================
        // Get Loan
        // =========================================================

        var loan =
            await _context.EmployeeLoans
                .Include(x => x.Installments)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (loan == null)
        {
            return Result.Failure(
                "Employee loan not found.");
        }


        // =========================================================
        // Validate Status
        // =========================================================

        if (loan.Status == LoanStatus.Cancelled)
        {
            return Result.Failure(
                "Employee loan is already cancelled.");
        }

        if (loan.Status == LoanStatus.Completed)
        {
            return Result.Failure(
                "Completed loan cannot be cancelled.");
        }


        // =========================================================
        // Paid Installments
        // =========================================================

        var hasPaidInstallments =
            loan.Installments.Any(
                x => x.Status == LoanInstallmentStatus.Paid);

        if (hasPaidInstallments)
        {
            return Result.Failure(
                "Loan cannot be cancelled because it has paid installments.");
        }


        // =========================================================
        // Applied Installments
        // =========================================================

        var hasAppliedInstallments =
            loan.Installments.Any(
                x => x.Status == LoanInstallmentStatus.Applied);

        if (hasAppliedInstallments)
        {
            return Result.Failure(
                "Loan cannot be cancelled because one or more installments are already applied to payroll.");
        }


        // =========================================================
        // Transaction
        // =========================================================

        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            var oldValues = new
            {
                loan.Status,
                loan.PaidAmount,
                loan.PaidInstallments,
                loan.RemainingAmount
            };


            // =====================================================
            // Cancel Pending Installments
            // =====================================================

            foreach (var installment in loan.Installments)
            {
                if (installment.Status ==
                    LoanInstallmentStatus.Pending)
                {
                    installment.Status =
                        LoanInstallmentStatus.Cancelled;
                }
            }


            // =====================================================
            // Cancel Loan
            // =====================================================

            var cancelledAt = DateTime.UtcNow;

            loan.Status =
                LoanStatus.Cancelled;

            loan.CancelledAt =
                cancelledAt;

            loan.CancelledByUserId =
                userId;

            loan.CancellationReason =
                string.IsNullOrWhiteSpace(reason)
                    ? null
                    : reason.Trim();


            // =====================================================
            // Audit
            // =====================================================

            await _auditService.LogAsync(
                "Cancel",
                "EmployeeLoan",
                loan.Id.ToString(),
                oldValues,
                new
                {
                    loan.Status,
                    loan.CancelledAt,
                    loan.CancelledByUserId,
                    loan.CancellationReason
                });


            // =====================================================
            // Save
            // =====================================================

            await _context.SaveChangesAsync(
                cancellationToken);


            await transaction.CommitAsync(
                cancellationToken);


            return Result.Succeeded(
                "Employee loan cancelled successfully.");
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }
    }

    public async Task<Result> SuspendAsync(
     int id,
     string userId,
     string reason,
     CancellationToken cancellationToken = default)
    {
        // =========================================================
        // Validate User
        // =========================================================

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result.Failure(
                "User ID is required.");
        }


        // =========================================================
        // Validate Reason
        // =========================================================

        if (string.IsNullOrWhiteSpace(reason))
        {
            return Result.Failure(
                "Suspension reason is required.");
        }


        // =========================================================
        // Get Loan
        // =========================================================

        var loan =
            await _context.EmployeeLoans
                .Include(x => x.Installments)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (loan == null)
        {
            return Result.Failure(
                "Employee loan not found.");
        }


        // =========================================================
        // Validate Status
        // =========================================================

        if (loan.Status == LoanStatus.Suspended)
        {
            return Result.Failure(
                "Employee loan is already suspended.");
        }


        if (loan.Status == LoanStatus.Completed)
        {
            return Result.Failure(
                "Completed loan cannot be suspended.");
        }


        if (loan.Status == LoanStatus.Cancelled)
        {
            return Result.Failure(
                "Cancelled loan cannot be suspended.");
        }


        if (loan.Status != LoanStatus.Active)
        {
            return Result.Failure(
                "Only active loans can be suspended.");
        }


        // =========================================================
        // Check Applied Installments
        // =========================================================
        //
        // Applied means that the installment has already been
        // included in a generated payroll.
        //
        // We do not allow suspension after that point.
        //
        // =========================================================

        var hasAppliedInstallments =
            loan.Installments.Any(
                x =>
                    x.Status ==
                    LoanInstallmentStatus.Applied);


        if (hasAppliedInstallments)
        {
            return Result.Failure(
                "Loan cannot be suspended because one or more " +
                "installments are already applied to payroll.");
        }


        // =========================================================
        // Transaction
        // =========================================================

        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            // =====================================================
            // Old Values
            // =====================================================

            var oldValues = new
            {
                loan.Status,
                loan.SuspendedAt,
                loan.SuspendedByUserId,
                loan.SuspensionReason
            };


            // =====================================================
            // Suspend Loan
            // =====================================================

            var suspendedAt =
                DateTime.UtcNow;


            loan.Status =
                LoanStatus.Suspended;

            loan.SuspendedAt =
                suspendedAt;

            loan.SuspendedByUserId =
                userId;

            loan.SuspensionReason =
                reason.Trim();


            // =====================================================
            // Audit
            // =====================================================

            await _auditService.LogAsync(
                "Suspend",
                "EmployeeLoan",
                loan.Id.ToString(),
                oldValues,
                new
                {
                    loan.Status,
                    loan.SuspendedAt,
                    loan.SuspendedByUserId,
                    loan.SuspensionReason
                });


            // =====================================================
            // Save
            // =====================================================

            await _context.SaveChangesAsync(
                cancellationToken);


            // =====================================================
            // Commit
            // =====================================================

            await transaction.CommitAsync(
                cancellationToken);


            return Result.Succeeded(
                "Employee loan suspended successfully.");
        }
        catch
        {
            try
            {
                await transaction.RollbackAsync(
                    cancellationToken);
            }
            catch
            {
                // Do not hide the original exception.
            }

            throw;
        }
    }

    public async Task<Result> ResumeAsync(
       int id,
       CancellationToken cancellationToken = default)
    {
        // =========================================================
        // Get Loan
        // =========================================================

        var loan =
            await _context.EmployeeLoans
                .Include(x => x.Installments)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (loan == null)
        {
            return Result.Failure(
                "Employee loan not found.");
        }


        // =========================================================
        // Validate Status
        // =========================================================

        if (loan.Status == LoanStatus.Active)
        {
            return Result.Failure(
                "Employee loan is already active.");
        }

        if (loan.Status == LoanStatus.Completed)
        {
            return Result.Failure(
                "Completed loan cannot be resumed.");
        }

        if (loan.Status == LoanStatus.Cancelled)
        {
            return Result.Failure(
                "Cancelled loan cannot be resumed.");
        }

        if (loan.Status != LoanStatus.Suspended)
        {
            return Result.Failure(
                "Only suspended loans can be resumed.");
        }


        // =========================================================
        // Get Skipped Installments
        // =========================================================

        var skippedInstallments =
            loan.Installments
                .Where(x =>
                    x.Status ==
                    LoanInstallmentStatus.Skipped)
                .OrderBy(x => x.InstallmentNumber)
                .ToList();


        // =========================================================
        // No Skipped Installments
        // =========================================================

        if (skippedInstallments.Count == 0)
        {
            var oldStatus =
                loan.Status;

            loan.Status =
                LoanStatus.Active;


            await _auditService.LogAsync(
                "Resume",
                "EmployeeLoan",
                loan.Id.ToString(),
                new
                {
                    Status = oldStatus
                },
                new
                {
                    Status = loan.Status,
                    RescheduledInstallments = 0
                });


            await _context.SaveChangesAsync(
                cancellationToken);


            return Result.Succeeded(
                "Employee loan resumed successfully.");
        }


        // =========================================================
        // Get Occupied Dates From Other Loans
        // =========================================================

        var occupiedDates =
            await _context.EmployeeLoanInstallments
                .AsNoTracking()
                .Where(x =>
                    x.EmployeeLoan.EmployeeId ==
                        loan.EmployeeId &&

                    x.EmployeeLoanId !=
                        loan.Id &&

                    x.EmployeeLoan.Status !=
                        LoanStatus.Cancelled &&

                    (
                        x.Status ==
                            LoanInstallmentStatus.Pending ||

                        x.Status ==
                            LoanInstallmentStatus.Applied ||
                        x.Status ==
                            LoanInstallmentStatus.Skipped ||

                        x.Status ==
                            LoanInstallmentStatus.Paid
                    ))
                .Select(x => x.DueDate)
                .ToListAsync(
                    cancellationToken);


        // =========================================================
        // Used Months
        // =========================================================

        var usedMonths =
            occupiedDates
                .Select(x =>
                    new DateOnly(
                        x.Year,
                        x.Month,
                        1))
                .ToHashSet();


        // =========================================================
        // Find Last Scheduled Installment
        // =========================================================
        //
        // IMPORTANT:
        // Skipped installments are historical records.
        // They must NOT be reused or considered scheduled.
        //
        // Replacement installments will start after the last
        // currently scheduled installment.
        // =========================================================

        var lastScheduledInstallment =
            loan.Installments
                .Where(x =>
                    x.Status !=
                        LoanInstallmentStatus.Skipped &&

                    x.Status !=
                        LoanInstallmentStatus.Cancelled)
                .OrderByDescending(x => x.DueDate)
                .FirstOrDefault();


        DateOnly candidateDate;


        if (lastScheduledInstallment != null)
        {
            candidateDate =
                new DateOnly(
                    lastScheduledInstallment.DueDate.Year,
                    lastScheduledInstallment.DueDate.Month,
                    1)
                .AddMonths(1);
        }
        else
        {
            candidateDate =
                new DateOnly(
                    loan.StartDate.Year,
                    loan.StartDate.Month,
                    1)
                .AddMonths(1);
        }


        // =========================================================
        // Next Installment Number
        // =========================================================
        //
        // DO NOT reuse the number of the skipped installment.
        //
        // Example:
        //
        // #1 Skipped
        // #2 Pending
        // #3 Pending
        //
        // Replacement:
        //
        // #4 Pending
        //
        // This preserves the history and avoids the unique index
        // violation on:
        //
        // EmployeeLoanId + InstallmentNumber
        // =========================================================

        var nextInstallmentNumber =
            loan.Installments
                .Select(x => x.InstallmentNumber)
                .DefaultIfEmpty(0)
                .Max() + 1;


        // =========================================================
        // Old Values
        // =========================================================

        var oldValues = new
        {
            Status =
                loan.Status,

            SkippedInstallments =
                skippedInstallments
                    .Select(x => new
                    {
                        x.Id,
                        x.InstallmentNumber,
                        x.DueDate,
                        x.Amount,
                        x.Status
                    })
                    .ToList()
        };


        // =========================================================
        // Transaction
        // =========================================================

        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            var newInstallments =
                new List<EmployeeLoanInstallment>();


            // =====================================================
            // Create Replacement Installments
            // =====================================================

            foreach (var skippedInstallment in
                     skippedInstallments)
            {
                // -------------------------------------------------
                // Find First Available Month
                // -------------------------------------------------

                while (usedMonths.Contains(
                    candidateDate))
                {
                    candidateDate =
                        candidateDate.AddMonths(1);
                }


                // -------------------------------------------------
                // Generate New Installment Number
                // -------------------------------------------------

                var replacementNumber =
                    nextInstallmentNumber++;


                // -------------------------------------------------
                // Create Replacement Installment
                // -------------------------------------------------

                var newInstallment =
                    new EmployeeLoanInstallment
                    {
                        EmployeeLoanId =
                            loan.Id,

                        InstallmentNumber =
                            replacementNumber,

                        DueDate =
                            candidateDate,

                        Amount =
                            skippedInstallment.Amount,

                        Status =
                            LoanInstallmentStatus.Pending,

                        PayrollAdjustmentId =
                            null,

                        PaidAt =
                            null,

                        RescheduledFromInstallmentId =
                            skippedInstallment.Id,

                        Notes =
                            $"Replacement installment " +
                            $"#{replacementNumber} " +
                            $"for skipped installment " +
                            $"#{skippedInstallment.InstallmentNumber} " +
                            $"originally due in " +
                            $"{skippedInstallment.DueDate:MM/yyyy} " +
                            $"for loan '{loan.Title}'."
                    };


                newInstallments.Add(
                    newInstallment);


                // -------------------------------------------------
                // Reserve Month
                // -------------------------------------------------

                usedMonths.Add(
                    candidateDate);


                // -------------------------------------------------
                // Move To Next Month
                // -------------------------------------------------

                candidateDate =
                    candidateDate.AddMonths(1);
            }


            // =====================================================
            // Add New Installments
            // =====================================================

            await _context.EmployeeLoanInstallments.AddRangeAsync(
                newInstallments,
                cancellationToken);


            // =====================================================
            // Resume Loan
            // =====================================================

            loan.Status =
                LoanStatus.Active;


            // =====================================================
            // Audit
            // =====================================================

            var newValues = new
            {
                Status =
                    loan.Status,

                RescheduledInstallments =
                    newInstallments
                        .Select(x => new
                        {
                            x.Id,
                            x.InstallmentNumber,
                            x.DueDate,
                            x.Amount,
                            x.Status,
                            x.RescheduledFromInstallmentId,
                            x.Notes
                        })
                        .ToList()
            };


            await _auditService.LogAsync(
                "Resume",
                "EmployeeLoan",
                loan.Id.ToString(),
                oldValues,
                newValues);


            // =====================================================
            // Save
            // =====================================================

            await _context.SaveChangesAsync(
                cancellationToken);


            // =====================================================
            // Commit
            // =====================================================

            await transaction.CommitAsync(
                cancellationToken);


            return Result.Succeeded(
                $"Employee loan resumed successfully. " +
                $"Created {newInstallments.Count} " +
                $"replacement installments.");
        }
        catch
        {
            try
            {
                await transaction.RollbackAsync(
                    cancellationToken);
            }
            catch
            {
                // Do not hide the original exception.
            }

            throw;
        }
    }


}
