using AutoMapper;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.Payrolls.Adjustments.DTOs;
using MicroERP.Application.Features.Payrolls.Adjustments.Interfaces;
using MicroERP.Domin.Entities.Payrolls;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.Payrolls.Adjustments.Services;

public class PayrollAdjustmentService
    : IPayrollAdjustmentService
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;
    private readonly IMapper _mapper;


    public PayrollAdjustmentService(
        IApplicationDbContext context,
        IMapper mapper,
        IAuditService auditService)
    {
        _context = context;
        _mapper = mapper;
        _auditService = auditService;
    }



    public async Task<Result<PayrollAdjustmentDto>> CreateAsync(
        CreatePayrollAdjustmentDto dto,
        CancellationToken cancellationToken)
    {
        // =========================================================
        // Employee
        // =========================================================

        var employeeExists =
            await _context.Employees
                .AnyAsync(
                    x => x.Id == dto.EmployeeId,
                    cancellationToken);

        if (!employeeExists)
        {
            return Result<PayrollAdjustmentDto>
                .Failure(
                    "Employee not found.");
        }


        // =========================================================
        // Validation
        // =========================================================

        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            return Result<PayrollAdjustmentDto>
                .Failure(
                    "Adjustment title is required.");
        }


        if (dto.Amount <= 0)
        {
            return Result<PayrollAdjustmentDto>
                .Failure(
                    "Adjustment amount must be greater than zero.");
        }


        // =========================================================
        // Payroll Period
        // =========================================================

        var period =
            await _context.PayrollPeriods
                .FirstOrDefaultAsync(
                    x => x.Id == dto.PayrollPeriodId,
                    cancellationToken);

        if (period == null)
        {
            return Result<PayrollAdjustmentDto>
                .Failure(
                    "Payroll period not found.");
        }


        if (period.Status == PayrollPeriodStatus.Closed)
        {
            return Result<PayrollAdjustmentDto>
                .Failure(
                    "Cannot create an adjustment for a closed payroll period.");
        }


        // =========================================================
        // Salary Component
        // =========================================================

        if (dto.SalaryComponentId.HasValue)
        {
            var salaryComponentExists =
                await _context.SalaryComponents
                    .AnyAsync(
                        x => x.Id == dto.SalaryComponentId.Value,
                        cancellationToken);

            if (!salaryComponentExists)
            {
                return Result<PayrollAdjustmentDto>
                    .Failure(
                        "Salary component not found.");
            }
        }


        // =========================================================
        // Create
        // =========================================================

        var entity =
            _mapper.Map<PayrollAdjustment>(dto);

        entity.IsApplied = false;


        await _context.PayrollAdjustments
            .AddAsync(
                entity,
                cancellationToken);


        await _context.SaveChangesAsync(
            cancellationToken);


        // =========================================================
        // Reload
        // =========================================================

        var result =
            await _context.PayrollAdjustments
                .Include(x => x.Employee)
                    .ThenInclude(x => x.User)
                .Include(x => x.PayrollPeriod)
                .Include(x => x.SalaryComponent)
                .FirstAsync(
                    x => x.Id == entity.Id,
                    cancellationToken);


        return Result<PayrollAdjustmentDto>
            .Succeeded(
                _mapper.Map<PayrollAdjustmentDto>(result));
    }

    public async Task<Result<List<PayrollAdjustmentDto>>>
       CreateForAllEmployeesAsync(
           CreatePayrollAdjustmentForAllEmployeesDto dto,
           CancellationToken cancellationToken = default)
    {
        // =========================================================
        // Validation
        // =========================================================

        if (dto == null)
        {
            return Result<List<PayrollAdjustmentDto>>
                .Failure(
                    "Adjustment data is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            return Result<List<PayrollAdjustmentDto>>
                .Failure(
                    "Adjustment title is required.");
        }

        if (dto.Amount <= 0)
        {
            return Result<List<PayrollAdjustmentDto>>
                .Failure(
                    "Adjustment amount must be greater than zero.");
        }


        // =========================================================
        // Payroll Period
        // =========================================================

        var period =
            await _context.PayrollPeriods
                .FirstOrDefaultAsync(
                    x => x.Id == dto.PayrollPeriodId,
                    cancellationToken);

        if (period == null)
        {
            return Result<List<PayrollAdjustmentDto>>
                .Failure(
                    "Payroll period not found.");
        }

        if (period.Status == PayrollPeriodStatus.Closed)
        {
            return Result<List<PayrollAdjustmentDto>>
                .Failure(
                    "Cannot create adjustments for a closed payroll period.");
        }


        // =========================================================
        // Salary Component
        // =========================================================

        if (dto.SalaryComponentId.HasValue)
        {
            var salaryComponentExists =
                await _context.SalaryComponents
                    .AnyAsync(
                        x => x.Id == dto.SalaryComponentId.Value,
                        cancellationToken);

            if (!salaryComponentExists)
            {
                return Result<List<PayrollAdjustmentDto>>
                    .Failure(
                        "Salary component not found.");
            }
        }


        // =========================================================
        // Employees
        // =========================================================

        var employees =
            await _context.Employees
                .AsNoTracking()
                .Select(x => x.Id)
                .ToListAsync(
                    cancellationToken);

        if (employees.Count == 0)
        {
            return Result<List<PayrollAdjustmentDto>>
                .Failure(
                    "No employees found.");
        }


        // =========================================================
        // Existing Adjustments
        // =========================================================

        var existingEmployeeIds =
            await _context.PayrollAdjustments
                .AsNoTracking()
                .Where(x =>
                    x.PayrollPeriodId ==
                        dto.PayrollPeriodId &&

                    employees.Contains(
                        x.EmployeeId) &&

                    x.Title ==
                        dto.Title &&

                    x.Type ==
                        dto.Type &&

                    !x.IsApplied)
                .Select(x => x.EmployeeId)
                .ToListAsync(
                    cancellationToken);


        var existingIds =
            existingEmployeeIds.ToHashSet();


        // =========================================================
        // Create Adjustments
        // =========================================================

        var entities =
            new List<PayrollAdjustment>();


        foreach (var employeeId in employees)
        {
            if (existingIds.Contains(employeeId))
            {
                continue;
            }

            entities.Add(
                new PayrollAdjustment
                {
                    EmployeeId =
                        employeeId,

                    PayrollPeriodId =
                        dto.PayrollPeriodId,

                    SalaryComponentId =
                        dto.SalaryComponentId,

                    Type =
                        dto.Type,

                    Title =
                        dto.Title,

                    Notes =
                        dto.Notes,

                    Amount =
                        dto.Amount,

                    IsApplied =
                        false
                });
        }


        // =========================================================
        // Nothing To Create
        // =========================================================

        if (entities.Count == 0)
        {
            return Result<List<PayrollAdjustmentDto>>
                .Failure(
                    "Adjustments already exist for all employees in this payroll period.");
        }


        // =========================================================
        // Save
        // =========================================================

        await _context.PayrollAdjustments
            .AddRangeAsync(
                entities,
                cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);


        // =========================================================
        // Reload
        // =========================================================

        var ids =
            entities
                .Select(x => x.Id)
                .ToList();

        var result =
            await _context.PayrollAdjustments
                .AsNoTracking()
                .Include(x => x.Employee)
                    .ThenInclude(x => x.User)
                .Include(x => x.PayrollPeriod)
                .Include(x => x.SalaryComponent)
                .Where(x =>
                    ids.Contains(x.Id))
                .ToListAsync(
                    cancellationToken);


        // =========================================================
        // Map
        // =========================================================

        var data =
            _mapper.Map<List<PayrollAdjustmentDto>>(
                result);


        return Result<List<PayrollAdjustmentDto>>
            .Succeeded(data);
    }


    public async Task<Result<PayrollAdjustmentDto>> UpdateAsync(int id,
        UpdatePayrollAdjustmentDto dto,
        CancellationToken cancellationToken)
    {
        var entity =
            await _context.PayrollAdjustments
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (entity == null)
        {
            return Result<PayrollAdjustmentDto>
                .Failure(
                    "Payroll adjustment not found.");
        }

        if (entity.IsApplied)
        {
            return Result<PayrollAdjustmentDto>
                .Failure(
                    "Applied adjustment cannot be modified.");
        }

        var oldValues = new
        {
            entity.EmployeeId,
            entity.PayrollPeriodId,
            entity.SalaryComponentId,
            entity.Type,
            entity.Title,
            entity.Notes,
            entity.Amount,
            entity.IsApplied
        };

        if (dto.Title != null)
            entity.Title = dto.Title;

        if (dto.Notes != null)
            entity.Notes = dto.Notes;

        if (dto.Type.HasValue)
            entity.Type = dto.Type.Value;

        if (dto.Amount.HasValue)
        {
            if (dto.Amount.Value <= 0)
            {
                return Result<PayrollAdjustmentDto>
                    .Failure(
                        "Adjustment amount must be greater than zero.");
            }

            entity.Amount = dto.Amount.Value;
        }

        var newValues = new
        {
            entity.EmployeeId,
            entity.PayrollPeriodId,
            entity.SalaryComponentId,
            entity.Type,
            entity.Title,
            entity.Notes,
            entity.Amount,
            entity.IsApplied
        };

        await _context.SaveChangesAsync(
            cancellationToken);

        await _auditService.LogAsync(
            "Update",
            "PayrollAdjustment",
            entity.Id.ToString(),
            oldValues,
            newValues);

        var result =
            await _context.PayrollAdjustments
                .Include(x => x.Employee)
                    .ThenInclude(x => x.User)
                .Include(x => x.PayrollPeriod)
                .FirstAsync(
                    x => x.Id == id,
                    cancellationToken);

        return Result<PayrollAdjustmentDto>
            .Succeeded(
                _mapper.Map<PayrollAdjustmentDto>(result));
    }





    public async Task<Result> DeleteAsync(int id,
     CancellationToken cancellationToken = default)
    {
        var adjustment =
            await _context.PayrollAdjustments
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);


        if (adjustment == null)
        {
            return Result.Failure(
                "Payroll adjustment not found.");
        }
        if (adjustment.IsApplied)
        {
            return Result.Failure(
                "Applied adjustment cannot be deleted.");
        }

        var oldValues = new
        {
            adjustment.EmployeeId,
            adjustment.PayrollPeriodId,
            adjustment.SalaryComponentId,
            adjustment.Type,
            adjustment.Title,
            adjustment.Notes,
            adjustment.Amount,
            adjustment.IsApplied
        };


        _context.PayrollAdjustments
            .Remove(adjustment);


        await _context.SaveChangesAsync(
            cancellationToken);


        await _auditService.LogAsync(
            "Delete",
            "PayrollAdjustment",
            id.ToString(),
            oldValues,
            null);


        return Result.Succeeded(
            "Payroll adjustment deleted successfully.");
    }
}