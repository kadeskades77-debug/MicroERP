using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeSpecialLeaves.DTOs;
using MicroERP.Application.Features.EmployeeSpecialLeaves.Interfaces;
using MicroERP.Domin.Entities;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.EmployeeSpecialLeaves.Services;

public class EmployeeSpecialLeaveService : IEmployeeSpecialLeaveService
{
    private readonly IApplicationDbContext _context;
    private readonly IEmployeeSpecialLeaveQueries _queries;
    private readonly IFileStorageService _fileStorage;

    public EmployeeSpecialLeaveService(
        IApplicationDbContext context,
        IEmployeeSpecialLeaveQueries queries,
        IFileStorageService fileStorage)
    {
        _context = context;
        _queries = queries;
        _fileStorage = fileStorage;
    }



    public async Task<Result<SpecialLeaveDto>> CreateAsync(
       int employeeId,
       string userId,
       CreateSpecialLeaveDto dto,
       CancellationToken cancellationToken = default)
    {
        if (!Enum.IsDefined(typeof(SpecialLeaveType), dto.Type))
        {
            return Result<SpecialLeaveDto>.Failure(
                "Invalid special leave type.");
        }
        var employeeExists = await _context.Employees
            .AnyAsync(x => x.Id == employeeId, cancellationToken);

        if (!employeeExists)
        {
            return Result<SpecialLeaveDto>.Failure(
                "Employee not found");
        }

        var policy = await _context.LeavePolicies
            .FirstOrDefaultAsync(
                x => x.LeaveType == (LeaveType)dto.Type,
                cancellationToken);

        if (policy == null)
        {
            return Result<SpecialLeaveDto>.Failure(
                "Leave policy not found");
        }

        if (dto.Type == SpecialLeaveType.Marriage)
        {
            var marriageExists = await _context.EmployeeSpecialLeaves
                .AnyAsync(x =>
                    x.EmployeeId == employeeId &&
                    x.Type == SpecialLeaveType.Marriage &&
                    x.Status != SpecialLeaveStatus.Cancelled,
                    cancellationToken);

            if (marriageExists)
            {
                return Result<SpecialLeaveDto>.Failure(
                    "Marriage leave can only be granted once during employment.");
            }
        }

        var startDate = dto.StartDate;
        var totalDays = policy.MaximumDaysPerYear;
        var endDate = startDate.AddDays(totalDays - 1);

        if (await HasOverlappingLeaveAsync(
                employeeId,
                startDate,
                endDate,
                cancellationToken))
        {
            return Result<SpecialLeaveDto>.Failure(
                "Employee already has another leave during this period.");
        }

        var leave = new EmployeeSpecialLeave
        {
            EmployeeId = employeeId,

            Type = dto.Type,

            StartDate = startDate,

            EndDate = endDate,

            TotalDays = totalDays,

            Reason = dto.Reason,

            Status = SpecialLeaveStatus.Approved,

            ApprovedByUserId = userId,

            ApprovedOn = DateTime.UtcNow
        };

        _context.EmployeeSpecialLeaves.Add(leave);

        await _context.SaveChangesAsync(cancellationToken);

        var result = await _queries.GetByIdAsync(
            leave.Id,
            cancellationToken);

        return Result<SpecialLeaveDto>.Succeeded(result!);
    }


    public async Task<Result<SpecialLeaveDto>> CancelAsync(
     int id,
     string userId,
     CancellationToken cancellationToken = default)
    {

        var leave = await _context.EmployeeSpecialLeaves
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (leave == null)
        {
            return Result<SpecialLeaveDto>.Failure(
                "Special leave not found");
        }

        if (leave.Status != SpecialLeaveStatus.Approved)
        {
            return Result<SpecialLeaveDto>.Failure(
                "Only approved requests can be cancelled");
        }

        leave.Status = SpecialLeaveStatus.Cancelled;

        leave.ModifiedBy = userId;

        leave.ModifiedOn = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        var result = await _queries.GetByIdAsync(
            id,
            cancellationToken);

        return Result<SpecialLeaveDto>.Succeeded(result!);
    }

    //====================Helpers=======================

    private async Task<bool> HasOverlappingLeaveAsync(
        int employeeId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        var normalLeave = await _context.EmployeeLeaves
            .AnyAsync(x =>
                x.EmployeeId == employeeId &&
                (x.Status == LeaveStatus.Pending ||
                 x.Status == LeaveStatus.Approved) &&
                startDate <= x.EndDate &&
                endDate >= x.StartDate,
                cancellationToken);

        if (normalLeave)
        {
            return true;
        }

        return await _context.EmployeeSpecialLeaves
            .AnyAsync(x =>
                x.EmployeeId == employeeId &&
                x.Status == SpecialLeaveStatus.Approved &&
                startDate <= x.EndDate &&
                endDate >= x.StartDate,
                cancellationToken);
    }
}