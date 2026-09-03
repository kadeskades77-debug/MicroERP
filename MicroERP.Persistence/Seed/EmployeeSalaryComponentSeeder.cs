using Microsoft.EntityFrameworkCore;
using MicroERP.Domin.Entities.Payrolls;

namespace MicroERP.Persistence.Seed;

public static class EmployeeSalaryComponentSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context)
    {
        if (await context.EmployeeSalaryComponents.AnyAsync())
            return;


        // =========================================================
        // Salary Components
        // =========================================================

        var components =
            await context.SalaryComponents
                .ToDictionaryAsync(
                    x => x.Code,
                    x => x.Id);


        // =========================================================
        // Employees
        // =========================================================

        var employees =
            await context.Employees
                .Select(x => new
                {
                    x.Id,
                    x.Salary
                })
                .ToListAsync();


        var effectiveFrom =
            new DateOnly(2026, 1, 1);


        var data =
            new List<EmployeeSalaryComponent>();


        var random =
            new Random();


        // =========================================================
        // Helper
        // =========================================================

        void AddComponent(
            int employeeId,
            string code,
            decimal amount)
        {
            data.Add(
                new EmployeeSalaryComponent
                {
                    EmployeeId =
                        employeeId,

                    SalaryComponentId =
                        components[code],

                    Amount =
                        amount,

                    EffectiveFrom =
                        effectiveFrom,

                    EffectiveTo =
                        null,

                    IsActiveComponent =
                        true
                });
        }


        // =========================================================
        // Seed Components For Every Employee
        // =========================================================

        foreach (var employee in employees)
        {
            // =====================================================
            // Basic Salary
            // From Employees table
            // =====================================================

            AddComponent(
                employee.Id,
                "BASIC",
                employee.Salary);


            // =====================================================
            // Housing
            // 30,000 - 70,000
            // =====================================================

            var housing =
                random.Next(
                    30000,
                    70001);

            AddComponent(
                employee.Id,
                "HOUSING",
                housing);


            // =====================================================
            // Transport
            // 10,000 - 20,000
            // =====================================================

            var transport =
                random.Next(
                    10000,
                    20001);

            AddComponent(
                employee.Id,
                "TRANSPORT",
                transport);


            // =====================================================
            // Tax
            // 10%
            // =====================================================

            AddComponent(
                employee.Id,
                "TAX",
                10);


            // =====================================================
            // Insurance
            // 10%
            // =====================================================

            AddComponent(
                employee.Id,
                "INSURANCE",
                10);
        }


        // =========================================================
        // Save
        // =========================================================

        await context.EmployeeSalaryComponents
            .AddRangeAsync(data);

        await context.SaveChangesAsync();
    }
}