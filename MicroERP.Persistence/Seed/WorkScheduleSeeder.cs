using MicroERP.Application.Common.Interfaces;
using MicroERP.Domin.Entities;
using MicroERP.Domin.Entities.EmployeeAttendance;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Persistence.Seed;

public static class WorkScheduleSeeder
{
    public static async Task SeedAsync(
        IApplicationDbContext context,
        CancellationToken cancellationToken = default)
    {
        var schedule = await context.WorkSchedules
            .FirstOrDefaultAsync(
                x => x.Name == "Default Shift",
                cancellationToken);



        if (schedule == null)
        {
            schedule = new WorkSchedule
            {
                Name = "Default Shift",

                // الفترة الأولى
                FirstShiftStart = new TimeOnly(8, 0),

                FirstShiftEnd = new TimeOnly(12, 0),


                // الفترة الثانية
                SecondShiftStart = new TimeOnly(14, 0),

                SecondShiftEnd = new TimeOnly(16, 30),


                LateGraceMinutes = 10,

                EarlyLeaveGraceMinutes = 10,

                MinimumWorkMinutes = 420,

                IsDefault = true
            };


            context.WorkSchedules.Add(schedule);
        }
        else
        {
            // تحديث الجدول الموجود
            schedule.FirstShiftStart = new TimeOnly(8, 0);

            schedule.FirstShiftEnd = new TimeOnly(12, 0);


            schedule.SecondShiftStart = new TimeOnly(14, 0);

            schedule.SecondShiftEnd = new TimeOnly(16, 30);


            schedule.LateGraceMinutes = 10;

            schedule.EarlyLeaveGraceMinutes = 10;

            schedule.MinimumWorkMinutes = 420;

            schedule.IsDefault = true;
        }



        await context.SaveChangesAsync(
            cancellationToken);



        // ربط الموظف بالدوام
        var employee = await context.Employees
            .FirstOrDefaultAsync(
                x => x.Id == 1,
                cancellationToken);



        if (employee != null &&
            employee.WorkScheduleId != schedule.Id)
        {
            employee.WorkScheduleId = schedule.Id;


            await context.SaveChangesAsync(
                cancellationToken);
        }
    }
}