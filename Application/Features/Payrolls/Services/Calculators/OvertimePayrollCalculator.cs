using MicroERP.Application.Common.Interfaces;

using MicroERP.Domin.Entities.Payrolls;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.Payrolls.Services.Calculators;

public class OvertimePayrollCalculator
{
    private readonly IApplicationDbContext _context;


    public OvertimePayrollCalculator(
        IApplicationDbContext context)
    {
        _context = context;
    }



    public async Task<List<PayrollItem>> CalculateAsync(
        int employeeId,
        int payrollId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken)
    {
        var overtimes =
            await _context.EmployeeOvertimes
            .Where(x =>
                x.EmployeeId == employeeId &&
                x.StartDateTime >= startDate &&
                x.StartDateTime <= endDate &&
                x.Status == OvertimeStatus.Approved &&
                !x.IsPaid)
            .ToListAsync(cancellationToken);



        if (!overtimes.Any())
            return new List<PayrollItem>();



        var items = new List<PayrollItem>();



        foreach (var overtime in overtimes)
        {
            var item = new PayrollItem
            {
                PayrollId = payrollId,

                SalaryComponentId = null,

                ItemName =
                    "Overtime",

                Description =
                    $"Overtime {overtime.StartDateTime}",

                Amount =
                    overtime.Amount,

                Source =
                    PayrollItemSource.Overtime,

                Type =
                    SalaryComponentType.Allowance
            };


            items.Add(item);


            overtime.IsPaid = true;

            overtime.PayrollItem = item;
        }



        return items;
    }
}