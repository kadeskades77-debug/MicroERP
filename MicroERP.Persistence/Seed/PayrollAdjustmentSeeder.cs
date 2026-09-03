using Microsoft.EntityFrameworkCore;
using MicroERP.Domin.Entities.Payrolls;
using MicroERP.Domin.Enums;

namespace MicroERP.Persistence.Seed;

public static class PayrollAdjustmentSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context)
    {
        if (await context.PayrollAdjustments.AnyAsync())
            return;



        var adjustments = new List<PayrollAdjustment>
        {

            //================ Employee 1 =================

            new()
            {
                EmployeeId = 1,

                PayrollPeriodId = 1,

                SalaryComponentId = null,

                Type = AdjustmentType.Bonus,

                Title = "مكافأة أداء",

                Notes = "مكافأة شهرية",

                Amount = 500,

                IsApplied = false
            },



            //================ Employee 2 =================

            new()
            {
                EmployeeId = 2,

                PayrollPeriodId = 1,

                SalaryComponentId = null,

                Type = AdjustmentType.Deduction,

                Title = "خصم إداري",

                Notes = "خصم على الراتب",

                Amount = 150,

                IsApplied = false
            },



            //================ Employee 3 =================

            new()
            {
                EmployeeId = 3,

                PayrollPeriodId = 1,

                SalaryComponentId = null,

                Type = AdjustmentType.Bonus,

                Title = "بدل عمل إضافي",

                Notes = "ساعات إضافية",

                Amount = 300,

                IsApplied = false
            },



            //================ Employee 4 =================

            new()
            {
                EmployeeId = 4,

                PayrollPeriodId = 1,

                SalaryComponentId = null,

                Type = AdjustmentType.Deduction,

                Title = "خصم مخالفة",

                Notes = "خصم حسب النظام",

                Amount = 100,

                IsApplied = false
            }

        };



        await context.PayrollAdjustments
            .AddRangeAsync(adjustments);


        await context.SaveChangesAsync();
    }
}