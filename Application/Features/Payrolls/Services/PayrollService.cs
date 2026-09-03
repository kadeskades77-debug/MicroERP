
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using MicroERP.Application.Features.Payrolls.DTOs;
using MicroERP.Application.Features.Payrolls.Interfaces;
using MicroERP.Application.Features.Payrolls.SalaryComponents.Services;
using MicroERP.Application.Features.Payrolls.Services.Calculators;
using MicroERP.Domin.Entities.EmployeeAttendance;
using MicroERP.Domin.Entities.Payrolls;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using static HRPermissions;
using Payroll = MicroERP.Domin.Entities.Payrolls.Payroll;

namespace MicroERP.Application.Features.Payrolls.Services
{
    public class PayrollService : IPayrollService
    {
        private readonly IApplicationDbContext _context;
        private readonly IPayrollCalculator _calculator;
        private readonly IAttendancePayrollCalculator _attendancePayrollCalculator;
        private readonly IUnpaidLeavePayrollCalculator  _unpaidLeavePayrollCalculator;
        private readonly IAuditService  _auditService;
        private readonly IOvertimeCalculationService _calculationService;
        public PayrollService(IApplicationDbContext context, IPayrollCalculator calculator, IAttendancePayrollCalculator attendancePayrollCalculator, IUnpaidLeavePayrollCalculator unpaidLeavePayrollCalculator, IAuditService auditService, IOvertimeCalculationService calculationService)
        {
            _context = context;
            _calculator = calculator;
            _attendancePayrollCalculator = attendancePayrollCalculator;
            _unpaidLeavePayrollCalculator = unpaidLeavePayrollCalculator;
            _auditService = auditService;
            _calculationService = calculationService;
        }



        public async Task<Result<PayrollPeriodDto>> CreatePeriodAsync(
            CreatePayrollPeriodDto dto,
            CancellationToken cancellationToken)
        {

            var exists = await _context.PayrollPeriods
                .AnyAsync(x =>
                    x.Year == dto.Year &&
                    x.Month == dto.Month,
                    cancellationToken);


            if (exists)
            {
                return Result<PayrollPeriodDto>
                    .Failure("Payroll period already exists");
            }


            var start = new DateOnly(
                dto.Year,
                dto.Month,
                1);


            var end = start
                .AddMonths(1)
                .AddDays(-1);



            var period = new PayrollPeriod
            {
                Year = dto.Year,
                Month = dto.Month,
                StartDate = start,
                EndDate = end,
                Status = PayrollPeriodStatus.Open
            };


            await _context.PayrollPeriods.AddAsync(
                period,
                cancellationToken);


            await _context.SaveChangesAsync(
                cancellationToken);



            return Result<PayrollPeriodDto>.Succeeded(
                new PayrollPeriodDto
                {
                    Id = period.Id,
                    Year = period.Year,
                    Month = period.Month,
                    StartDate = period.StartDate,
                    EndDate = period.EndDate,
                    Status = period.Status
                });
        }

        public async Task<Result> UpdatePeriodAsync(int id,
         UpdatePayrollPeriodDto dto,
         CancellationToken cancellationToken = default)
        {
            var period =
                await _context.PayrollPeriods
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);


            if (period == null)
            {
                return Result.Failure(
                    "Payroll period not found.");
            }



            if (period.Status == PayrollPeriodStatus.Closed)
            {
                return Result.Failure(
                    "Closed payroll period cannot be updated.");
            }



            var oldValues = new
            {
                period.Year,
                period.Month,
                period.StartDate,
                period.EndDate,
                period.Status
            };



            if (dto.StartDate.HasValue)
            {
                period.StartDate =
                    dto.StartDate.Value;
            }


            if (dto.EndDate.HasValue)
            {
                period.EndDate =
                    dto.EndDate.Value;
            }



            if (period.EndDate < period.StartDate)
            {
                return Result.Failure(
                    "End date cannot be before start date.");
            }



            var newValues = new
            {
                period.Year,
                period.Month,
                period.StartDate,
                period.EndDate,
                period.Status
            };

            await _auditService.LogAsync(
               "Update",
               "PayrollPeriod",
               period.Id.ToString(),
               oldValues,
               newValues);

            await _context.SaveChangesAsync(
                cancellationToken);



           



            return Result.Succeeded(
                "Payroll period updated successfully.");
        }

        public async Task<Result> DeletePeriodAsync(int id,
        CancellationToken cancellationToken = default)
        {
            var period =
                await _context.PayrollPeriods
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);



            if (period == null)
            {
                return Result.Failure(
                    "Payroll period not found.");
            }



            if (period.Status != PayrollPeriodStatus.Open)
            {
                return Result.Failure(
                    "Only Open payroll periods can be deleted.");
            }



            var hasPayrolls =
                await _context.Payrolls
                .AnyAsync(
                    x => x.PayrollPeriodId == id,
                    cancellationToken);



            if (hasPayrolls)
            {
                return Result.Failure(
                    "Payroll period contains payroll records and cannot be deleted.");
            }



            var oldValues = new
            {
                period.Year,
                period.Month,
                period.StartDate,
                period.EndDate,
                period.Status
            };



            period.IsDeleted = true;

            period.IsActive = false;



            await _context.SaveChangesAsync(
                cancellationToken);



            await _auditService.LogAsync(
                "Delete",
                "PayrollPeriod",
                period.Id.ToString(),
                oldValues,
                null);



            return Result.Succeeded(
                "Payroll period deleted successfully.");
        }

        public async Task<Result> GeneratePayrollAsync(int payrollPeriodId,
        CancellationToken cancellationToken)
        {
            var period =
                await _context.PayrollPeriods
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x => x.Id == payrollPeriodId,
                        cancellationToken);

            if (period is null)
            {
                return Result.Failure(
                    "Payroll period was not found.");
            }


            if (period.Status == PayrollPeriodStatus.Closed)
            {
                return Result.Failure(
                    "Payroll period is already closed.");
            }


            if (period.StartDate > period.EndDate)
            {
                return Result.Failure(
                    "Payroll period dates are invalid.");
            }

            var salaryComponents =
                await _context.SalaryComponents
                    .AsNoTracking()
                    .ToDictionaryAsync(
                        x => x.Id,
                        cancellationToken);


            var employees =
                await _context.Employees
                    .Where(x => x.IsActive)
                    .ToListAsync(
                        cancellationToken);


            if (employees.Count == 0)
            {
                return Result.Failure(
                    "No active employees were found.");
            }


            var generatedCount = 0;
            var skippedCount = 0;


            var overtimeLinks =
     new List<OvertimePayrollItemLink>();

            var appliedAdjustments =
                new List<PayrollAdjustment>();

            var appliedLoanInstallments =
                new List<EmployeeLoanInstallment>();

            foreach (var employee in employees)
            {
                // =========================================================
                // Existing Payroll
                // =========================================================

                var existingPayroll =
                    await _context.Payrolls
                        .Include(x => x.PayrollItems)
                        .FirstOrDefaultAsync(
                            x =>
                                x.EmployeeId == employee.Id &&
                                x.PayrollPeriodId ==
                                    payrollPeriodId,
                            cancellationToken);


                if (existingPayroll != null)
                {
                    if (existingPayroll.Status == PayrollStatus.Paid)
                    {
                        skippedCount++;
                        continue;
                    }

                    await ResetPayrollAsync(
                        existingPayroll,
                        period,
                        cancellationToken);
                }


                // =========================================================
                // Bank Account Validation
                // =========================================================

                var bankAccount =
                    await _context.EmployeeBankAccounts
                        .AsNoTracking()
                        .FirstOrDefaultAsync(
                            x =>
                                x.EmployeeId == employee.Id &&
                                x.IsPrimary &&
                                x.IsActive,
                            cancellationToken);


                if (bankAccount == null)
                {
                    return Result.Failure(
                        $"Employee '{employee.Id}' does not have " +
                        "an active primary bank account.");
                }


                if (string.IsNullOrWhiteSpace(
                        bankAccount.BankName))
                {
                    return Result.Failure(
                        $"Employee '{employee.Id}' primary bank account " +
                        "does not have a bank name.");
                }


                if (string.IsNullOrWhiteSpace(
                        bankAccount.AccountNumber) &&
                    string.IsNullOrWhiteSpace(
                        bankAccount.IBAN))
                {
                    return Result.Failure(
                        $"Employee '{employee.Id}' primary bank account " +
                        "must contain an account number or IBAN.");
                }


                // =========================================================
                // Employee Salary Components
                // =========================================================

                var employeeComponents =
                    await GetEmployeeComponentsAsync(
                        employee.Id,
                        period.StartDate,
                        period.EndDate,
                        cancellationToken);


                if (!employeeComponents.Any())
                {
                    skippedCount++;
                    continue;
                }


                // =========================================================
                // Base Payroll Calculation
                // =========================================================

                var calculation =
                    _calculator.Calculate(
                        employeeComponents);


                // =========================================================
                // Attendance
                // =========================================================

                await ApplyAttendanceAsync(
                    calculation,
                    employee.Id,
                    period,
                    salaryComponents,
                    cancellationToken);


                // =========================================================
                // Suspended Loans
                // =========================================================

                var skippedLoanInstallments =
                    await SkipSuspendedLoanInstallmentsAsync(
                        employee.Id,
                        payrollPeriodId,
                        cancellationToken);


                // =========================================================
                // Active Loans
                // =========================================================

                var employeeLoanInstallments =
                 await ApplyLoanInstallmentsAsync(
                     employee.Id,
                     payrollPeriodId,
                     cancellationToken);

                appliedLoanInstallments.AddRange(
                    employeeLoanInstallments);


                // =========================================================
                // Adjustments
                // =========================================================

                var employeeAdjustments =
                    await ApplyAdjustmentsAsync(
                        calculation,
                        employee.Id,
                        payrollPeriodId,
                        employeeLoanInstallments,
                        cancellationToken);


                appliedAdjustments.AddRange(
                    employeeAdjustments);

                // =========================================================
                // Overtime
                // =========================================================

                var employeeOvertimeLinks =
                    await ApplyOvertimeAsync(
                        calculation,
                        employee.Id,
                        period.StartDate,
                        period.EndDate,
                        cancellationToken);


                overtimeLinks.AddRange(
                    employeeOvertimeLinks);


                // =========================================================
                // Unpaid Leave
                // =========================================================

                await ApplyUnpaidLeaveAsync(
                    calculation,
                    employee.Id,
                    period.StartDate,
                    period.EndDate,
                    cancellationToken);


                // =========================================================
                // Validate Payroll Items
                // =========================================================

                foreach (var item in calculation.Items)
                {
                    if (string.IsNullOrWhiteSpace(
                            item.ItemName))
                    {
                        throw new Exception(
                            $"Missing ItemName | " +
                            $"EmployeeId={employee.Id} | " +
                            $"SalaryComponentId=" +
                            $"{item.SalaryComponentId} | " +
                            $"Amount={item.Amount} | " +
                            $"Source={item.Source}");
                    }


                    if (item.Amount < 0)
                    {
                        throw new Exception(
                            $"Invalid PayrollItem amount | " +
                            $"EmployeeId={employee.Id} | " +
                            $"ItemName={item.ItemName} | " +
                            $"Amount={item.Amount}");
                    }
                }


                // =========================================================
                // Validate Salary
                // =========================================================

                if (calculation.GrossSalary < 0)
                {
                    return Result.Failure(
                        $"Invalid gross salary for employee " +
                        $"{employee.Id}.");
                }


                if (calculation.TotalAllowances < 0)
                {
                    return Result.Failure(
                        $"Invalid total allowances for employee " +
                        $"{employee.Id}.");
                }


                if (calculation.TotalOvertime < 0)
                {
                    return Result.Failure(
                        $"Invalid total overtime for employee " +
                        $"{employee.Id}.");
                }


                if (calculation.TotalDeductions < 0)
                {
                    return Result.Failure(
                        $"Invalid total deductions for employee " +
                        $"{employee.Id}.");
                }


                // =========================================================
                // Net Salary
                // =========================================================

                // TotalOvertime is already included in
                // TotalAllowances by ApplyOvertimeAsync.
                //
                // Therefore DO NOT add TotalOvertime again here.

                calculation.NetSalary =
                    decimal.Round(
                        calculation.GrossSalary
                        + calculation.TotalOvertime
                        - calculation.TotalDeductions,
                        2,
                        MidpointRounding.AwayFromZero);


                if (calculation.NetSalary < 0)
                {
                    return Result.Failure(
                        $"Net salary cannot be negative for employee " +
                        $"{employee.Id}.");
                }


                // =========================================================
                // Create Payroll
                // =========================================================

                var payroll =
                    CreatePayroll(
                        employee.Id,
                        payrollPeriodId,
                        bankAccount,
                        calculation);


                await _context.Payrolls.AddAsync(
                    payroll,
                    cancellationToken);


                generatedCount++;
            }


            // =============================================================
            // Save Payrolls + PayrollItems
            // =============================================================

            await _context.SaveChangesAsync(
                cancellationToken);


            // =============================================================
            // Link EmployeeOvertime -> PayrollItem
            // =============================================================

            foreach (var link in overtimeLinks)
            {
                if (link.PayrollItem.Id <= 0)
                {
                    throw new Exception(
                        $"PayrollItem ID was not generated " +
                        $"for overtime.");
                }


                link.Overtime.PayrollItemId =
                    link.PayrollItem.Id;
            }
            foreach (var installment in appliedLoanInstallments)
            {
                installment.Status =
                    LoanInstallmentStatus.Applied;
            }

            foreach (var adjustment in appliedAdjustments)
            {
                adjustment.IsApplied = true;
            }

            await _context.SaveChangesAsync(cancellationToken);


            // =============================================================
            // Audit
            // =============================================================

            await _auditService.LogAsync(
                "Generate Payroll",
                "PayrollPeriod",
                payrollPeriodId.ToString(),
                null,
                new
                {
                    EmployeesCount =
                        generatedCount,

                    SkippedCount =
                        skippedCount,

                    Period =
                        $"{period.Month}/{period.Year}"
                });


            return Result.Succeeded(
                $"Payroll generation completed successfully. " +
                $"Generated: {generatedCount}, " +
                $"Skipped: {skippedCount}.");
        }

        public async Task<Result> ApproveAsync(
        int payrollId,
        string userId,
        CancellationToken cancellationToken)
        {
            var payroll =
                await _context.Payrolls
                    .FirstOrDefaultAsync(
                        x => x.Id == payrollId,
                        cancellationToken);

            if (payroll == null)
            {
                return Result.Failure(
                    "Payroll not found.");
            }

            if (payroll.Status != PayrollStatus.Calculated)
            {
                return Result.Failure(
                    "Only calculated payroll can be approved.");
            }

            var oldStatus = payroll.Status;

            payroll.Status = PayrollStatus.Approved;
            payroll.ApprovedByUserId = userId;
            payroll.ApprovedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            await _auditService.LogAsync(
                "Approve Payroll",
                "Payroll",
                payroll.Id.ToString(),
                new
                {
                    Status = oldStatus
                },
                new
                {
                    Status = payroll.Status
                });

            return Result.Succeeded(
                "Payroll approved successfully.");
        }

        public async Task<Result> ApprovePeriodAsync(
        int payrollPeriodId,
        string userId,
        CancellationToken cancellationToken)
        {
            var period =
                await _context.PayrollPeriods
                    .FirstOrDefaultAsync(
                        x => x.Id == payrollPeriodId,
                        cancellationToken);

            if (period == null)
            {
                return Result.Failure(
                    "Payroll period was not found.");
            }

            if (period.Status == PayrollPeriodStatus.Closed)
            {
                return Result.Failure(
                    "Payroll period is already closed.");
            }

            var payrolls =
                await _context.Payrolls
                    .Where(x =>
                        x.PayrollPeriodId == payrollPeriodId)
                    .ToListAsync(cancellationToken);

            if (payrolls.Count == 0)
            {
                return Result.Failure(
                    "No payrolls were found for this period.");
            }

            var invalidPayrolls =
                payrolls
                    .Where(x =>
                        x.Status != PayrollStatus.Calculated)
                    .ToList();

            if (invalidPayrolls.Count > 0)
            {
                return Result.Failure(
                    "All payrolls must be in Calculated status before approving the period.");
            }

            var approvedOn = DateTime.UtcNow;

            foreach (var payroll in payrolls)
            {
                payroll.Status =
                    PayrollStatus.Approved;

                payroll.ApprovedByUserId =
                    userId;

                payroll.ApprovedOn =
                    approvedOn;
            }

            await _context.SaveChangesAsync(
                cancellationToken);

            await _auditService.LogAsync(
                "Approve Payroll Period",
                "PayrollPeriod",
                payrollPeriodId.ToString(),
                new
                {
                    Status = PayrollStatus.Calculated,
                    PayrollsCount = payrolls.Count
                },
                new
                {
                    Status = PayrollStatus.Approved,
                    PayrollsCount = payrolls.Count,
                    ApprovedBy = userId,
                    ApprovedOn = approvedOn
                });

            return Result.Succeeded(
                $"Payroll period approved successfully. " +
                $"Approved: {payrolls.Count} payroll(s).");
        }

        public async Task<Result> MarkAsPaidAsync(
        int payrollId,
        string userId,
        CancellationToken cancellationToken)
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
            // Get Payroll
            // =========================================================

            var payroll =
                await _context.Payrolls
                    .FirstOrDefaultAsync(
                        x => x.Id == payrollId,
                        cancellationToken);

            if (payroll == null)
            {
                return Result.Failure(
                    "Payroll not found.");
            }


            // =========================================================
            // Validate Payroll Status
            // =========================================================

            if (payroll.Status != PayrollStatus.Approved)
            {
                return Result.Failure(
                    "Only approved payroll can be paid.");
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
                // Get Overtime Records
                // =====================================================

                var overtimes =
                    await _context.EmployeeOvertimes
                        .Where(x =>
                            x.PayrollItem != null &&
                            x.PayrollItem.PayrollId == payroll.Id &&
                            x.PayrollItem.Source ==
                                PayrollItemSource.Overtime &&
                            !x.IsPaid)
                        .ToListAsync(
                            cancellationToken);


                // =====================================================
                // Get Loan Installments
                // =====================================================

                var installments =
                    await _context.EmployeeLoanInstallments
                        .Include(x => x.EmployeeLoan)
                        .Include(x => x.PayrollAdjustment)
                        .Where(x =>
                            x.PayrollAdjustment != null &&
                            x.PayrollAdjustment.EmployeeId ==
                                payroll.EmployeeId &&
                            x.PayrollAdjustment.PayrollPeriodId ==
                                payroll.PayrollPeriodId &&
                            x.Status ==
                                LoanInstallmentStatus.Applied)
                        .ToListAsync(
                            cancellationToken);


                // =====================================================
                // Validate Loan Installments
                // =====================================================

                foreach (var installment in installments)
                {
                    if (installment.EmployeeLoan.Status ==
                        LoanStatus.Cancelled)
                    {
                        return Result.Failure(
                            $"Loan installment #{installment.InstallmentNumber} " +
                            "belongs to a cancelled loan.");
                    }
                }


                // =====================================================
                // Mark Payroll as Paid
                // =====================================================

                var paidOn =
                    DateTime.UtcNow;


                payroll.Status =
                    PayrollStatus.Paid;

                payroll.PaidByUserId =
                    userId;

                payroll.PaidOn =
                    paidOn;


                // =====================================================
                // Mark Overtime as Paid
                // =====================================================

                foreach (var overtime in overtimes)
                {
                    overtime.IsPaid = true;
                }


                // =====================================================
                // Mark Loan Installments as Paid
                // =====================================================

                var paidLoanInstallmentsCount = 0;

                foreach (var installment in installments)
                {
                    // -------------------------------------------------
                    // Prevent duplicate payment
                    // -------------------------------------------------

                    if (installment.Status ==
                        LoanInstallmentStatus.Paid)
                    {
                        continue;
                    }


                    installment.Status =
                        LoanInstallmentStatus.Paid;

                    installment.PaidAt =
                        paidOn;

                    paidLoanInstallmentsCount++;


                    // -------------------------------------------------
                    // Update Loan
                    // -------------------------------------------------

                    var loan =
                        installment.EmployeeLoan;

                    loan.PaidInstallments++;

                    loan.PaidAmount =
                        decimal.Round(
                            loan.PaidAmount +
                            installment.Amount,
                            2,
                            MidpointRounding.AwayFromZero);

                    loan.RemainingAmount =
                        decimal.Round(
                            loan.TotalAmount -
                            loan.PaidAmount,
                            2,
                            MidpointRounding.AwayFromZero);


                    // -------------------------------------------------
                    // Complete Loan
                    // -------------------------------------------------

                    if (loan.RemainingAmount <= 0)
                    {
                        loan.RemainingAmount = 0;

                        loan.PaidAmount =
                            loan.TotalAmount;

                        loan.PaidInstallments =
                            loan.NumberOfInstallments;

                        loan.Status =
                            LoanStatus.Completed;
                    }
                }
                // =====================================================
                // Save
                // =====================================================

                await _context.SaveChangesAsync(
                    cancellationToken);

                // =====================================================
                // Audit
                // =====================================================

                await _auditService.LogAsync(
                    "Pay Payroll",
                    "Payroll",
                    payroll.Id.ToString(),
                    new
                    {
                        Status =
                            PayrollStatus.Approved
                    },
                    new
                    {
                        Status =
                            PayrollStatus.Paid,

                        PaidOn =
                            paidOn,

                        OvertimeCount =
                            overtimes.Count,

                        LoanInstallmentsCount =
                            paidLoanInstallmentsCount
                    });


         


                // =====================================================
                // Commit
                // =====================================================

                await transaction.CommitAsync(
                    cancellationToken);


                return Result.Succeeded(
                    "Payroll paid successfully.");
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

        public async Task<Result> MarkPeriodAsPaidAsync(
        int payrollPeriodId,
        string userId,
        CancellationToken cancellationToken)
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
            // Get Payroll Period
            // =========================================================

            var period =
                await _context.PayrollPeriods
                    .FirstOrDefaultAsync(
                        x => x.Id == payrollPeriodId,
                        cancellationToken);

            if (period == null)
            {
                return Result.Failure(
                    "Payroll period was not found.");
            }


            // =========================================================
            // Get Payrolls
            // =========================================================

            var payrolls =
                await _context.Payrolls
                    .Where(x =>
                        x.PayrollPeriodId ==
                        payrollPeriodId)
                    .ToListAsync(
                        cancellationToken);

            if (payrolls.Count == 0)
            {
                return Result.Failure(
                    "No payrolls were found for this period.");
            }


            // =========================================================
            // Validate Payroll Status
            // =========================================================

            var invalidPayrolls =
                payrolls
                    .Where(x =>
                        x.Status != PayrollStatus.Approved)
                    .ToList();

            if (invalidPayrolls.Count > 0)
            {
                return Result.Failure(
                    "All payrolls must be approved before " +
                    "the payroll period can be paid.");
            }


            // =========================================================
            // Transaction
            // =========================================================

            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    cancellationToken);

            try
            {
                var paidOn =
                    DateTime.UtcNow;

                var totalOvertimeCount = 0;
                var totalPaidLoanInstallments = 0;


                // =====================================================
                // Process Payrolls
                // =====================================================

                foreach (var payroll in payrolls)
                {
                    // =================================================
                    // Get Overtime Records
                    // =================================================

                    var overtimes =
                        await _context.EmployeeOvertimes
                            .Where(x =>
                                x.PayrollItem != null &&
                                x.PayrollItem.PayrollId ==
                                    payroll.Id &&
                                x.PayrollItem.Source ==
                                    PayrollItemSource.Overtime &&
                                !x.IsPaid)
                            .ToListAsync(
                                cancellationToken);


                    totalOvertimeCount +=
                        overtimes.Count;


                    // =================================================
                    // Get Loan Installments
                    // =================================================

                    var installments =
                        await _context.EmployeeLoanInstallments
                            .Include(x => x.EmployeeLoan)
                            .Include(x => x.PayrollAdjustment)
                            .Where(x =>
                                x.PayrollAdjustment != null &&
                                x.PayrollAdjustment.EmployeeId ==
                                    payroll.EmployeeId &&
                                x.PayrollAdjustment.PayrollPeriodId ==
                                    payrollPeriodId &&
                                x.Status ==
                                    LoanInstallmentStatus.Applied)
                            .ToListAsync(
                                cancellationToken);


                    // =================================================
                    // Validate Loan Installments
                    // =================================================

                    foreach (var installment in installments)
                    {
                        if (installment.EmployeeLoan.Status ==
                            LoanStatus.Cancelled)
                        {
                            return Result.Failure(
                                $"Loan installment " +
                                $"#{installment.InstallmentNumber} " +
                                $"belongs to a cancelled loan " +
                                $"for employee " +
                                $"{payroll.EmployeeId}.");
                        }
                    }


                    // =================================================
                    // Mark Payroll as Paid
                    // =================================================

                    payroll.Status =
                        PayrollStatus.Paid;

                    payroll.PaidByUserId =
                        userId;

                    payroll.PaidOn =
                        paidOn;


                    // =================================================
                    // Mark Overtime as Paid
                    // =================================================

                    foreach (var overtime in overtimes)
                    {
                        overtime.IsPaid = true;
                    }


                    // =================================================
                    // Mark Loan Installments as Paid
                    // =================================================

                    foreach (var installment in installments)
                    {
                        // -------------------------------------------------
                        // Prevent duplicate payment
                        // -------------------------------------------------

                        if (installment.Status ==
                            LoanInstallmentStatus.Paid)
                        {
                            continue;
                        }


                        installment.Status =
                            LoanInstallmentStatus.Paid;

                        installment.PaidAt =
                            paidOn;

                        totalPaidLoanInstallments++;


                        // -------------------------------------------------
                        // Update Loan
                        // -------------------------------------------------

                        var loan =
                            installment.EmployeeLoan;

                        loan.PaidInstallments++;

                        loan.PaidAmount =
                            decimal.Round(
                                loan.PaidAmount +
                                installment.Amount,
                                2,
                                MidpointRounding.AwayFromZero);

                        loan.RemainingAmount =
                            decimal.Round(
                                loan.TotalAmount -
                                loan.PaidAmount,
                                2,
                                MidpointRounding.AwayFromZero);


                        // -------------------------------------------------
                        // Complete Loan
                        // -------------------------------------------------

                        if (loan.RemainingAmount <= 0)
                        {
                            loan.RemainingAmount = 0;

                            loan.PaidAmount =
                                loan.TotalAmount;

                            loan.PaidInstallments =
                                loan.NumberOfInstallments;

                            loan.Status =
                                LoanStatus.Completed;
                        }
                    }
                }


          


                // =========================================================
                // Save
                // =========================================================

                await _context.SaveChangesAsync(
                    cancellationToken);


                // =========================================================
                // Commit
                // =========================================================

                await transaction.CommitAsync(
                    cancellationToken);
                // =========================================================
                // Audit
                // =========================================================

                await _auditService.LogAsync(
                    "Pay Payroll Period",
                    "PayrollPeriod",
                    payrollPeriodId.ToString(),
                    new
                    {
                        Status =
                            PayrollStatus.Approved
                    },
                    new
                    {
                        Status =
                            PayrollStatus.Paid,

                        PaidOn =
                            paidOn,

                        PayrollsCount =
                            payrolls.Count,

                        OvertimeCount =
                            totalOvertimeCount,

                        LoanInstallmentsCount =
                            totalPaidLoanInstallments
                    });

                return Result.Succeeded(
                    $"Payroll period paid successfully. " +
                    $"Paid: {payrolls.Count} payroll(s).");
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

        public async Task<Result<bool>> ClosePeriodAsync(int periodId,
          CancellationToken cancellationToken)
        {
            var period =
                await _context.PayrollPeriods
                .FirstOrDefaultAsync(
                    x => x.Id == periodId,
                    cancellationToken);


            if (period == null)
                return Result<bool>.Failure(
                    "Payroll period not found");


            if (period.Status == PayrollPeriodStatus.Closed)
                return Result<bool>.Failure(
                    "Payroll period already closed");


            var allPaid =
                await _context.Payrolls
                .Where(x =>
                    x.PayrollPeriodId == periodId)
                .AllAsync(
                    x => x.Status == PayrollStatus.Paid,
                    cancellationToken);


            if (!allPaid)
                return Result<bool>.Failure(
                    "All payrolls must be paid before closing period");
            var oldValues = new
            {
                Status = period.Status
            };

            period.Status =
                PayrollPeriodStatus.Closed;

            period.ClosedOn = DateTime.Now;

            var newValues = new
            {
                Status = period.Status
            };
            await _context.SaveChangesAsync(
               cancellationToken);

            await _auditService.LogAsync(
                "Close",
                "PayrollPeriod",
                period.Id.ToString(),
                oldValues,
                newValues);

           


            return Result<bool>.Succeeded(true, "Closeed is Done");
        }

        //============= Helpers =================


        private async Task<List<EmployeeSalaryComponent>>
         GetEmployeeComponentsAsync(
        int employeeId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken)
        {
            return await _context.EmployeeSalaryComponents
                .Include(x => x.SalaryComponent)
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    x.IsActiveComponent &&
                    x.EffectiveFrom <= endDate &&
                    (
                        x.EffectiveTo == null ||
                        x.EffectiveTo >= startDate
                    ))
                .ToListAsync(cancellationToken);
        }

        private async Task ApplyAttendanceAsync(
       PayrollCalculationResult calculation,
       int employeeId,
       PayrollPeriod period,
       IReadOnlyDictionary<int, SalaryComponent> salaryComponents,
       CancellationToken cancellationToken)
        {
            var basicSalary= 
                await _calculationService.GetBasicSalaryAsync(employeeId,cancellationToken); 


            var attendanceItems =
                await _attendancePayrollCalculator
                    .CalculateAsync(
                        employeeId,
                        period.StartDate,
                        period.EndDate,
                        basicSalary,
                        cancellationToken);

            foreach (var item in attendanceItems)
            {
                calculation.Items.Add(item);

                if (!item.SalaryComponentId.HasValue)
                    continue;

                if (!salaryComponents.TryGetValue(
                        item.SalaryComponentId.Value,
                        out var component))
                    continue;

                if (component.Type == SalaryComponentType.Allowance)
                {
                    calculation.TotalAllowances += item.Amount;
                }
                else
                {
                    calculation.TotalDeductions += item.Amount;
                }
            }
        }

        private async Task<List<PayrollAdjustment>> ApplyAdjustmentsAsync(
         PayrollCalculationResult calculation,
         int employeeId,
         int payrollPeriodId,
         List<EmployeeLoanInstallment> loanInstallments,
         CancellationToken cancellationToken)
        {
            var adjustments =
                await _context.PayrollAdjustments
                    .Where(x =>
                        x.EmployeeId == employeeId &&
                        x.PayrollPeriodId == payrollPeriodId &&
                        !x.IsApplied)
                    .ToListAsync(cancellationToken);


            // =========================================================
            // Loan Adjustments
            // =========================================================

            foreach (var installment in loanInstallments)
            {
                var adjustment =
                    installment.PayrollAdjustment;

                if (adjustment == null)
                    continue;

                if (!adjustments.Contains(adjustment))
                {
                    adjustments.Add(adjustment);
                }
            }


            if (adjustments.Count == 0)
                return [];


            // =========================================================
            // Create Payroll Items
            // =========================================================

            foreach (var adjustment in adjustments)
            {
                calculation.Items.Add(
                    new PayrollItem
                    {
                        SalaryComponentId =
                            adjustment.SalaryComponentId,

                        Amount =
                            adjustment.Amount,

                        ItemName =
                            adjustment.Title,

                        Description =
                            adjustment.Notes,

                        Source =
                            PayrollItemSource.Adjustment,

                        Type =
                            adjustment.Type == AdjustmentType.Bonus
                                ? SalaryComponentType.Allowance
                                : SalaryComponentType.Deduction
                    });


                switch (adjustment.Type)
                {
                    case AdjustmentType.Bonus:

                        calculation.TotalAllowances +=
                            adjustment.Amount;

                        break;

                    case AdjustmentType.Deduction:

                        calculation.TotalDeductions +=
                            adjustment.Amount;

                        break;
                }
            }


            return adjustments;
        }


        private async Task<List<EmployeeLoanInstallment>>
       ApplyLoanInstallmentsAsync(
           int employeeId,
           int payrollPeriodId,
           CancellationToken cancellationToken)
        {
            // =========================================================
            // Get Payroll Period
            // =========================================================

            var period =
                await _context.PayrollPeriods
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x => x.Id == payrollPeriodId,
                        cancellationToken);

            if (period == null)
                return [];


            // =========================================================
            // Get Pending Installments
            // =========================================================
            //
            // Only:
            // - Same employee
            // - Active loan
            // - Current payroll period
            // - Pending installment
            //
            // =========================================================

            var installments =
                await _context.EmployeeLoanInstallments
                    .Include(x => x.EmployeeLoan)
                    .Include(x => x.PayrollAdjustment)
                    .Where(x =>
                        x.EmployeeLoan.EmployeeId == employeeId &&

                        x.EmployeeLoan.Status ==
                            LoanStatus.Active &&

                        x.DueDate >= period.StartDate &&
                        x.DueDate <= period.EndDate &&

                        x.Status ==
                            LoanInstallmentStatus.Pending)
                    .OrderBy(x => x.DueDate)
                    .ThenBy(x => x.InstallmentNumber)
                    .ToListAsync(
                        cancellationToken);


            if (installments.Count == 0)
                return [];


            // =========================================================
            // Create / Restore Payroll Adjustments
            // =========================================================

            foreach (var installment in installments)
            {
                PayrollAdjustment? adjustment =
                    installment.PayrollAdjustment;


                // =====================================================
                // Existing Adjustment
                // =====================================================

                if (adjustment != null)
                {
                    // It must belong to the current payroll period.
                    if (adjustment.PayrollPeriodId !=
                        payrollPeriodId)
                    {
                        continue;
                    }


                    // The adjustment is going to be included
                    // in the current payroll again.

                    adjustment.IsApplied = false;
                }


                // =====================================================
                // Create Adjustment
                // =====================================================

                else
                {
                    adjustment =
                        new PayrollAdjustment
                        {
                            EmployeeId =
                                employeeId,

                            PayrollPeriodId =
                                payrollPeriodId,

                            SalaryComponentId =
                                null,

                            Type =
                                AdjustmentType.Deduction,

                            Title =
                                $"{installment.EmployeeLoan.Title} - " +
                                $"Installment #{installment.InstallmentNumber}",

                            Notes =
                                $"Loan installment #{installment.InstallmentNumber}",

                            Amount =
                                installment.Amount,

                            IsApplied =
                                false
                        };


                    await _context.PayrollAdjustments.AddAsync(
                        adjustment,
                        cancellationToken);


                    installment.PayrollAdjustment =
                        adjustment;
                }


                // =====================================================
                // IMPORTANT
                // =====================================================
                //
                // DO NOT change:
                //
                // installment.Status = Applied;
                //
                // It remains Pending until payroll is successfully
                // generated and saved.
                //
            }


            // =========================================================
            // IMPORTANT
            // =========================================================
            //
            // No SaveChangesAsync here.
            //
            // The caller GeneratePayrollAsync will save:
            //
            // Payroll
            // PayrollItems
            // PayrollAdjustments
            // Loan Installments
            //
            // together.
            //
            // =========================================================

            return installments;
        }


        private async Task<List<OvertimePayrollItemLink>>
           ApplyOvertimeAsync(
           PayrollCalculationResult calculation,
           int employeeId,
           DateOnly startDate,
           DateOnly endDate,
           CancellationToken cancellationToken)
        {
            var overtimeComponentId =
                await _context.SalaryComponents
                    .Where(x =>
                        x.Code ==
                        SalaryComponentCodes.Overtime)
                    .Select(x => x.Id)
                    .FirstOrDefaultAsync(
                        cancellationToken);

            if (overtimeComponentId == 0)
            {
                throw new Exception(
                    "OVERTIME salary component not found.");
            }

            var startDateTime = startDate.ToDateTime(TimeOnly.MinValue);
            var endDateTime = endDate.AddDays(1).ToDateTime(TimeOnly.MinValue);

            var overtimes =
                await _context.EmployeeOvertimes
                    .Where(x =>
                        x.EmployeeId == employeeId &&
                        x.StartDateTime >= startDateTime &&
                        x.StartDateTime <= endDateTime &&
                        x.Status == OvertimeStatus.Approved &&
                        !x.IsPaid &&
                        x.PayrollItemId == null &&
                        x.Amount > 0)
                    .OrderBy(x => x.StartDateTime)
                    .ToListAsync(
                        cancellationToken);


            if (overtimes.Count == 0)
                return [];


            var links =
                new List<OvertimePayrollItemLink>();


            foreach (var overtime in overtimes)
            {
                var amount =
                    decimal.Round(
                        overtime.Amount,
                        2,
                        MidpointRounding.AwayFromZero);


                var payrollItem =
                    new PayrollItem
                    {
                        SalaryComponentId =
                            overtimeComponentId,

                        Amount =
                            amount,

                        ItemName =
                            "Overtime",

                        Description =
                            $"Overtime - {overtime.StartDateTime:yyyy-MM-dd}",

                        Source =
                            PayrollItemSource.Overtime,

                        Type =
                            SalaryComponentType.Allowance
                    };


                calculation.Items.Add(
                    payrollItem);


                links.Add(
                    new OvertimePayrollItemLink
                    {
                        Overtime =
                            overtime,

                        PayrollItem =
                            payrollItem
                    });


                calculation.TotalOvertime +=
                    amount;

            }


            return links;
        }

        private static Payroll CreatePayroll(
        int employeeId,
        int payrollPeriodId,
        EmployeeBankAccount bankAccount,
        PayrollCalculationResult calculation)
        {
            if (bankAccount == null)
            {
                throw new Exception(
                    $"Primary bank account is required " +
                    $"for employee {employeeId}.");
            }


            if (string.IsNullOrWhiteSpace(
                    bankAccount.BankName))
            {
                throw new Exception(
                    $"Bank name is required " +
                    $"for employee {employeeId}.");
            }


            if (string.IsNullOrWhiteSpace(
                    bankAccount.AccountNumber) &&
                string.IsNullOrWhiteSpace(
                    bankAccount.IBAN))
            {
                throw new Exception(
                    $"Account number or IBAN is required " +
                    $"for employee {employeeId}.");
            }


            foreach (var item in calculation.Items)
            {
                if (string.IsNullOrWhiteSpace(
                        item.ItemName))
                {
                    throw new Exception(
                        $"PayrollItem has no ItemName. " +
                        $"EmployeeId={employeeId}, " +
                        $"SalaryComponentId={item.SalaryComponentId}, " +
                        $"Amount={item.Amount}");
                }
            }


            return new Payroll
            {
                EmployeeId =
                    employeeId,

                PayrollPeriodId =
                    payrollPeriodId,

                GrossSalary =
                    calculation.GrossSalary,

                TotalAllowances =
                    calculation.TotalAllowances,

                TotalOvertime =
                    calculation.TotalOvertime,

                TotalDeductions =
                    calculation.TotalDeductions,

                NetSalary =
                    calculation.NetSalary,

                Status =
                    PayrollStatus.Calculated,

                BankName =
                    bankAccount.BankName,

                BankAccountNumber =
                    bankAccount.AccountNumber,

                IBAN =
                    bankAccount.IBAN,

                PayrollItems =
                    calculation.Items
            };
        }

        private async Task ApplyUnpaidLeaveAsync(
        PayrollCalculationResult calculation,
        int employeeId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken)
        {
            var items =
                await _unpaidLeavePayrollCalculator.CalculateAsync(
                    employeeId,
                    startDate,
                    endDate,
                    calculation.GrossSalary,
                    cancellationToken);


            if (!items.Any())
                return;


            foreach (var item in items)
            {
                calculation.Items.Add(item);

                calculation.TotalDeductions +=
                    item.Amount;
            }
        }

        private async Task ResetPayrollAsync(
        Payroll payroll,
        PayrollPeriod period,
        CancellationToken cancellationToken)
        {
            // =========================================================
            // Paid Payroll Cannot Be Regenerated
            // =========================================================

            if (payroll.Status == PayrollStatus.Paid)
            {
                throw new Exception(
                    "Cannot regenerate a paid payroll.");
            }


            // =========================================================
            // Reset Overtime Payroll Links
            // =========================================================

            var startDateTime =
           period.StartDate.ToDateTime(TimeOnly.MinValue);

            var endDateTimeExclusive =
                period.EndDate
                    .AddDays(1)
                    .ToDateTime(TimeOnly.MinValue);

            var overtimes =
                await _context.EmployeeOvertimes
                   .Where(x =>
                    x.EmployeeId == payroll.EmployeeId &&
                    x.StartDateTime >= startDateTime &&
                    x.StartDateTime < endDateTimeExclusive &&
                    x.PayrollItemId != null)
                   .ToListAsync(
                        cancellationToken);


            foreach (var overtime in overtimes)
            {
                // لا نغيّر:
                // Amount
                // HourlyRate
                // Multiplier
                // IsPaid

                overtime.PayrollItemId = null;
            }


            // =========================================================
            // Reset Payroll Adjustments
            // =========================================================

            var adjustments =
           await _context.PayrollAdjustments
          .Where(x =>
              x.EmployeeId == payroll.EmployeeId &&
              x.PayrollPeriodId == payroll.PayrollPeriodId &&
              x.IsApplied)
          .ToListAsync(cancellationToken);

            foreach (var adjustment in adjustments)
            {
                adjustment.IsApplied = false;
            }

            // =========================================================
            // Reset Loan Installments
            // =========================================================

            var loanInstallments =
                await _context.EmployeeLoanInstallments
                    .Where(x =>
                        x.PayrollAdjustment != null &&
                        x.PayrollAdjustment.PayrollPeriodId ==
                            payroll.PayrollPeriodId &&
                        x.EmployeeLoan.EmployeeId ==
                            payroll.EmployeeId &&
                        x.Status ==
                            LoanInstallmentStatus.Applied)
                    .ToListAsync(
                        cancellationToken);


            foreach (var installment in loanInstallments)
            {
                installment.Status =
                    LoanInstallmentStatus.Pending;

                installment.PayrollAdjustment!.IsApplied = false;

            }
            // =========================================================
            // Delete Payroll Items
            // =========================================================

            if (payroll.PayrollItems != null &&
                payroll.PayrollItems.Count > 0)
            {
                _context.PayrollItems.RemoveRange(
                    payroll.PayrollItems);
            }


            // =========================================================
            // Delete Payroll
            // =========================================================

            _context.Payrolls.Remove(
                payroll);
            await _context.SaveChangesAsync(cancellationToken);
        }

        private sealed class OvertimePayrollItemLink
        {
            public EmployeeOvertime Overtime { get; init; } = null!;

            public PayrollItem PayrollItem { get; init; } = null!;
        }

    private async Task<List<EmployeeLoanInstallment>>
    SkipSuspendedLoanInstallmentsAsync(
        int employeeId,
        int payrollPeriodId,
        CancellationToken cancellationToken)
        {
            // =========================================================
            // Get Payroll Period
            // =========================================================

            var period =
                await _context.PayrollPeriods
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x => x.Id == payrollPeriodId,
                        cancellationToken);

            if (period == null)
                return [];


            // =========================================================
            // Get Pending Installments
            // =========================================================
            //
            // Only:
            // - Same employee
            // - Suspended loan
            // - Current payroll period
            // - Pending installments
            //
            // =========================================================

            var installments =
                await _context.EmployeeLoanInstallments
                    .Include(x => x.EmployeeLoan)
                    .Where(x =>
                        x.EmployeeLoan.EmployeeId == employeeId &&

                        x.EmployeeLoan.Status ==
                            LoanStatus.Suspended &&

                        x.DueDate >= period.StartDate &&
                        x.DueDate <= period.EndDate &&

                        x.Status ==
                            LoanInstallmentStatus.Pending)
                    .OrderBy(x => x.DueDate)
                    .ThenBy(x => x.InstallmentNumber)
                    .ToListAsync(
                        cancellationToken);


            if (installments.Count == 0)
                return [];


            // =========================================================
            // Skip Installments
            // =========================================================

            var skippedAt =
                DateOnly.FromDateTime(
                    DateTime.UtcNow);


            foreach (var installment in installments)
            {
                installment.Status =
                    LoanInstallmentStatus.Skipped;

                installment.SkippedAt =
                    skippedAt;

                installment.SkipReason =
                    "Loan suspended";
            }


            // =========================================================
            // IMPORTANT
            // =========================================================
            //
            // Do not call SaveChangesAsync here.
            //
            // GeneratePayrollAsync should control the transaction
            // and save everything together.
            //
            // =========================================================

            return installments;
        }
    }

  }
