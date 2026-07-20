using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Mappings;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeLeaveBalances.Interfaces;
using MicroERP.Application.Features.EmployeeLeaves.DTOs;
using MicroERP.Application.Features.EmployeeLeaves.Interfaces;
using MicroERP.Domin.Entities;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.EmployeeLeaves.Services;

public class EmployeeLeaveService : IEmployeeLeaveService
{
    private readonly IApplicationDbContext _context;
    private readonly IEmployeeLeaveQueries _queries;
    private readonly ILeaveBalanceGenerator _leaveBalanceGenerator;
    private readonly IFileStorageService _fileStorage;
    public EmployeeLeaveService(
        IApplicationDbContext context,
        IEmployeeLeaveQueries queries,
        ILeaveBalanceGenerator leaveBalanceGenerator,
        IFileStorageService fileStorage)
    {
        _context = context;
        _queries = queries;
        _leaveBalanceGenerator = leaveBalanceGenerator;
        _fileStorage = fileStorage;
    }


    // Get all employee leave requests
    public async Task<Result<List<LeaveListDto>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var leaves = await _queries.GetAllAsync(cancellationToken);

        return Result<List<LeaveListDto>>.Succeeded(
            leaves.Select(x => x.ToListDto()).ToList());
    }



    // Get leave request by id
    public async Task<Result<LeaveDto>> GetByIdAsync(int id,
        CancellationToken cancellationToken = default)
    {
        var leave = await _queries.GetByIdAsync(
            id,
            cancellationToken);


        if (leave == null)
            return Result<LeaveDto>.Failure(
                "Leave request not found");


        return Result<LeaveDto>.Succeeded(
            leave.ToDto());
    }



    // Get all leave requests for specific employee
    public async Task<Result<List<LeaveListDto>>> GetByEmployeeAsync(int employeeId,
        CancellationToken cancellationToken = default)
    {
        var leaves = await _queries.GetByEmployeeIdAsync(
            employeeId,
            cancellationToken);


        return Result<List<LeaveListDto>>.Succeeded(
            leaves.Select(x => x.ToListDto()).ToList());
    }



    // Create new leave request
    public async Task<Result<LeaveDto>> CreateAsync(int employeeId,CreateLeaveDto dto,
      CancellationToken cancellationToken = default)
    {
        if (dto.StartDate > dto.EndDate)
            return Result<LeaveDto>.Failure(
                "Start date cannot be after end date");


        var employeeExists = await _context.Employees
            .AnyAsync(
                x => x.Id == employeeId,
                cancellationToken);


        if (!employeeExists)
            return Result<LeaveDto>.Failure(
                "Employee not found");



        if (await _queries.HasOverlappingLeaveAsync(
                employeeId,
                dto.StartDate,
                dto.EndDate,
                null,
                cancellationToken))
        {
            return Result<LeaveDto>.Failure(
                "Employee already has leave during this period");
        }



        var totalDays =
            dto.EndDate.DayNumber -
            dto.StartDate.DayNumber + 1;



        var policy = await _context.LeavePolicies
            .FirstOrDefaultAsync(
                x => x.LeaveType == dto.LeaveType,
                cancellationToken);



        if (policy == null)
            return Result<LeaveDto>.Failure(
                "Leave policy not found");



        int sickDays = 0;

        int emergencyDays = 0;

        int unpaidDays = 0;



        bool hasWarning = false;

        string? warningMessage = null;



        // ==============================
        // Leaves without balance
        // Unpaid / Maternity / Paternity
        // ==============================
        if (!policy.RequiresBalance)
        {
            unpaidDays =
                dto.LeaveType == LeaveType.Unpaid
                ? totalDays
                : 0;
        }


        // ==============================
        // Leaves with balance
        // ==============================
        else
        {
            if (dto.LeaveType == LeaveType.Sick)
            {
                var sickBalance = await GetBalanceAsync(
                    employeeId,
                    LeaveType.Sick,
                    dto.StartDate.Year,
                    cancellationToken);



                var sickRemaining =
                    sickBalance.TotalDays -
                    sickBalance.UsedDays;



                sickDays = Math.Min(
                    sickRemaining,
                    totalDays);



                var remaining =
                    totalDays - sickDays;



                if (remaining > 0)
                {
                    var emergencyBalance =
                        await GetBalanceAsync(
                            employeeId,
                            LeaveType.Emergency,
                            dto.StartDate.Year,
                            cancellationToken);



                    var emergencyRemaining =
                        emergencyBalance.TotalDays -
                        emergencyBalance.UsedDays;



                    emergencyDays = Math.Min(
                        emergencyRemaining,
                        remaining);



                    remaining -= emergencyDays;



                    if (remaining > 0)
                        unpaidDays = remaining;



                    hasWarning = true;

                    warningMessage =
                        $"Sick balance: {sickDays} days, " +
                        $"Emergency balance: {emergencyDays} days, " +
                        $"Unpaid: {unpaidDays} days.";
                }
            }



            else if (dto.LeaveType == LeaveType.Emergency)
            {
                var balance = await GetBalanceAsync(
                    employeeId,
                    LeaveType.Emergency,
                    dto.StartDate.Year,
                    cancellationToken);



                var remaining =
                    balance.TotalDays -
                    balance.UsedDays;



                emergencyDays =
                    Math.Min(
                        remaining,
                        totalDays);



                if (emergencyDays < totalDays)
                {
                    unpaidDays =
                        totalDays - emergencyDays;


                    hasWarning = true;

                    warningMessage =
                        $"{unpaidDays} days will be unpaid.";
                }
            }



            else if (dto.LeaveType == LeaveType.Annual)
            {
                var balance = await GetBalanceAsync(
                    employeeId,
                    LeaveType.Annual,
                    dto.StartDate.Year,
                    cancellationToken);



                var remaining =
                    balance.TotalDays -
                    balance.UsedDays;



                if (remaining < totalDays)
                {
                    var negativeDays =
                        totalDays - remaining;



                    if (!policy.AllowNegativeBalance ||
                        negativeDays > policy.MaxNegativeDays)
                    {
                        return Result<LeaveDto>.Failure(
                            "Insufficient annual leave balance");
                    }



                    hasWarning = true;

                    warningMessage =
                        $"This request uses {negativeDays} future annual leave days.";
                }
            }
        }
        if (!policy.RequiresBalance &&
       policy.MaximumDaysPerYear > 0 &&
         totalDays > policy.MaximumDaysPerYear)
        {
            return Result<LeaveDto>.Failure(
                $"Maximum allowed days for {dto.LeaveType} is {policy.MaximumDaysPerYear}");
        }


        var leave = new EmployeeLeave
        {
            EmployeeId = employeeId,

            LeaveType = dto.LeaveType,

            StartDate = dto.StartDate,

            EndDate = dto.EndDate,

            TotalDays = totalDays,

            SickDays = sickDays,

            EmergencyDays = emergencyDays,

            UnpaidDays = unpaidDays,

            Reason = dto.Reason,

            Status = LeaveStatus.Pending
        };



        _context.EmployeeLeaves.Add(leave);


        await _context.SaveChangesAsync(
            cancellationToken);



        var createdLeave =
            await _queries.GetByIdAsync(
                leave.Id,
                cancellationToken);



        var result =
            createdLeave!.ToDto();



        result.HasWarning = hasWarning;

        result.WarningMessage = warningMessage;



        return Result<LeaveDto>.Succeeded(result);
    }



    // Update pending leave request
    public async Task<Result<LeaveDto>> UpdateAsync(int id,UpdateLeaveDto dto,
        CancellationToken cancellationToken = default)
    {
        var leave = await _context.EmployeeLeaves
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);


        if (leave == null)
            return Result<LeaveDto>.Failure(
                "Leave request not found");


        if (leave.Status != LeaveStatus.Pending)
            return Result<LeaveDto>.Failure(
                "Only pending requests can be updated");


        var startDate = dto.StartDate ?? leave.StartDate;
        var endDate = dto.EndDate ?? leave.EndDate;


        if (startDate > endDate)
            return Result<LeaveDto>.Failure(
                "Start date cannot be after end date");


        if (await _queries.HasOverlappingLeaveAsync(
                leave.EmployeeId,
                startDate,
                endDate,
                id,
                cancellationToken))
        {
            return Result<LeaveDto>.Failure(
                "Employee already has leave during this period");
        }


        if (dto.LeaveType.HasValue)
            leave.LeaveType = dto.LeaveType.Value;


        if (dto.StartDate.HasValue)
            leave.StartDate = dto.StartDate.Value;


        if (dto.EndDate.HasValue)
            leave.EndDate = dto.EndDate.Value;


        if (dto.Reason != null)
            leave.Reason = dto.Reason;


        leave.TotalDays =
            leave.EndDate.DayNumber -
            leave.StartDate.DayNumber + 1;


        await _context.SaveChangesAsync(cancellationToken);


        var updatedLeave = await _queries.GetByIdAsync(
            id,
            cancellationToken);


        return Result<LeaveDto>.Succeeded(
            updatedLeave!.ToDto());
    }



    // Delete pending leave request
    public async Task<Result> DeleteAsync(int id,
        CancellationToken cancellationToken = default)
    {
        var leave = await _context.EmployeeLeaves
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);


        if (leave == null)
            return Result.Failure(
                "Leave request not found");


        if (leave.Status != LeaveStatus.Pending)
            return Result.Failure(
                "Only pending requests can be deleted");


        _context.EmployeeLeaves.Remove(leave);

        await _context.SaveChangesAsync(cancellationToken);


        return Result.Succeeded();
    }



    // Approve leave request
    public async Task<Result<LeaveDto>> ApproveAsync(int id,string userId,
    CancellationToken cancellationToken = default)
    {
        var leave = await _context.EmployeeLeaves
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);


        if (leave == null)
            return Result<LeaveDto>.Failure(
                "Leave request not found");


        if (leave.Status != LeaveStatus.Pending)
            return Result<LeaveDto>.Failure(
                "Only pending requests can be approved");



        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);


        try
        {
            if (leave.SickDays > 0)
            {
                var sickBalance = await GetBalanceAsync(
                    leave.EmployeeId,
                    LeaveType.Sick,
                    leave.StartDate.Year,
                    cancellationToken);


                sickBalance.UsedDays += leave.SickDays;
            }



            if (leave.EmergencyDays > 0)
            {
                var emergencyBalance = await GetBalanceAsync(
                    leave.EmployeeId,
                    LeaveType.Emergency,
                    leave.StartDate.Year,
                    cancellationToken);


                emergencyBalance.UsedDays += leave.EmergencyDays;
            }



            if (leave.LeaveType == LeaveType.Annual)
            {
                var annualBalance = await GetBalanceAsync(
                    leave.EmployeeId,
                    LeaveType.Annual,
                    leave.StartDate.Year,
                    cancellationToken);


                annualBalance.UsedDays += leave.TotalDays;
            }



            // UnpaidDays لا يتم خصمها من أي رصيد
            if (leave.UnpaidDays > 0)
            {
                // لا يوجد خصم
            }



            leave.Status = LeaveStatus.Approved;

            leave.ApprovedByUserId = userId;

            leave.ApprovedOn = DateTime.UtcNow;



            await _context.SaveChangesAsync(
                cancellationToken);



            await transaction.CommitAsync(
                cancellationToken);



            var result = await _queries.GetByIdAsync(
                id,
                cancellationToken);


            return Result<LeaveDto>.Succeeded(
                result!.ToDto());
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }
    }


    // Reject leave request
    public async Task<Result<LeaveDto>> RejectAsync(int id,string userId,RejectLeaveDto dto,
        CancellationToken cancellationToken = default)
    {
        var leave = await _context.EmployeeLeaves
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);


        if (leave == null)
            return Result<LeaveDto>.Failure(
                "Leave request not found");


        if (leave.Status != LeaveStatus.Pending)
            return Result<LeaveDto>.Failure(
                "Only pending requests can be rejected");


        leave.Status = LeaveStatus.Rejected;
        leave.RejectedByUserId = userId;
        leave.RejectedOn = DateTime.UtcNow;
        leave.RejectionReason = dto.RejectionReason;


        await _context.SaveChangesAsync(cancellationToken);


        var rejectedLeave = await _queries.GetByIdAsync(
            id,
            cancellationToken);


        return Result<LeaveDto>.Succeeded(
            rejectedLeave!.ToDto());
    }



    // Cancel leave request
    public async Task<Result<LeaveDto>> CancelAsync(int id,string userId,
        CancellationToken cancellationToken = default)
    {
        var leave = await _context.EmployeeLeaves
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);


        if (leave == null)
            return Result<LeaveDto>.Failure(
                "Leave request not found");


        if (leave.Status != LeaveStatus.Approved)
            return Result<LeaveDto>.Failure(
                "Only approved leaves can be cancelled");



        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);


        try
        {
            if (leave.SickDays > 0)
            {
                var sickBalance = await GetBalanceAsync(
                    leave.EmployeeId,
                    LeaveType.Sick,
                    leave.StartDate.Year,
                    cancellationToken);


                sickBalance.UsedDays -= leave.SickDays;


                if (sickBalance.UsedDays < 0)
                    sickBalance.UsedDays = 0;
            }



            if (leave.EmergencyDays > 0)
            {
                var emergencyBalance = await GetBalanceAsync(
                    leave.EmployeeId,
                    LeaveType.Emergency,
                    leave.StartDate.Year,
                    cancellationToken);


                emergencyBalance.UsedDays -= leave.EmergencyDays;


                if (emergencyBalance.UsedDays < 0)
                    emergencyBalance.UsedDays = 0;
            }



            if (leave.LeaveType == LeaveType.Annual)
            {
                var annualBalance = await GetBalanceAsync(
                    leave.EmployeeId,
                    LeaveType.Annual,
                    leave.StartDate.Year,
                    cancellationToken);


                annualBalance.UsedDays -= leave.TotalDays;


                if (annualBalance.UsedDays < 0)
                    annualBalance.UsedDays = 0;
            }



            // UnpaidDays لا يوجد لها رصيد لإرجاعه



            leave.Status = LeaveStatus.Cancelled;

            leave.ModifiedBy = userId;

            leave.ModifiedOn = DateTime.UtcNow;



            await _context.SaveChangesAsync(
                cancellationToken);



            await transaction.CommitAsync(
                cancellationToken);



            var result = await _queries.GetByIdAsync(
                id,
                cancellationToken);



            return Result<LeaveDto>.Succeeded(
                result!.ToDto());
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }
    }


    //======================HELPERS================

    private async Task<EmployeeLeaveBalance> GetBalanceAsync(int employeeId,LeaveType leaveType,int year,
    CancellationToken cancellationToken)
    {
        var balance = await _context.EmployeeLeaveBalances
            .FirstOrDefaultAsync(
                x =>
                    x.EmployeeId == employeeId &&
                    x.Year == year &&
                    x.LeaveType == leaveType,
                cancellationToken);


        if (balance == null)
        {
            await _leaveBalanceGenerator.GenerateForEmployeeAsync(
                employeeId,
                year,
                cancellationToken);


            balance = await _context.EmployeeLeaveBalances
                .FirstOrDefaultAsync(
                    x =>
                        x.EmployeeId == employeeId &&
                        x.Year == year &&
                        x.LeaveType == leaveType,
                    cancellationToken);
        }


        if (balance == null)
            throw new Exception(
                $"Leave balance not found for {leaveType}");


        return balance;
    }
}