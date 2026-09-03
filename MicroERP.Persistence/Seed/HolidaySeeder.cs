using MicroERP.Application.Common.Interfaces;
using MicroERP.Domin.Entities.EmployeeAttendance;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Persistence.Seeder;

public static class HolidaySeeder
{
    public static async Task SeedAsync(
        IApplicationDbContext context)
    {
        if (await context.Holidays.AnyAsync())
            return;



        var year = DateTime.Now.Year;



        var holidays = new List<Holiday>
        {
            new()
            {
                Name = "New Year Holiday",

                StartDate = new DateOnly(
                    year,
                    1,
                    1),

                EndDate = new DateOnly(
                    year,
                    1,
                    1),

                Type = HolidayType.Public,

                IsRecurring = true,

                Notes = "Annual public holiday"
            },


            new()
            {
                Name = "Labour Day",

                StartDate = new DateOnly(
                    year,
                    5,
                    1),

                EndDate = new DateOnly(
                    year,
                    5,
                    1),

                Type = HolidayType.Public,

                IsRecurring = true
            },


            new()
            {
                Name = "National Day",

                StartDate = new DateOnly(
                    year,
                    9,
                    23),

                EndDate = new DateOnly(
                    year,
                    9,
                    23),

                Type = HolidayType.Public,

                IsRecurring = true
            }
        };



        await context.Holidays
            .AddRangeAsync(holidays);



        await context.SaveChangesAsync();
    }
}