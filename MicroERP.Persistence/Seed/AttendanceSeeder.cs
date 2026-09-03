using MicroERP.Domin.Entities.EmployeeAttendance;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Persistence.Seed;

public static class AttendanceSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        var employeeExists = await context.Employees
            .AnyAsync(x => x.Id == 1);

        if (!employeeExists)
            return;


        var startDate = new DateOnly(2026, 7, 1);

        var endDate = DateOnly.FromDateTime(DateTime.Now);


        var records = new List<AttendanceRecord>();


        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            var exists = await context.AttendanceRecords
                .AnyAsync(x =>
                    x.EmployeeId == 1 &&
                    x.Date == date);


            if (exists)
                continue;



            var checkIn = new TimeOnly(8, 0);

            var checkOut = new TimeOnly(16, 0);



            records.Add(new AttendanceRecord
            {
                EmployeeId = 1,

                Date = date,

                WorkedMinutes = 480,

                LateMinutes = 0,

                EarlyLeaveMinutes = 0,

                Status = AttendanceStatus.Present,

                Notes = "Seed Data"
            });
        }


        if (records.Any())
        {
            await context.AttendanceRecords.AddRangeAsync(records);

            await context.SaveChangesAsync();
        }
    }
}