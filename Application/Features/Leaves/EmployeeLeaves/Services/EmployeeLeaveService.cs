using MicroERP.Application.Common.Files.Interfaces;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Mappings;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Leaves.EmployeeLeaveBalances.Interfaces;
using MicroERP.Application.Features.Leaves.EmployeeLeaves.DTOs;
using MicroERP.Application.Features.Leaves.EmployeeLeaves.Interfaces;
using MicroERP.Domin.Entities;
using MicroERP.Domin.Entities.EmployeeLeaves;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;


namespace MicroERP.Application.Features.Leaves.EmployeeLeaves.Services;

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
    public async Task<Result<LeaveDto>> CreateAsync(
    string userId,
    CreateLeaveDto dto,
    CancellationToken cancellationToken = default)
    {
        if (dto.StartDate > dto.EndDate)
            return Result<LeaveDto>.Failure(
                "Start date cannot be after end date");

        var employeeResult = await GetEmployeeIdAsync(
       userId,
         cancellationToken);

        if (!employeeResult.Success)
        {
            return Result<LeaveDto>.Failure(
                employeeResult.Message);
        }

        var employeeId = employeeResult.Data!;




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
            await CalculateLeaveDaysAsync(
                employeeId,
                dto.StartDate,
                dto.EndDate,
                cancellationToken);



        if (totalDays == 0)
        {
            return Result<LeaveDto>.Failure(
                "Selected period contains only weekends or holidays.");
        }



        var policy = await _context.LeavePolicies
            .FirstOrDefaultAsync(
                x => x.LeaveType == dto.LeaveType,
                cancellationToken);



        if (policy == null)
            return Result<LeaveDto>.Failure(
                "Leave policy not found");



        int sickDays = 0;

        int emergencyDays = 0;

        int annualDays = 0;

        int unpaidDays = 0;



        bool hasWarning = false;

        string? warningMessage = null;



        // =====================================
        // Leave without balance
        // =====================================

        if (!policy.RequiresBalance)
        {
            unpaidDays =
                dto.LeaveType == LeaveType.Unpaid
                ? totalDays
                : 0;
        }



        // =====================================
        // Leave with balance
        // =====================================

        else
        {

            // =====================================
            // Sick -> Emergency -> Annual -> Unpaid
            // =====================================

            if (dto.LeaveType == LeaveType.Sick)
            {
                var remainingDays = totalDays;



                // Sick Balance
                var sickBalance =
                    await GetBalanceAsync(
                        employeeId,
                        LeaveType.Sick,
                        dto.StartDate.Year,
                        cancellationToken);



                var sickRemaining =
                    Math.Max(
                        0,
                        sickBalance.TotalDays -
                        sickBalance.UsedDays);



                sickDays =
                    Math.Min(
                        sickRemaining,
                        remainingDays);



                remainingDays -= sickDays;



                // Emergency Balance
                if (remainingDays > 0)
                {
                    var emergencyBalance =
                        await GetBalanceAsync(
                            employeeId,
                            LeaveType.Emergency,
                            dto.StartDate.Year,
                            cancellationToken);



                    var emergencyRemaining =
                        Math.Max(
                            0,
                            emergencyBalance.TotalDays -
                            emergencyBalance.UsedDays);



                    emergencyDays =
                        Math.Min(
                            emergencyRemaining,
                            remainingDays);



                    remainingDays -= emergencyDays;
                }



                // Annual Balance
                if (remainingDays > 0)
                {
                    var annualBalance =
                        await GetBalanceAsync(
                            employeeId,
                            LeaveType.Annual,
                            dto.StartDate.Year,
                            cancellationToken);



                    var annualRemaining =
                        Math.Max(
                            0,
                            annualBalance.TotalDays -
                            annualBalance.UsedDays);



                    annualDays =
                        Math.Min(
                            annualRemaining,
                            remainingDays);



                    remainingDays -= annualDays;
                }



                // Unpaid
                if (remainingDays > 0)
                {
                    unpaidDays = remainingDays;
                }



                if (emergencyDays > 0 ||
                    annualDays > 0 ||
                    unpaidDays > 0)
                {
                    hasWarning = true;

                    warningMessage =
                        $"Sick: {sickDays} days, " +
                        $"Emergency: {emergencyDays} days, " +
                        $"Annual: {annualDays} days, " +
                        $"Unpaid: {unpaidDays} days.";
                }
            }
   

            // =====================================
            // Emergency Leave
            // =====================================

            else if (dto.LeaveType == LeaveType.Emergency)
            {
                var emergencyBalance =
                    await GetBalanceAsync(
                        employeeId,
                        LeaveType.Emergency,
                        dto.StartDate.Year,
                        cancellationToken);



                var emergencyRemaining =
                    Math.Max(
                        0,
                        emergencyBalance.TotalDays -
                        emergencyBalance.UsedDays);



                if (emergencyRemaining < totalDays)
                {
                    return Result<LeaveDto>.Failure(
                        "Emergency leave balance is not enough.");
                }



                var annualBalance =
                    await GetBalanceAsync(
                        employeeId,
                        LeaveType.Annual,
                        dto.StartDate.Year,
                        cancellationToken);



                var annualRemaining =
                    Math.Max(
                        0,
                        annualBalance.TotalDays -
                        annualBalance.UsedDays);



                if (annualRemaining < totalDays)
                {
                    return Result<LeaveDto>.Failure(
                        "Annual leave balance must cover emergency leave days.");
                }



                emergencyDays = totalDays;
            }



            // =====================================
            // Annual Leave
            // =====================================

            else if (dto.LeaveType == LeaveType.Annual)
            {
                var annualBalance =
                    await GetBalanceAsync(
                        employeeId,
                        LeaveType.Annual,
                        dto.StartDate.Year,
                        cancellationToken);


                var annualRemaining =
                    Math.Max(
                        0,
                        annualBalance.TotalDays -
                        annualBalance.UsedDays);



                if (annualRemaining < totalDays)
                {
                    var shortage =
                        totalDays - annualRemaining;


                    if (!policy.AllowNegativeBalance ||
                        shortage > policy.MaxNegativeDays)
                    {
                        return Result<LeaveDto>.Failure(
                            "Insufficient annual leave balance");
                    }


                    hasWarning = true;

                    warningMessage =
                        $"Annual balance shortage {shortage} days.";
                }


                annualDays = totalDays;
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

            AnnualDays = annualDays,

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
    public async Task<Result<LeaveDto>> UpdateAsync(int id,
     UpdateLeaveDto dto,
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



        var startDate =
            dto.StartDate ?? leave.StartDate;


        var endDate =
            dto.EndDate ?? leave.EndDate;



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



        // إعادة الحساب مع استبعاد الجمعة والعطل الرسمية
        leave.TotalDays =
          await CalculateLeaveDaysAsync(
              leave.EmployeeId,
              leave.StartDate,
              leave.EndDate,
              cancellationToken);



        if (leave.TotalDays == 0)
        {
            return Result<LeaveDto>.Failure(
                "Selected period contains only weekends or holidays.");
        }



        // إعادة توزيع الرصيد
        leave.SickDays = 0;
        leave.EmergencyDays = 0;
        leave.AnnualDays = 0;
        leave.UnpaidDays = 0;



        if (leave.LeaveType == LeaveType.Sick)
        {
            var remainingDays = leave.TotalDays;



            var sickBalance =
                await GetBalanceAsync(
                    leave.EmployeeId,
                    LeaveType.Sick,
                    leave.StartDate.Year,
                    cancellationToken);



            var sickRemaining =
                Math.Max(
                    0,
                    sickBalance.TotalDays -
                    sickBalance.UsedDays);



            leave.SickDays =
                Math.Min(
                    sickRemaining,
                    remainingDays);



            remainingDays -= leave.SickDays;



            if (remainingDays > 0)
            {
                var emergencyBalance =
                    await GetBalanceAsync(
                        leave.EmployeeId,
                        LeaveType.Emergency,
                        leave.StartDate.Year,
                        cancellationToken);



                var emergencyRemaining =
                    Math.Max(
                        0,
                        emergencyBalance.TotalDays -
                        emergencyBalance.UsedDays);



                leave.EmergencyDays =
                    Math.Min(
                        emergencyRemaining,
                        remainingDays);



                remainingDays -= leave.EmergencyDays;
            }



            if (remainingDays > 0)
            {
                var annualBalance =
                    await GetBalanceAsync(
                        leave.EmployeeId,
                        LeaveType.Annual,
                        leave.StartDate.Year,
                        cancellationToken);



                var annualRemaining =
                    Math.Max(
                        0,
                        annualBalance.TotalDays -
                        annualBalance.UsedDays);



                leave.AnnualDays =
                    Math.Min(
                        annualRemaining,
                        remainingDays);



                remainingDays -= leave.AnnualDays;
            }



            if (remainingDays > 0)
            {
                leave.UnpaidDays = remainingDays;
            }
        }



        await _context.SaveChangesAsync(
            cancellationToken);



        var updatedLeave =
            await _queries.GetByIdAsync(
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



        leave.IsDeleted = true;

        leave.IsActive = false;



        await _context.SaveChangesAsync(
            cancellationToken);



        return Result.Succeeded(
            "Leave request deleted successfully.");
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
            // =====================================
            // Sick Balance
            // =====================================

            if (leave.SickDays > 0)
            {
                var sickBalance =
                    await GetBalanceAsync(
                        leave.EmployeeId,
                        LeaveType.Sick,
                        leave.StartDate.Year,
                        cancellationToken);

                sickBalance.UsedDays += leave.SickDays;
            }



            // =====================================
            // Emergency Balance
            // =====================================

            if (leave.EmergencyDays > 0)
            {
                var emergencyBalance =
                    await GetBalanceAsync(
                        leave.EmployeeId,
                        LeaveType.Emergency,
                        leave.StartDate.Year,
                        cancellationToken);

                emergencyBalance.UsedDays += leave.EmergencyDays;
            }



            // =====================================
            // Annual Balance
            // =====================================

            if (leave.AnnualDays > 0)
            {
                // إذا كانت الإجازة سنوية فقد تكون موزعة على أكثر من سنة
                if (leave.LeaveType == LeaveType.Annual)
                {
                    var annualDaysByYear =
                        await CalculateLeaveDaysByYearAsync(
                            leave.EmployeeId,
                            leave.StartDate,
                            leave.EndDate,
                            cancellationToken);

                    foreach (var item in annualDaysByYear)
                    {
                        var annualBalance =
                            await GetBalanceAsync(
                                leave.EmployeeId,
                                LeaveType.Annual,
                                item.Year,
                                cancellationToken);

                        annualBalance.UsedDays += item.Days;
                    }
                }
                else
                {
                    var annualBalance =
                        await GetBalanceAsync(
                            leave.EmployeeId,
                            LeaveType.Annual,
                            leave.StartDate.Year,
                            cancellationToken);

                    var remainingAnnual =
                        annualBalance.TotalDays -
                        annualBalance.UsedDays;

                    if (leave.AnnualDays > remainingAnnual)
                    {
                        throw new InvalidOperationException(
                            "Annual leave balance is not enough to cover the remaining sick leave.");
                    }

                    annualBalance.UsedDays += leave.AnnualDays;
                }
            }



            // =====================================
            // Unpaid
            // =====================================
            // لا يوجد أي رصيد يتم خصمه



            leave.Status = LeaveStatus.Approved;

            leave.ApprovedByUserId = userId;

            leave.ApprovedOn = DateTime.UtcNow;



            await _context.SaveChangesAsync(
                cancellationToken);



            await transaction.CommitAsync(
                cancellationToken);



            var result =
                await _queries.GetByIdAsync(
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
    public async Task<Result<LeaveDto>> RejectAsync(
       int id,
       string userId,
       RejectLeaveDto dto,
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



        if (string.IsNullOrWhiteSpace(dto.RejectionReason))
        {
            return Result<LeaveDto>.Failure(
                "Rejection reason is required.");
        }



        leave.Status =
            LeaveStatus.Rejected;



        leave.RejectedByUserId =
            userId;



        leave.RejectedOn =
            DateTime.UtcNow;



        leave.RejectionReason =
            dto.RejectionReason;



        await _context.SaveChangesAsync(
            cancellationToken);



        var rejectedLeave =
            await _queries.GetByIdAsync(
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
        {
            return Result<LeaveDto>.Failure(
                "Only approved leaves can be cancelled.");
        }



        var today = DateOnly.FromDateTime(
            DateTime.UtcNow);



        if (today >= leave.StartDate)
        {
            return Result<LeaveDto>.Failure(
                "Cannot cancel a leave that has already started.");
        }



        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);



        try
        {
            // =====================================
            // Restore Sick Balance
            // =====================================

            if (leave.SickDays > 0)
            {
                var sickBalance =
                    await GetBalanceAsync(
                        leave.EmployeeId,
                        LeaveType.Sick,
                        leave.StartDate.Year,
                        cancellationToken);

                sickBalance.UsedDays -= leave.SickDays;

                if (sickBalance.UsedDays < 0)
                    sickBalance.UsedDays = 0;
            }



            // =====================================
            // Restore Emergency Balance
            // =====================================

            if (leave.EmergencyDays > 0)
            {
                var emergencyBalance =
                    await GetBalanceAsync(
                        leave.EmployeeId,
                        LeaveType.Emergency,
                        leave.StartDate.Year,
                        cancellationToken);

                emergencyBalance.UsedDays -= leave.EmergencyDays;

                if (emergencyBalance.UsedDays < 0)
                    emergencyBalance.UsedDays = 0;
            }



            // =====================================
            // Restore Annual Balance
            // =====================================

            if (leave.AnnualDays > 0)
            {
                // إذا كانت الإجازة سنوية فقد تكون موزعة على أكثر من سنة
                if (leave.LeaveType == LeaveType.Annual)
                {
                    var annualDaysByYear =
                        await CalculateLeaveDaysByYearAsync(
                            leave.EmployeeId,
                            leave.StartDate,
                            leave.EndDate,
                            cancellationToken);

                    foreach (var item in annualDaysByYear)
                    {
                        var annualBalance =
                            await GetBalanceAsync(
                                leave.EmployeeId,
                                LeaveType.Annual,
                                item.Year,
                                cancellationToken);

                        annualBalance.UsedDays -= item.Days;

                        if (annualBalance.UsedDays < 0)
                            annualBalance.UsedDays = 0;
                    }
                }
                else
                {
                    // الإجازة المرضية التي استهلكت من الرصيد السنوي
                    var annualBalance =
                        await GetBalanceAsync(
                            leave.EmployeeId,
                            LeaveType.Annual,
                            leave.StartDate.Year,
                            cancellationToken);

                    annualBalance.UsedDays -= leave.AnnualDays;

                    if (annualBalance.UsedDays < 0)
                        annualBalance.UsedDays = 0;
                }
            }



            // =====================================
            // Unpaid
            // =====================================
            // لا يوجد رصيد يتم إرجاعه



            leave.Status = LeaveStatus.Cancelled;

            leave.ModifiedBy = userId;

            leave.ModifiedOn = DateTime.UtcNow;



            await _context.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);



            var result =
                await _queries.GetByIdAsync(
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

    private async Task<EmployeeLeaveBalance> GetBalanceAsync(
     int employeeId,
     LeaveType leaveType,
     int year,
     CancellationToken cancellationToken)
    {
        var balance = await _context.EmployeeLeaveBalances
            .FirstOrDefaultAsync(x =>
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
                .FirstOrDefaultAsync(x =>
                    x.EmployeeId == employeeId &&
                    x.Year == year &&
                    x.LeaveType == leaveType,
                    cancellationToken);
        }

        if (balance == null)
            throw new Exception("Leave balance not found");

        return balance;
    }


    private async Task<int> CalculateLeaveDaysAsync(
     int employeeId,
     DateOnly startDate,
     DateOnly endDate,
     CancellationToken cancellationToken)
    {
        var holidays =
            await _context.Holidays
            .Where(x =>
                x.StartDate <= endDate &&
                x.EndDate >= startDate)
            .ToListAsync(cancellationToken);



        var specialLeaves =
            await _context.EmployeeSpecialLeaves
            .Where(x =>
                x.EmployeeId == employeeId &&
                x.Status == SpecialLeaveStatus.Approved &&
                x.StartDate <= endDate &&
                x.EndDate >= startDate)
            .ToListAsync(cancellationToken);



        var totalDays = 0;



        for (var date = startDate;
             date <= endDate;
             date = date.AddDays(1))
        {
            // استبعاد الجمعة
            if (date.DayOfWeek == DayOfWeek.Friday)
                continue;



            // استبعاد العطل الرسمية
            if (holidays.Any(x =>
                    x.StartDate <= date &&
                    x.EndDate >= date))
            {
                continue;
            }



            // استبعاد الإجازات الخاصة
            if (specialLeaves.Any(x =>
                    x.StartDate <= date &&
                    x.EndDate >= date))
            {
                continue;
            }



            totalDays++;
        }



        return totalDays;
    }

    private async Task<List<(int Year, int Days)>> CalculateLeaveDaysByYearAsync(
     int employeeId,
     DateOnly startDate,
     DateOnly endDate,
     CancellationToken cancellationToken)
    {
        var holidays =
            await _context.Holidays
            .Where(x =>
                x.StartDate <= endDate &&
                x.EndDate >= startDate)
            .ToListAsync(cancellationToken);



        var specialLeaves =
            await _context.EmployeeSpecialLeaves
            .Where(x =>
                x.EmployeeId == employeeId &&
                x.Status == SpecialLeaveStatus.Approved &&
                x.StartDate <= endDate &&
                x.EndDate >= startDate)
            .ToListAsync(cancellationToken);



        var result =
            new Dictionary<int, int>();



        for (var date = startDate;
             date <= endDate;
             date = date.AddDays(1))
        {
            // الجمعة
            if (date.DayOfWeek == DayOfWeek.Friday)
                continue;



            // العطل الرسمية
            if (holidays.Any(x =>
                    x.StartDate <= date &&
                    x.EndDate >= date))
            {
                continue;
            }



            // الإجازات الخاصة
            if (specialLeaves.Any(x =>
                    x.StartDate <= date &&
                    x.EndDate >= date))
            {
                continue;
            }



            if (!result.ContainsKey(date.Year))
                result[date.Year] = 0;



            result[date.Year]++;
        }



        return result
            .Select(x => (x.Key, x.Value))
            .ToList();
    }

    private async Task<Result<int>> GetEmployeeIdAsync(
      string userId,
      CancellationToken cancellationToken = default)
    {
        var employee = await _context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.UserId == userId,
                cancellationToken);

        if (employee == null)
        {
            return Result<int>.Failure(
                "Employee profile not found.");
        }

        return Result<int>.Succeeded(employee.Id);
    }



}