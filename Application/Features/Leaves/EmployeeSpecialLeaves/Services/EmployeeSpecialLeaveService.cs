using MicroERP.Application.Common.Files.Interfaces;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Leaves.EmployeeSpecialLeaves.DTOs;
using MicroERP.Application.Features.Leaves.EmployeeSpecialLeaves.Interfaces;
using MicroERP.Domin.Entities.EmployeeLeaves;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.Leaves.EmployeeSpecialLeaves.Services;

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


        var employeeExists =
            await _context.Employees
            .AnyAsync(
                x => x.Id == employeeId,
                cancellationToken);


        if (!employeeExists)
        {
            return Result<SpecialLeaveDto>.Failure(
                "Employee not found");
        }



        var policy =
            await _context.LeavePolicies
            .FirstOrDefaultAsync(
                x => x.LeaveType == (LeaveType)dto.Type,
                cancellationToken);



        if (policy == null)
        {
            return Result<SpecialLeaveDto>.Failure(
                "Leave policy not found");
        }


        


        // ===============================
        // Marriage can be granted once
        // ===============================

        if (dto.Type == SpecialLeaveType.Marriage)
        {
            var marriageExists =
                await _context.EmployeeSpecialLeaves
                .AnyAsync(
                    x =>
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




        // ===============================
        // Calculate period
        // ===============================

        var startDate = dto.StartDate;

        DateOnly endDate;
        int totalDays;

  
        if (dto.Type == SpecialLeaveType.Marriage)
        {
            (endDate, totalDays) =
                CalculateMarriageLeavePeriod(
                    startDate,
                    policy.MaximumDaysPerYear);
        }
        else
        {
            (endDate, totalDays) =
                CalculateBereavementLeavePeriod(
                    startDate,
                    policy.MaximumDaysPerYear);
        }
        if (await HasOverlappingSpecialLeaveAsync(
               employeeId,
               startDate,
               endDate,
               null,
               cancellationToken))
        {
            return Result<SpecialLeaveDto>.Failure(
                "Employee already has another special leave during this period.");
        }


        // ===============================
        // Create leave
        // ===============================

        var leave = new EmployeeSpecialLeave
        {
            EmployeeId = employeeId,

            Type = dto.Type,

            StartDate = startDate,

            EndDate = endDate,

            TotalDays = totalDays,

            Reason = dto.Reason,


            // حسب تصميمك الحالي
            Status = SpecialLeaveStatus.Approved,


            ApprovedByUserId = userId,

            ApprovedOn = DateTime.UtcNow
        };



        _context.EmployeeSpecialLeaves.Add(leave);



        await _context.SaveChangesAsync(
            cancellationToken);

        await RestoreOverlappingLeaveBalancesAsync(
       employeeId,
       startDate,
       endDate,
       cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);


        var result =
            await _queries.GetByIdAsync(
                leave.Id,
                cancellationToken);



        return Result<SpecialLeaveDto>.Succeeded(
            result!);
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
        await ReApplyOverlappingLeaveBalancesAsync(
            leave.EmployeeId,
            leave.StartDate,
            leave.EndDate,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

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


    private static (DateOnly EndDate, int TotalDays)
    CalculateBereavementLeavePeriod(
      DateOnly startDate,
      int policyDays)
    {
        if (policyDays <= 0)
            return (startDate, 0);


        var totalDays = policyDays;


        var endDate =
            startDate.AddDays(totalDays - 1);


        return (
            endDate,
            totalDays
        );
    }

    private static (DateOnly EndDate, int TotalDays)
    CalculateMarriageLeavePeriod(
    DateOnly startDate,
    int policyDays)
    {
        if (policyDays <= 0)
            return (startDate, 0);


        var totalDays = policyDays;


        var endDate =
            startDate.AddDays(totalDays - 1);


        return (
            endDate,
            totalDays
        );
    }

    private async Task<bool> HasOverlappingSpecialLeaveAsync(
    int employeeId,
    DateOnly startDate,
    DateOnly endDate,
    int? excludeLeaveId = null,
    CancellationToken cancellationToken = default)
    {
        return await _context.EmployeeSpecialLeaves
            .AnyAsync(x =>
                x.EmployeeId == employeeId &&
                x.Status == SpecialLeaveStatus.Approved &&
                (!excludeLeaveId.HasValue || x.Id != excludeLeaveId.Value) &&
                startDate <= x.EndDate &&
                endDate >= x.StartDate,
                cancellationToken);
    }

    private async Task RestoreOverlappingLeaveBalancesAsync(
       int employeeId,
       DateOnly startDate,
       DateOnly endDate,
       CancellationToken cancellationToken)
    {
        var leaves =
            await _context.EmployeeLeaves
            .Where(x =>
                x.EmployeeId == employeeId &&
                x.Status == LeaveStatus.Approved &&
                x.StartDate <= endDate &&
                x.EndDate >= startDate)
            .ToListAsync(cancellationToken);



        foreach (var leave in leaves)
        {
            if (leave.LeaveType == LeaveType.Annual &&
                leave.TotalDays > 0)
            {
                var balance =
                    await _context.EmployeeLeaveBalances
                    .FirstOrDefaultAsync(x =>
                        x.EmployeeId == employeeId &&
                        x.LeaveType == LeaveType.Annual &&
                        x.Year == leave.StartDate.Year,
                        cancellationToken);


                if (balance != null)
                {
                    balance.UsedDays -= leave.TotalDays;

                    if (balance.UsedDays < 0)
                        balance.UsedDays = 0;
                }
            }



            if (leave.SickDays > 0)
            {
                var balance =
                    await _context.EmployeeLeaveBalances
                    .FirstOrDefaultAsync(x =>
                        x.EmployeeId == employeeId &&
                        x.LeaveType == LeaveType.Sick &&
                        x.Year == leave.StartDate.Year,
                        cancellationToken);


                if (balance != null)
                {
                    balance.UsedDays -= leave.SickDays;

                    if (balance.UsedDays < 0)
                        balance.UsedDays = 0;
                }
            }



            if (leave.EmergencyDays > 0)
            {
                var balance =
                    await _context.EmployeeLeaveBalances
                    .FirstOrDefaultAsync(x =>
                        x.EmployeeId == employeeId &&
                        x.LeaveType == LeaveType.Emergency &&
                        x.Year == leave.StartDate.Year,
                        cancellationToken);


                if (balance != null)
                {
                    balance.UsedDays -= leave.EmergencyDays;

                    if (balance.UsedDays < 0)
                        balance.UsedDays = 0;
                }
            }



            // تحويل الجزء غير المغطى إلى Unpaid لا يحتاج تعديل رصيد
        }
    }


    private async Task ReApplyOverlappingLeaveBalancesAsync(
    int employeeId,
    DateOnly startDate,
    DateOnly endDate,
    CancellationToken cancellationToken)
    {
        var leaves =
            await _context.EmployeeLeaves
            .Where(x =>
                x.EmployeeId == employeeId &&
                x.Status == LeaveStatus.Approved &&
                x.StartDate <= endDate &&
                x.EndDate >= startDate)
            .ToListAsync(cancellationToken);



        foreach (var leave in leaves)
        {
            if (leave.LeaveType == LeaveType.Annual)
            {
                var balance =
                    await _context.EmployeeLeaveBalances
                    .FirstOrDefaultAsync(x =>
                        x.EmployeeId == employeeId &&
                        x.LeaveType == LeaveType.Annual &&
                        x.Year == leave.StartDate.Year,
                        cancellationToken);


                if (balance != null)
                {
                    balance.UsedDays += leave.TotalDays;
                }
            }



            if (leave.SickDays > 0)
            {
                var balance =
                    await _context.EmployeeLeaveBalances
                    .FirstOrDefaultAsync(x =>
                        x.EmployeeId == employeeId &&
                        x.LeaveType == LeaveType.Sick &&
                        x.Year == leave.StartDate.Year,
                        cancellationToken);


                if (balance != null)
                {
                    balance.UsedDays += leave.SickDays;
                }
            }



            if (leave.EmergencyDays > 0)
            {
                var balance =
                    await _context.EmployeeLeaveBalances
                    .FirstOrDefaultAsync(x =>
                        x.EmployeeId == employeeId &&
                        x.LeaveType == LeaveType.Emergency &&
                        x.Year == leave.StartDate.Year,
                        cancellationToken);


                if (balance != null)
                {
                    balance.UsedDays += leave.EmergencyDays;
                }
            }
        }
    }
}