using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Features.Leaves.EmployeeLeaveBalances.Interfaces;
using MicroERP.Domin.Entities.EmployeeLeaves;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.Leaves.EmployeeLeaveBalances.Services;

public class LeaveBalanceGeneratorService : ILeaveBalanceGenerator
{
    private readonly IApplicationDbContext _context;

    public LeaveBalanceGeneratorService(
        IApplicationDbContext context)
    {
        _context = context;
    }


    // Generate leave balances for a single employee
    public async Task GenerateForEmployeeAsync(
    int employeeId,
    int year,
    CancellationToken cancellationToken = default)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(
                x => x.Id == employeeId,
                cancellationToken);


        if (employee == null)
            throw new Exception("Employee not found");



        // الموظف لم يكن موظفاً في هذه السنة
        if (employee.HireDate.Year > year)
            return;



        var policies = await _context.LeavePolicies
            .AsNoTracking()
            .ToListAsync(cancellationToken);



        foreach (var policy in policies)
        {
            if (!policy.RequiresBalance)
                continue;



            var exists = await _context.EmployeeLeaveBalances
                .AnyAsync(
                    x =>
                        x.EmployeeId == employeeId &&
                        x.Year == year &&
                        x.LeaveType == policy.LeaveType,
                    cancellationToken);



            if (exists)
                continue;



            int totalDays;



            // موظف موجود قبل بداية السنة
            if (employee.HireDate.Year < year)
            {
                totalDays =
                    policy.MaximumDaysPerYear;
            }


            // موظف تم توظيفه خلال نفس السنة
            else
            {
                var remainingMonths =
                    12 - employee.HireDate.Month + 1;



                // الإجازات الشهرية
                // Annual = 2.5 لكل شهر
                // Sick = 2 لكل شهر
                if (policy.LeaveType == LeaveType.Annual ||
                    policy.LeaveType == LeaveType.Sick)
                {
                    totalDays =
                        (int)Math.Round(
                            remainingMonths * policy.DaysPerMonth,
                            MidpointRounding.AwayFromZero);
                }


                // Emergency ثابت حسب البوليسي
                else
                {
                    totalDays =
                        policy.MaximumDaysPerYear;
                }
            }



            // منع تجاوز الحد الأعلى في البوليسي
            totalDays =
                Math.Min(
                    totalDays,
                    policy.MaximumDaysPerYear);



            _context.EmployeeLeaveBalances.Add(
                new EmployeeLeaveBalance
                {
                    EmployeeId = employeeId,

                    Year = year,

                    LeaveType = policy.LeaveType,

                    TotalDays = totalDays,

                    UsedDays = 0
                });
        }



        await _context.SaveChangesAsync(
            cancellationToken);
    }


    // Generate leave balances for all active employees
    public async Task GenerateForAllEmployeesAsync(int year,
        CancellationToken cancellationToken = default)
    {
        var employeeIds = await _context.Employees
            .Where(x => x.IsActive)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);


        foreach (var employeeId in employeeIds)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await GenerateForEmployeeAsync(
                employeeId,
                year,
                cancellationToken);
        }
    }

    // Generate leave balances for a specific year
    public async Task GenerateForYearAsync(int year,
        CancellationToken cancellationToken = default)
    {
        var hasBalances = await _context.EmployeeLeaveBalances
            .AnyAsync(
                x => x.Year == year,
                cancellationToken);

        if (hasBalances)
            return;

        await GenerateForAllEmployeesAsync(
            year,
            cancellationToken);
    }
}