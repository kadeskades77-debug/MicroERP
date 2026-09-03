using MicroERP.Application.Common.Interfaces;
using MicroERP.Domin.Entities;
using MicroERP.Domin.Entities.EmployeeAttendance;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Persistence.Seed;

public static class AttendanceTestDataSeeder
{
    public static async Task SeedAsync(
        IApplicationDbContext context,
        CancellationToken cancellationToken = default)
    {
        // إنشاء جهاز البصمة
        var device = await context.AttendanceDevices
            .FirstOrDefaultAsync(
                x => x.DeviceCode == "TEST-DEVICE-001",
                cancellationToken);


        if (device == null)
        {
            device = new AttendanceDevice
            {
                Name = "Test Attendance Device",

                DeviceCode = "TEST-DEVICE-001",

                IpAddress = "192.168.1.10",

                Location = "HR",

                IsActiveDevice = true,

                ConnectionType = DeviceConnectionType.Network
            };


            context.AttendanceDevices.Add(device);

            await context.SaveChangesAsync(
                cancellationToken);
        }



        var employees = await context.Employees
            .Where(x =>
                x.Id >= 1 &&
                x.Id <= 4)
            .ToListAsync(cancellationToken);



        foreach (var employee in employees)
        {
            var exists =
                await context.EmployeeAttendanceDevices
                .AnyAsync(
                    x =>
                    x.EmployeeId == employee.Id &&
                    x.AttendanceDeviceId == device.Id,
                    cancellationToken);



            if (!exists)
            {
                context.EmployeeAttendanceDevices.Add(
                    new EmployeeAttendanceDevice
                    {
                        EmployeeId = employee.Id,

                        AttendanceDeviceId = device.Id,

                        DeviceEmployeeId =
                            employee.Id.ToString("000")
                    });
            }
        }


        await context.SaveChangesAsync(
            cancellationToken);



        var startDate =
            new DateOnly(2026, 7, 1);


        var endDate =
            DateOnly.FromDateTime(
                DateTime.Today);



        for (
            var date = startDate;
            date <= endDate;
            date = date.AddDays(1))
        {

            // تجاهل الجمعة والسبت
            if (date.DayOfWeek ==
                DayOfWeek.Friday ||
                date.DayOfWeek ==
                DayOfWeek.Saturday)
            {
                continue;
            }



            foreach (var employee in employees)
            {

                var deviceEmployeeCode =
                    employee.Id.ToString("000");



                var alreadyExists =
                    await context.AttendanceLogs
                    .AnyAsync(
                        x =>
                        x.AttendanceDeviceId == device.Id &&
                        x.DeviceEmployeeId ==
                            deviceEmployeeCode &&
                        DateOnly.FromDateTime(
                            x.LogTime) == date,
                        cancellationToken);



                if (alreadyExists)
                    continue;



                // دخول الفترة الأولى
                context.AttendanceLogs.Add(
                    new AttendanceLog
                    {
                        AttendanceDeviceId =
                            device.Id,

                        DeviceEmployeeId =
                            deviceEmployeeCode,

                        LogTime =
                            date.ToDateTime(
                                new TimeOnly(8, 0)),

                        Type =
                            AttendanceLogType.CheckIn
                    });



                // خروج الفترة الأولى
                context.AttendanceLogs.Add(
                    new AttendanceLog
                    {
                        AttendanceDeviceId =
                            device.Id,

                        DeviceEmployeeId =
                            deviceEmployeeCode,

                        LogTime =
                            date.ToDateTime(
                                new TimeOnly(12, 0)),

                        Type =
                            AttendanceLogType.CheckOut
                    });



                // دخول الفترة الثانية
                context.AttendanceLogs.Add(
                    new AttendanceLog
                    {
                        AttendanceDeviceId =
                            device.Id,

                        DeviceEmployeeId =
                            deviceEmployeeCode,

                        LogTime =
                            date.ToDateTime(
                                new TimeOnly(14, 0)),

                        Type =
                            AttendanceLogType.CheckIn
                    });



                // خروج الفترة الثانية
                context.AttendanceLogs.Add(
                    new AttendanceLog
                    {
                        AttendanceDeviceId =
                            device.Id,

                        DeviceEmployeeId =
                            deviceEmployeeCode,

                        LogTime =
                            date.ToDateTime(
                                new TimeOnly(16, 30)),

                        Type =
                            AttendanceLogType.CheckOut
                    });
            }
        }



        await context.SaveChangesAsync(
            cancellationToken);
    }
}