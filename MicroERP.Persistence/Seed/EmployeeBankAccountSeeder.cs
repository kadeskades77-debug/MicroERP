

namespace MicroERP.Persistence.Seed
{
    using MicroERP.Domin.Entities.Payrolls;
    using Microsoft.EntityFrameworkCore;

    public static class EmployeeBankAccountSeeder
    {
        public static async Task Seed(
            ApplicationDbContext context)
        {
            if (await context.EmployeeBankAccounts.AnyAsync())
                return;



            var employees =
                await context.Employees
                .ToListAsync();



            var accounts = new List<EmployeeBankAccount>();



            foreach (var employee in employees)
            {
                accounts.Add(
                    new EmployeeBankAccount
                    {
                        EmployeeId = employee.Id,

                        BankName = "البنك اليمني للتنمية",

                        BranchName = "الفرع الرئيسي",

                        AccountNumber =
                            $"1000{employee.Id}0001",

                        IBAN =
                            $"YE00YEME000000000{employee.Id}",

                        IsPrimary = true,

                        IsActive = true
                    });
            }



            await context.EmployeeBankAccounts
                .AddRangeAsync(accounts);



            await context.SaveChangesAsync();
        }
    }
}
