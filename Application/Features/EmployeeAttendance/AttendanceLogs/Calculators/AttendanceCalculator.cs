using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using MicroERP.Application.Features.EmployeeAttendance.AttendanceLogs.Interfaces;
using MicroERP.Domin.Entities.EmployeeAttendance;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.EmployeeAttendance.AttendanceLogs.Calculators;

public class AttendanceCalculator : IAttendanceCalculator
{
    private readonly IApplicationDbContext _context;
    private readonly IAttendanceOvertimeService _overtimeService;
    private readonly IAttendanceAbsentService _absentService;
    public AttendanceCalculator(
        IApplicationDbContext context, IAttendanceOvertimeService overtimeService, IAttendanceAbsentService absentService)
    {
        _context = context;
        _overtimeService = overtimeService;
        _absentService = absentService;
    }



    public async Task CalculateAsync(
        AttendanceRecord attendance,
        CancellationToken cancellationToken = default)
    {

        var isOnLeave =
            await _context.EmployeeLeaves
            .AnyAsync(
                x =>
                x.EmployeeId == attendance.EmployeeId &&
                x.Status == LeaveStatus.Approved &&
                x.StartDate <= attendance.Date &&
                x.EndDate >= attendance.Date,
                cancellationToken);

        var isOnSpecialLeave =
            await _context.EmployeeSpecialLeaves
            .AnyAsync(
                x =>
                x.EmployeeId == attendance.EmployeeId &&
                x.StartDate <= attendance.Date &&
                x.EndDate >= attendance.Date,
                cancellationToken);



        var isHoliday =
            await _context.Holidays
            .AnyAsync(
                x =>
                x.StartDate <= attendance.Date &&
                x.EndDate >= attendance.Date,
                cancellationToken);

     

        var employee =
            await _context.Employees
            .Include(x => x.WorkSchedule)
            .FirstOrDefaultAsync(
                x =>
                x.Id == attendance.EmployeeId,
                cancellationToken);



        if (employee == null ||
            employee.WorkSchedule == null)
        {
            return;
        }



        var schedule =
            employee.WorkSchedule;



        var transactions =
            await _context.AttendanceTransactions
            .Where(x =>
                x.AttendanceRecordId == attendance.Id)
            .OrderBy(x =>
                x.TransactionTime)
            .ToListAsync(
                cancellationToken);



        ResetAttendanceValues(
            attendance);



        var firstShiftResult =
    CalculateShift(
        transactions,
        ShiftNumber.First,
        schedule.FirstShiftStart,
        schedule.FirstShiftEnd,
        schedule.SecondShiftStart,
        schedule.LateGraceMinutes,
        schedule.EarlyLeaveGraceMinutes,
        attendance.Date);



        ShiftCalculationResult? secondShiftResult = null;

        if (schedule.SecondShiftStart.HasValue &&
            schedule.SecondShiftEnd.HasValue)
        {
            secondShiftResult =
                CalculateShift(
                    transactions,
                    ShiftNumber.Second,
                    schedule.SecondShiftStart.Value,
                    schedule.SecondShiftEnd.Value,
                    null,
                    schedule.LateGraceMinutes,
                    schedule.EarlyLeaveGraceMinutes,
                    attendance.Date);
        }



        attendance.WorkedMinutes =
    firstShiftResult.WorkedMinutes +
    (secondShiftResult?.WorkedMinutes ?? 0);

        attendance.LateMinutes =
            firstShiftResult.LateMinutes +
            (secondShiftResult?.LateMinutes ?? 0);

        attendance.EarlyLeaveMinutes =
            firstShiftResult.EarlyLeaveMinutes +
            (secondShiftResult?.EarlyLeaveMinutes ?? 0);

        attendance.IsMissingCheckIn =
            firstShiftResult.IsMissingCheckIn ||
            (secondShiftResult?.IsMissingCheckIn ?? false);

        attendance.IsMissingCheckOut =
            firstShiftResult.IsMissingCheckOut ||
            (secondShiftResult?.IsMissingCheckOut ?? false);



        // خروج بدون دخول
        if (attendance.IsMissingCheckIn)
        {
            attendance.Status =
                AttendanceStatus.Absent;

            return;
        }




        var hasFirstShift =
            transactions.Any(x =>
                x.ShiftNumber ==
                ShiftNumber.First);



        var hasSecondShift =
            transactions.Any(x =>
                x.ShiftNumber ==
                ShiftNumber.Second);



        // حساب ExpectedMinutes
        CalculateExpectedMinutes(
            schedule,
            hasFirstShift,
            hasSecondShift,
            attendance);



        // LostTime = الفرق الحقيقي
        attendance.LostTimeMinutes =
            Math.Max(
                attendance.ExpectedMinutes -
                attendance.WorkedMinutes,
                0);

        await _overtimeService.CreateAsync(
         attendance,
         firstShiftResult,
         secondShiftResult,
         cancellationToken);

        // التحقق من العطلة الرسمية
        if (isHoliday)
        {
            SetSpecialStatus(attendance, AttendanceStatus.Holiday);

        }


        // نهاية الأسبوع
        else if (attendance.Date.DayOfWeek == DayOfWeek.Friday)
        {
            SetSpecialStatus(attendance, AttendanceStatus.Weekend);
            await _overtimeService.CreateHolidayWeekendOvertimeAsync(attendance);

        }


        // التحقق من الإجازة
        else if (isOnLeave)
        {
            //      await _absentService.CreateAbsentRecordsAsync(attendance.Date,
            //cancellationToken);
            SetSpecialStatus(attendance, AttendanceStatus.OnLeave);
          //  return;
        }


        //  التحقق من الإجازة الخاصه
        else if (isOnSpecialLeave)
        {
            
            SetSpecialStatus(attendance, AttendanceStatus.OnLeave);
            await _overtimeService.CreateHolidayWeekendOvertimeAsync(attendance);

        }
        // حضر شفت ناقص
      else if (!hasFirstShift ||
              !hasSecondShift)
        {
            attendance.Status =
                AttendanceStatus.PartialAttendance;

        }
        else     // لا توجد بصمات
        if (!transactions.Any())
        {
            attendance.Status =
                AttendanceStatus.Absent;

            return;
        }

        else attendance.Status =
              AttendanceStatus.Present;
      
       

    }


    private static void CalculateExpectedMinutes(
    WorkSchedule schedule,
    bool hasFirstShift,
    bool hasSecondShift,
    AttendanceRecord attendance)
    {
        attendance.ExpectedMinutes = 0;



        // دوام كامل
        if (hasFirstShift &&
            hasSecondShift)
        {
            if (schedule.IsFirstShiftRequired)
            {
                attendance.ExpectedMinutes +=
                    (int)(
                    schedule.FirstShiftEnd.ToTimeSpan() -
                    schedule.FirstShiftStart.ToTimeSpan())
                    .TotalMinutes;
            }



            if (schedule.IsSecondShiftRequired &&
                schedule.SecondShiftStart.HasValue &&
                schedule.SecondShiftEnd.HasValue)
            {
                attendance.ExpectedMinutes +=
                    (int)(
                    schedule.SecondShiftEnd.Value.ToTimeSpan() -
                    schedule.SecondShiftStart.Value.ToTimeSpan())
                    .TotalMinutes;
            }


            return;
        }



        // شفت واحد فقط
        if (hasFirstShift)
        {
            attendance.ExpectedMinutes =
                (int)(
                schedule.FirstShiftEnd.ToTimeSpan() -
                schedule.FirstShiftStart.ToTimeSpan())
                .TotalMinutes;


            return;
        }



        if (hasSecondShift &&
            schedule.SecondShiftStart.HasValue &&
            schedule.SecondShiftEnd.HasValue)
        {
            attendance.ExpectedMinutes =
                (int)(
                schedule.SecondShiftEnd.Value.ToTimeSpan() -
                schedule.SecondShiftStart.Value.ToTimeSpan())
                .TotalMinutes;
        }
    }



    private static void ResetAttendanceValues(
        AttendanceRecord attendance)
    {
        attendance.WorkedMinutes = 0;

        attendance.ExpectedMinutes = 0;

        attendance.LateMinutes = 0;

        attendance.EarlyLeaveMinutes = 0;

        attendance.LostTimeMinutes = 0;


        attendance.IsMissingCheckIn = false;

        attendance.IsMissingCheckOut = false;
    }




    private static void SetSpecialStatus(
      AttendanceRecord attendance,
      AttendanceStatus status)
    {
        attendance.Status = status;


        // يوجد بصمات، نحافظ على بيانات الحضور

        attendance.LateMinutes = 0;

        attendance.EarlyLeaveMinutes = 0;

        attendance.LostTimeMinutes = 0;
    }




    private static ShiftCalculationResult CalculateShift(
     List<AttendanceTransaction> transactions,
     ShiftNumber shift,
     TimeOnly shiftStart,
     TimeOnly shiftEnd,
     TimeOnly? nextShiftStart,
     int lateGrace,
     int earlyLeaveGrace,
     DateOnly date)
    {
        var result = new ShiftCalculationResult
        {
            Shift = shift
        };


        // =========================================================
        // Scheduled Times
        // =========================================================

        var scheduledStart =
            date.ToDateTime(shiftStart);

        var scheduledEnd =
            date.ToDateTime(shiftEnd);


        var nextShiftDateTime =
            nextShiftStart.HasValue
                ? date.ToDateTime(nextShiftStart.Value)
                : (DateTime?)null;


        // =========================================================
        // Transactions Of This Shift
        // =========================================================

        var shiftTransactions =
            transactions
                .Where(x =>
                    x.ShiftNumber == shift)
                .Where(x =>
                    // الشفت الأول لا يأخذ أي حركة بعد بداية الشفت الثاني
                    !nextShiftDateTime.HasValue ||
                    x.TransactionTime < nextShiftDateTime.Value)
                .OrderBy(x =>
                    x.TransactionTime)
                .ToList();


        var checkIn =
            shiftTransactions
                .FirstOrDefault(x =>
                    x.Type ==
                    AttendanceTransactionType.CheckIn);


        var checkOuts =
            shiftTransactions
                .Where(x =>
                    x.Type ==
                    AttendanceTransactionType.CheckOut)
                .OrderBy(x =>
                    x.TransactionTime)
                .ToList();


        // أول خروج
        var firstCheckOut =
            checkOuts.FirstOrDefault();


        // آخر خروج
        var lastCheckOut =
            checkOuts.LastOrDefault();


        // =========================================================
        // لا توجد حركة لهذا الشفت
        // =========================================================

        if (checkIn == null &&
            lastCheckOut == null)
        {
            return result;
        }


        // =========================================================
        // خروج بدون دخول
        // =========================================================

        if (checkIn == null &&
            lastCheckOut != null)
        {
            result.IsMissingCheckIn = true;

            result.CheckOut =
                lastCheckOut.TransactionTime;

            return result;
        }


        result.CheckIn =
            checkIn!.TransactionTime;


        // =========================================================
        // دخول بدون خروج
        // =========================================================

        if (lastCheckOut == null)
        {
            result.IsMissingCheckOut = true;

            // نغلق الشفت عند نهاية الدوام الرسمية
            result.CheckOut =
                scheduledEnd;


            if (scheduledEnd >
                checkIn.TransactionTime)
            {
                result.WorkedMinutes =
                    (int)(
                        scheduledEnd -
                        checkIn.TransactionTime)
                    .TotalMinutes;
            }


            result.LateMinutes =
                CalculateLateMinutes(
                    checkIn.TransactionTime,
                    scheduledStart,
                    lateGrace);


            return result;
        }


        // =========================================================
        // آخر خروج هو الخروج المعتمد
        // =========================================================

        result.CheckOut =
            lastCheckOut.TransactionTime;


        if (lastCheckOut.TransactionTime <=
            checkIn.TransactionTime)
        {
            return result;
        }


        // =========================================================
        // التأخير
        // =========================================================

        var late =
            Math.Max(
                0,
                (int)(
                    checkIn.TransactionTime -
                    scheduledStart)
                .TotalMinutes);


        var deductedLate =
            Math.Max(
                0,
                late - lateGrace);


        result.LateMinutes =
            deductedLate;


        // =========================================================
        // Worked Minutes
        // =========================================================

        result.WorkedMinutes =
            (int)(
                lastCheckOut.TransactionTime -
                checkIn.TransactionTime)
            .TotalMinutes
            + Math.Min(
                late,
                lateGrace);


        // =========================================================
        // Early Leave
        // =========================================================

        var earlyLeave =
            (int)(
                scheduledEnd -
                lastCheckOut.TransactionTime)
            .TotalMinutes;


        if (earlyLeave > earlyLeaveGrace)
        {
            result.EarlyLeaveMinutes =
                earlyLeave -
                earlyLeaveGrace;
        }


        // =========================================================
        // Overtime
        // =========================================================
        //
        // أول OUT = بداية الإضافي
        // آخر OUT = نهاية الإضافي
        //
        // بشرط أن تكون البصمات كلها داخل حدود الشفت.
        //
        // =========================================================

        // =========================================================
        // Overtime
        // =========================================================
        //
        // الإضافي يبدأ بعد نهاية الشفت الرسمية.
        // لا يشترط وجود أكثر من CheckOut.
        // =========================================================

        if (lastCheckOut.TransactionTime > scheduledEnd)
        {
            result.OvertimeStart =
                scheduledEnd;

            result.OvertimeMinutes =
                (int)(
                    lastCheckOut.TransactionTime -
                    scheduledEnd)
                .TotalMinutes;
        }

        return result;
    }




    private static int CalculateLateMinutes(
        DateTime actualCheckIn,
        DateTime scheduledStart,
        int graceMinutes)
    {
        var late =
            (int)(
                actualCheckIn -
                scheduledStart)
            .TotalMinutes;

        if (late <= graceMinutes)
            return 0;

        return late - graceMinutes;
    }
}