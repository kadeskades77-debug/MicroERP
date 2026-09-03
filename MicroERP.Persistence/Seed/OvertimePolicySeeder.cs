
using MicroERP.Domin.Entities.Policies;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Persistence.Seed;

public static class OvertimePolicySeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context)
    {
        if (await context.OvertimePolicies.AnyAsync())
            return;


        var policy = new OvertimePolicy
        {
            Name = "Default Overtime Policy",

            NormalMultiplier = 1.25m,

            WeekendMultiplier = 1.50m,

            HolidayMultiplier = 2.00m,

            WorkingHoursPerDay = 8,

            MaxHoursPerDay = 4,

            RequireApproval = true,

            IsDefault = true
        };


        await context.OvertimePolicies
            .AddAsync(policy);


        await context.SaveChangesAsync();
    }
}