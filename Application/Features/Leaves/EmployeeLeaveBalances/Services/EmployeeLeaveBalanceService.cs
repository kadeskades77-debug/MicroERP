using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Mappings;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Leaves.EmployeeLeaveBalances.DTOs;
using MicroERP.Application.Features.Leaves.EmployeeLeaveBalances.Interfaces;
using MicroERP.Domin.Entities.EmployeeLeaves;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.Leaves.EmployeeLeaveBalances.Services;

public class EmployeeLeaveBalanceService : IEmployeeLeaveBalanceService
{
    private readonly IApplicationDbContext _context;
    private readonly IEmployeeLeaveBalanceQueries _queries;


    public EmployeeLeaveBalanceService(
        IApplicationDbContext context,
        IEmployeeLeaveBalanceQueries queries)
    {
        _context = context;
        _queries = queries;
    }



    // Get employee leave balances
    public async Task<Result<List<LeaveBalanceDto>>> GetByEmployeeAsync(int employeeId,int year,
        CancellationToken cancellationToken = default)
    {
        var balances = await _queries.GetByEmployeeIdAsync(
            employeeId,
            year,
            cancellationToken);


        return Result<List<LeaveBalanceDto>>.Succeeded(
            balances.Select(x => x.ToDto()).ToList());
    }



    // Create employee leave balance
    public async Task<Result<LeaveBalanceDto>> CreateAsync(int employeeId,CreateLeaveBalanceDto dto,
        CancellationToken cancellationToken = default)
    {
        var employeeExists = await _context.Employees
            .AnyAsync(
                x => x.Id == employeeId,
                cancellationToken);


        if (!employeeExists)
            return Result<LeaveBalanceDto>.Failure(
                "Employee not found");


        var exists = await _queries.GetAsync(
            employeeId,
            dto.Year,
            dto.LeaveType,
            cancellationToken);


        if (exists != null)
            return Result<LeaveBalanceDto>.Failure(
                "Leave balance already exists");


        var balance = new EmployeeLeaveBalance
        {
            EmployeeId = employeeId,

            Year = dto.Year,

            LeaveType = dto.LeaveType,

            TotalDays = dto.TotalDays,

            UsedDays = 0
        };


        _context.EmployeeLeaveBalances.Add(balance);

        await _context.SaveChangesAsync(cancellationToken);


        var result = await _queries.GetAsync(
            employeeId,
            dto.Year,
            dto.LeaveType,
            cancellationToken);


        return Result<LeaveBalanceDto>.Succeeded(
            result!.ToDto());
    }



    // Update employee leave balance
    public async Task<Result<LeaveBalanceDto>> UpdateAsync(int id,UpdateLeaveBalanceDto dto,
        CancellationToken cancellationToken = default)
    {
        var balance = await _context.EmployeeLeaveBalances
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);


        if (balance == null)
            return Result<LeaveBalanceDto>.Failure(
                "Leave balance not found");


        if (dto.TotalDays < balance.UsedDays)
            return Result<LeaveBalanceDto>.Failure(
                "Total days cannot be less than used days");


        balance.TotalDays = dto.TotalDays;


        await _context.SaveChangesAsync(cancellationToken);


        return Result<LeaveBalanceDto>.Succeeded(
            balance.ToDto());
    }



    // Delete employee leave balance
    public async Task<Result> DeleteAsync(int id,
        CancellationToken cancellationToken = default)
    {
        var balance = await _context.EmployeeLeaveBalances
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);


        if (balance == null)
            return Result.Failure(
                "Leave balance not found");


        _context.EmployeeLeaveBalances.Remove(balance);

        await _context.SaveChangesAsync(cancellationToken);


        return Result.Succeeded();
    }
}