using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Features.Payrolls.Interfaces;
using MicroERP.Domin.Entities.Payrolls;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.Payrolls.Services;

public class UnpaidLeavePayrollCalculator
    : IUnpaidLeavePayrollCalculator
{
    private readonly IApplicationDbContext _context;


    public UnpaidLeavePayrollCalculator(
        IApplicationDbContext context)
    {
        _context = context;
    }



    public async Task<List<PayrollItem>> CalculateAsync(
        int employeeId,
        DateOnly startDate,
        DateOnly endDate,
        decimal basicSalary,
        CancellationToken cancellationToken = default)
    {
        var items = new List<PayrollItem>();


        var unpaidDays =
            await _context.EmployeeLeaves
            .Where(x =>
                x.EmployeeId == employeeId &&
                x.Status == LeaveStatus.Approved &&
                x.StartDate <= endDate &&
                x.EndDate >= startDate)
            .SumAsync(
                x => x.UnpaidDays,
                cancellationToken);



        if (unpaidDays <= 0)
            return items;



        // قيمة اليوم حسب الراتب الشهري
        var dailyRate =
            basicSalary / 30m;



        var deduction =
            unpaidDays * dailyRate;



        var component =
            await _context.SalaryComponents
            .FirstOrDefaultAsync(
                x => x.Code == "UNPAID",
                cancellationToken);



        if (component == null)
            return items;



        items.Add(
            new PayrollItem
            {
                SalaryComponentId =
                    component.Id,

                Amount =
                    deduction,

                ItemName =
                    component.NameAr,

                Description =
                    $"Unpaid leave deduction ({unpaidDays} days)",

                Source =
                    PayrollItemSource.Leave,

                Type =
                    SalaryComponentType.Deduction
            });



        return items;
    }
}