
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Services;
using MicroERP.Application.Features.EmployeeAttendance.AttendanceLogs.Interfaces;
using MicroERP.Domin.Entities.EmployeeAttendance;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace MicroERP.Persistence.Seed;

public static class AttendanceLogSeeder
{
    public static async Task SeedAsync(
    IApplicationDbContext context,
    IServiceProvider serviceProvider,
    CancellationToken cancellationToken = default)
    {
        using var scope =
            serviceProvider.CreateScope();

        var ProcessLogs =
            scope.ServiceProvider
                .GetRequiredService<IAttendanceLogProcessor>();

        var device =
            await context.AttendanceDevices
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == 2,
                    cancellationToken);

        if (device == null)
        {
            return;
        }

        await GenerateLogsAsync(
            context,
            device.Id,
            ProcessLogs,
            cancellationToken);
    }

    private static async Task CreateAttendanceLogs(
    IApplicationDbContext context,
    int attendanceDeviceId,
    EmployeeAttendanceDevice deviceEmployee,
    DateOnly date,
    CancellationToken cancellationToken)
    {
        var employee = deviceEmployee.Employee;

        var schedule = employee.WorkSchedule;

        if (schedule == null)
            return;


        var baseDate =
            date.ToDateTime(TimeOnly.MinValue);


        var pattern =
            GetEmployeePattern(
                employee.Id,
                date);


        var logs =
            new List<AttendanceLog>();


        // =========================================================
        // First Shift
        // =========================================================

        var firstCheckIn =
            baseDate.Add(
                schedule.FirstShiftStart.ToTimeSpan());

        var firstCheckOut =
            baseDate.Add(
                schedule.FirstShiftEnd.ToTimeSpan());


        DateTime? secondShiftStart = null;


        if (schedule.SecondShiftStart.HasValue &&
            schedule.SecondShiftEnd.HasValue)
        {
            secondShiftStart =
                baseDate.Add(
                    schedule.SecondShiftStart.Value.ToTimeSpan());
        }


        var firstTransactions =
            ApplyAttendancePattern(
                pattern,
                firstCheckIn,
                firstCheckOut,
                employee.Id,
                date,
                secondShiftStart);


        foreach (var transaction in firstTransactions)
        {
            logs.Add(
                CreateLog(
                    attendanceDeviceId,
                    deviceEmployee.DeviceEmployeeId,
                    transaction.Time,
                    transaction.Type));
        }


        // =========================================================
        // Second Shift
        // =========================================================

        if (schedule.SecondShiftStart.HasValue &&
            schedule.SecondShiftEnd.HasValue)
        {
            var secondCheckIn =
                baseDate.Add(
                    schedule.SecondShiftStart.Value.ToTimeSpan());

            var secondCheckOut =
                baseDate.Add(
                    schedule.SecondShiftEnd.Value.ToTimeSpan());


            var secondTransactions =
                ApplyAttendancePattern(
                    pattern,
                    secondCheckIn,
                    secondCheckOut,
                    employee.Id,
                    date);


            foreach (var transaction in secondTransactions)
            {
                logs.Add(
                    CreateLog(
                        attendanceDeviceId,
                        deviceEmployee.DeviceEmployeeId,
                        transaction.Time,
                        transaction.Type));
            }
        }


        // =========================================================
        // ترتيب جميع البصمات
        // =========================================================

        logs =
            logs
                .OrderBy(x => x.LogTime)
                .ToList();


        // =========================================================
        // Save
        // =========================================================

        await context.AttendanceLogs.AddRangeAsync(
            logs,
            cancellationToken);
    }

    private static List<(AttendanceLogType Type, DateTime Time)>
     ApplyAttendancePattern(
     AttendanceSeedPattern pattern,
     DateTime checkIn,
     DateTime checkOut,
     int employeeId,
     DateOnly date,
     DateTime? nextShiftStart = null)
    {
        var random =
            new Random(
                GetRandomSeed(
                    employeeId,
                    date));

        var transactions =
            new List<(AttendanceLogType Type, DateTime Time)>();


        // =========================================================
        // Check In
        // =========================================================

        switch (pattern)
        {
            // ==========================
            // حضور طبيعي
            // ==========================
            case AttendanceSeedPattern.Normal:

                transactions.Add(
                    (AttendanceLogType.CheckIn, checkIn));

                transactions.Add(
                    (AttendanceLogType.CheckOut, checkOut));

                break;


            // ==========================
            // تأخير بسيط
            // ==========================
            case AttendanceSeedPattern.Late:

                checkIn =
                    checkIn.AddMinutes(
                        random.Next(5, 21));

                transactions.Add(
                    (AttendanceLogType.CheckIn, checkIn));

                transactions.Add(
                    (AttendanceLogType.CheckOut, checkOut));

                break;


            // ==========================
            // تأخير كبير
            // ==========================
            case AttendanceSeedPattern.VeryLate:

                checkIn =
                    checkIn.AddMinutes(
                        random.Next(30, 61));

                transactions.Add(
                    (AttendanceLogType.CheckIn, checkIn));

                transactions.Add(
                    (AttendanceLogType.CheckOut, checkOut));

                break;


            // ==========================
            // خروج مبكر
            // ==========================
            case AttendanceSeedPattern.EarlyLeave:

                checkOut =
                    checkOut.AddMinutes(
                        -random.Next(15, 41));

                transactions.Add(
                    (AttendanceLogType.CheckIn, checkIn));

                transactions.Add(
                    (AttendanceLogType.CheckOut, checkOut));

                break;


            // ==========================
            // تأخير + خروج مبكر
            // ==========================
            case AttendanceSeedPattern.LateAndEarly:

                checkIn =
                    checkIn.AddMinutes(
                        random.Next(10, 31));

                checkOut =
                    checkOut.AddMinutes(
                        -random.Next(15, 41));

                transactions.Add(
                    (AttendanceLogType.CheckIn, checkIn));

                transactions.Add(
                    (AttendanceLogType.CheckOut, checkOut));

                break;


            // ==========================
            // نصف دوام
            // ==========================
            case AttendanceSeedPattern.HalfDay:

                checkOut =
                    checkIn.AddHours(4);

                transactions.Add(
                    (AttendanceLogType.CheckIn, checkIn));

                transactions.Add(
                    (AttendanceLogType.CheckOut, checkOut));

                break;


            // ==========================
            // وقت إضافي
            // ==========================
            case AttendanceSeedPattern.Overtime:
                {
                    transactions.Add(
                        (AttendanceLogType.CheckIn, checkIn));


                    // الخروج الرسمي
                    var firstCheckOut =
                        checkOut;


                    // -------------------------------------------------
                    // لا نسمح لخروج الشفت الأول بالدخول في الشفت الثاني
                    // -------------------------------------------------

                    var overtimeLimit =
                        nextShiftStart.HasValue
                            ? nextShiftStart.Value.AddMinutes(-1)
                            : checkOut.AddHours(3);


                    // -------------------------------------------------
                    // أول خروج
                    // -------------------------------------------------

                    var overtime1 =
                        checkOut.AddMinutes(
                            random.Next(30, 61));

                    if (overtime1 >= overtimeLimit)
                    {
                        overtime1 =
                            overtimeLimit;
                    }


                    // -------------------------------------------------
                    // خروج رسمي
                    // -------------------------------------------------

                    transactions.Add(
                        (AttendanceLogType.CheckOut,
                         firstCheckOut));


                    // -------------------------------------------------
                    // خروج إضافي 1
                    // -------------------------------------------------

                    if (overtime1 > firstCheckOut)
                    {
                        transactions.Add(
                            (AttendanceLogType.CheckOut,
                             overtime1));
                    }


                    // -------------------------------------------------
                    // خروج إضافي 2
                    // -------------------------------------------------

                    var overtime2 =
                        overtime1.AddMinutes(
                            random.Next(20, 61));

                    if (overtime2 < overtimeLimit)
                    {
                        transactions.Add(
                            (AttendanceLogType.CheckOut,
                             overtime2));
                    }


                    break;
                }


            // ==========================
            // حالة مختلطة
            // ==========================
            case AttendanceSeedPattern.Mixed:

                checkIn =
                    checkIn.AddMinutes(
                        random.Next(-5, 45));

                checkOut =
                    checkOut.AddMinutes(
                        random.Next(-30, 121));


                transactions.Add(
                    (AttendanceLogType.CheckIn, checkIn));

                transactions.Add(
                    (AttendanceLogType.CheckOut, checkOut));

                break;
        }


        return transactions
            .OrderBy(x => x.Time)
            .ToList();
    }

    private static AttendanceLog CreateLog(
    int deviceId,
    string deviceEmployeeId,
    DateTime time,
    AttendanceLogType type)
    {
        return new AttendanceLog
        {
            AttendanceDeviceId = deviceId,

            DeviceEmployeeId = deviceEmployeeId,

            LogTime = time,

            Type = type,

            IsProcessed = false
        };
    }

    private static AttendanceSeedPattern GetEmployeePattern(
     int employeeId,
     DateOnly date)
    {
        var random =
            new Random(
                GetRandomSeed(
                    employeeId,
                    date));


        return (AttendanceSeedPattern)
            random.Next(
                Enum.GetValues<AttendanceSeedPattern>().Length);
    }

    private static int GetRandomSeed(
    int employeeId,
    DateOnly date)
    {
        return HashCode.Combine(
            employeeId,
            date.Year,
            date.DayOfYear);
    }



    private static async Task GenerateLogsAsync(
      IApplicationDbContext context,
      int attendanceDeviceId,
      IAttendanceLogProcessor ProcessPendingLogs,
      CancellationToken cancellationToken)
    {
        var startDate = new DateOnly(2026, 7, 1);

        var endDate =
          new DateOnly(2026, 8, 28);
        //var endDate =
        //    DateOnly.FromDateTime(DateTime.Today);

        var employees =
            await context.EmployeeAttendanceDevices
            .Include(x => x.Employee)
                .ThenInclude(x => x.WorkSchedule)
            .Where(x =>
                x.AttendanceDeviceId == attendanceDeviceId &&
                x.Employee.IsActive &&
                x.Employee.WorkScheduleId != null)
            .ToListAsync(cancellationToken);

        
            for (var date = startDate;
                 date <= endDate;
                 date = date.AddDays(1))
            {
           
            foreach (var deviceEmployee in employees)
            {
                var exists =
           await context.AttendanceRecords
             .AnyAsync(
            x => x.EmployeeId == deviceEmployee.EmployeeId &&
                 x.Date == date,
            cancellationToken);

                if (exists)
                {
                    continue;
                }
            
                // الجمعة
                if (date.DayOfWeek == DayOfWeek.Friday)
                {
                    var fridayNumber = GetFridayNumber(date);

                    if (deviceEmployee.EmployeeId == fridayNumber)
                    {
                        await CreateAttendanceLogs(
                            context,
                            attendanceDeviceId,
                            deviceEmployee,
                            date,
                            cancellationToken);
                    }
                    else
                    {
                        if (!exists)
                        {
                            await context.AttendanceRecords.AddAsync(
                  new AttendanceRecord
                  {
                      EmployeeId = deviceEmployee.EmployeeId,
                      Date = date,
                      Status = AttendanceStatus.Weekend
                  },
                  cancellationToken);

                            continue;
                        }
                    }

                    continue;
                }

                // عطلة رسمية
                if (await HasHolidayAsync(
                        context,
                        date,
                        cancellationToken))
                {
                    await CreateAttendanceLogs(
                         context,
                         attendanceDeviceId,
                         deviceEmployee,
                         date,
                         cancellationToken);
                    continue;
                }

                // يوجد سجل بصمة مسبق
                if (await HasAttendanceLogsAsync(
                        context,
                        attendanceDeviceId,
                        deviceEmployee.DeviceEmployeeId,
                        date,
                        cancellationToken))
                {
                    continue;
                }

                // إجازة عادية
                if (await HasNormalLeaveAsync(
                        context,
                        deviceEmployee.EmployeeId,
                        date,
                        cancellationToken))
                {
                    if (!exists)
                    {
                        await context.AttendanceRecords.AddAsync(
              new AttendanceRecord
              {
                  EmployeeId = deviceEmployee.EmployeeId,
                  Date = date,
                  Status = AttendanceStatus.OnLeave
              },
              cancellationToken);

                        continue;
                    }
                }

                // إجازة خاصة
                if (await HasSpecialLeaveAsync(
                        context,
                        deviceEmployee.EmployeeId,
                        date,
                        cancellationToken))
                {
                    if (!exists)
                    {
                        await context.AttendanceRecords.AddAsync(
                        new AttendanceRecord
                        {
                            EmployeeId = deviceEmployee.EmployeeId,
                            Date = date,
                            Status = AttendanceStatus.OnLeave
                        },
                        cancellationToken);

                        continue;
                    }
                }

                await CreateAttendanceLogs(
                    context,
                    attendanceDeviceId,
                    deviceEmployee,
                    date,
                    cancellationToken);

                await context.SaveChangesAsync(cancellationToken);
            

            }
            await ProcessPendingLogs.ProcessPendingLogsAsync(
                               cancellationToken = default);

        }

       
    }

    private static int GetFridayNumber(DateOnly date)
    {
        var fridayNumber = 0;

        for (var d = new DateOnly(date.Year, date.Month, 1);
             d <= date;
             d = d.AddDays(1))
        {
            if (d.DayOfWeek == DayOfWeek.Friday)
                fridayNumber++;
        }

        return fridayNumber;
    }


    private static async Task<bool> HasHolidayAsync(
    IApplicationDbContext context,
    DateOnly date,
    CancellationToken cancellationToken)
    {
        return await context.Holidays
            .AnyAsync(
                x =>
                    x.StartDate <= date &&
                    x.EndDate >= date,
                cancellationToken);
    }

    private static async Task<bool> HasNormalLeaveAsync(
    IApplicationDbContext context,
    int employeeId,
    DateOnly date,
    CancellationToken cancellationToken)
    {
        return await context.EmployeeLeaves
            .AnyAsync(
                x =>
                    x.EmployeeId == employeeId &&
                    x.Status == LeaveStatus.Approved &&
                    x.StartDate <= date &&
                    x.EndDate >= date,
                cancellationToken);
    }

    private static async Task<bool> HasSpecialLeaveAsync(
    IApplicationDbContext context,
    int employeeId,
    DateOnly date,
    CancellationToken cancellationToken)
    {
        return await context.EmployeeSpecialLeaves
            .AnyAsync(
                x =>
                    x.EmployeeId == employeeId &&
                    x.Status == SpecialLeaveStatus.Approved &&
                    x.StartDate <= date &&
                    x.EndDate >= date,
                cancellationToken);
    }

    private static async Task<bool> HasAttendanceLogsAsync(
    IApplicationDbContext context,
    int attendanceDeviceId,
    string deviceEmployeeId,
    DateOnly date,
    CancellationToken cancellationToken)
    {
        var targetDate =
            date.ToDateTime(TimeOnly.MinValue).Date;


        return await context.AttendanceLogs
            .AnyAsync(
                x =>
                    x.AttendanceDeviceId == attendanceDeviceId &&
                    x.DeviceEmployeeId == deviceEmployeeId &&
                    x.LogTime.Date == targetDate,
                cancellationToken);
    }
}