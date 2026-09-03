using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Features.EmployeeAttendance.AttendanceLogs.Calculators;
using MicroERP.Application.Features.EmployeeAttendance.AttendanceLogs.Helpers;
using MicroERP.Application.Features.EmployeeAttendance.AttendanceLogs.Interfaces;
using MicroERP.Domin.Entities.EmployeeAttendance;
using MicroERP.Domin.Entities.Employees;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.EmployeeAttendance.AttendanceLogs.Services;

public class AttendanceLogProcessor : IAttendanceLogProcessor
{
    private readonly IApplicationDbContext _context;
    private readonly IShiftResolver _shiftResolver;
    private readonly IAttendanceCalculator _calculator;
    private readonly IAttendanceOvertimeService _overtimeService;
    public AttendanceLogProcessor(
        IApplicationDbContext context,
        IShiftResolver shiftResolver,
        IAttendanceCalculator calculator
,
        IAttendanceOvertimeService overtimeService)
    {
        _context = context;
        _shiftResolver = shiftResolver;
        _calculator = calculator;
        _overtimeService = overtimeService;
    }

    public async Task ProcessPendingLogsAsync(
     CancellationToken cancellationToken = default)
    {
        // 1) جلب سجلات البصمة غير المعالجة
        var logs = await GetPendingLogsAsync(
            cancellationToken);

        if (logs.Count == 0)
            return;


        // 2) بناء Cache الموظفين
        var employeeCache =
            await GetEmployeeDevicesCacheAsync(
                logs,
                cancellationToken);


        // 3) تحميل AttendanceRecord الموجودة
        var attendanceCache =
            await GetAttendanceRecordsCacheAsync(
                logs,
                employeeCache,
                cancellationToken);


        // 4) السجلات التي تغيرت لإعادة الحساب
        var affectedAttendances =
            new Dictionary<
                (int EmployeeId, DateOnly Date),
                AttendanceRecord>();


        // 5) معالجة البصمات بالترتيب الزمني
        foreach (var log in logs)
        {
            var employee =
                ResolveEmployee(
                    log,
                    employeeCache);

            if (employee == null)
                continue;


            var date =
                DateOnly.FromDateTime(
                    log.LogTime);


            var attendance =
                GetOrCreateAttendanceRecord(
                    employee,
                    date,
                    attendanceCache);


            var transaction =
                CreateAttendanceTransaction(
                    log,
                    attendance,
                    employee);


            attendance.Transactions.Add(
                transaction);


            MarkLogAsProcessed(
                log,
                employee.Id);


            affectedAttendances.TryAdd(
                (
                    attendance.EmployeeId,
                    attendance.Date
                ),
                attendance);
        }


        // 6) حفظ الحركات
        await _context.SaveChangesAsync(
            cancellationToken);


        // 7) إعادة حساب الحضور
        await CalculateAttendancesAsync(
            affectedAttendances.Values,
            cancellationToken);


        // 8) حفظ نتائج الحساب
        await _context.SaveChangesAsync(
            cancellationToken);
    }

    //============================HELPERS==================

    private async Task<List<AttendanceLog>> GetPendingLogsAsync(
    CancellationToken cancellationToken)
    {

        //    وظيفتها:
        //        تجلب فقط السجلات الجديدة من جهاز البصمة.
        //لا تعالج السجلات التي تم تحويلها سابقًا.
        //ترتبها حسب وقت البصمة لضمان التسلسل الصحيح.
        return await _context.AttendanceLogs
            .Where(x => !x.IsProcessed)
            .OrderBy(x => x.LogTime)
            .ToListAsync(cancellationToken);
    }

    private async Task<EmployeeCache>
       GetEmployeeDevicesCacheAsync(
           List<AttendanceLog> logs,
           CancellationToken cancellationToken)
    {
        var deviceIds = logs
            .Select(x => x.AttendanceDeviceId)
            .Distinct()
            .ToList();


        var employeeCodes = logs
            .Select(x => x.DeviceEmployeeId)
            .Distinct()
            .ToList();



        var employeeDevices = await _context.EmployeeAttendanceDevices
            .Include(x => x.Employee)
                .ThenInclude(x => x.WorkSchedule)
            .Where(x =>
                deviceIds.Contains(x.AttendanceDeviceId) &&
                employeeCodes.Contains(x.DeviceEmployeeId))
            .ToListAsync(cancellationToken);



        var cache = new EmployeeCache();



        foreach (var item in employeeDevices)
        {
            cache.ByDeviceCode.Add(
                (
                    item.AttendanceDeviceId,
                    item.DeviceEmployeeId
                ),
                item.Employee);



            if (!cache.ById.ContainsKey(item.EmployeeId))
            {
                cache.ById.Add(
                    item.EmployeeId,
                    item.Employee);
            }
        }



        return cache;
    }

    private async Task<Dictionary<(int EmployeeId, DateOnly Date), AttendanceRecord>>
     GetAttendanceRecordsCacheAsync(
         List<AttendanceLog> logs,
         EmployeeCache employeeCache,
         CancellationToken cancellationToken)
    {
        var employeeIds = logs
            .Select(log =>
                employeeCache.ByDeviceCode.TryGetValue(
                    (
                        log.AttendanceDeviceId,
                        log.DeviceEmployeeId
                    ),
                    out var employee)
                    ? employee.Id
                    : 0)
            .Where(id => id > 0)
            .Distinct()
            .ToList();



        if (employeeIds.Count == 0)
            return new();



        var dates = logs
            .Select(x => DateOnly.FromDateTime(x.LogTime))
            .Distinct()
            .ToList();



        var records = await _context.AttendanceRecords
            .Where(x =>
                employeeIds.Contains(x.EmployeeId) &&
                dates.Contains(x.Date))
            .Include(x => x.Transactions)
            .ToListAsync(cancellationToken);



        return records.ToDictionary(
            x => (
                x.EmployeeId,
                x.Date
            ));
    }

    private Employee? ResolveEmployee(
    AttendanceLog log,
    EmployeeCache employeeCache)
    {
        // إذا كان الربط موجوداً مسبقاً في AttendanceLog
        if (log.EmployeeId.HasValue &&
            employeeCache.ById.TryGetValue(
                log.EmployeeId.Value,
                out var employee))
        {
            return employee;
        }



        // البحث عن طريق رقم الموظف في جهاز البصمة
        employeeCache.ByDeviceCode.TryGetValue(
            (
                log.AttendanceDeviceId,
                log.DeviceEmployeeId
            ),
            out employee);



        return employee;
    }

    private AttendanceRecord GetOrCreateAttendanceRecord(
    Employee employee,
    DateOnly date,
    Dictionary<(int EmployeeId, DateOnly Date), AttendanceRecord> attendanceCache)
    {
        var key = (
            employee.Id,
            date
        );


        if (attendanceCache.TryGetValue(
            key,
            out var attendance))
        {
            return attendance;
        }



        attendance = new AttendanceRecord
        {
            EmployeeId = employee.Id,

            Date = date,

            Status = AttendanceStatus.Present
        };



        _context.AttendanceRecords.Add(attendance);



        attendanceCache.TryAdd(
            key,
            attendance);



        return attendance;
    }


    private AttendanceTransaction CreateAttendanceTransaction(
      AttendanceLog log,
      AttendanceRecord attendance,
      Employee employee)
    {
        if (employee.WorkSchedule == null)
            throw new InvalidOperationException(
                "Employee does not have work schedule.");

        var transactionType =
            MapTransactionType(log.Type);

        ShiftNumber shift;

        // =========================================================
        // Check In
        // =========================================================

        if (transactionType == AttendanceTransactionType.CheckIn)
        {
            shift = _shiftResolver.Resolve(
                employee.WorkSchedule,
                TimeOnly.FromDateTime(log.LogTime),
                transactionType);
        }

        // =========================================================
        // Check Out
        // =========================================================

        else
        {
            shift = FindLastShift(
                attendance.Transactions);
        }

        return new AttendanceTransaction
        {
            AttendanceRecord = attendance,

            AttendanceDeviceId = log.AttendanceDeviceId,

            AttendanceLogId = log.Id,

            TransactionTime = log.LogTime,

            Type = transactionType,

            ShiftNumber = shift
        };
    }

    private void MarkLogAsProcessed(
        AttendanceLog log,
        int employeeId)
    {
        log.EmployeeId = employeeId;

        log.IsProcessed = true;
    }

    private async Task CalculateAttendancesAsync(
    IEnumerable<AttendanceRecord> attendanceRecords,
    CancellationToken cancellationToken)
    {
        foreach (var attendance in attendanceRecords)
        {
            await _calculator.CalculateAsync(
                attendance,
                cancellationToken);
            await _overtimeService.CreateHolidayWeekendOvertimeAsync(
               attendance,
               cancellationToken);
        }
    }

    private AttendanceTransactionType MapTransactionType(
    AttendanceLogType type)
    {
        return type switch
        {
            AttendanceLogType.CheckIn =>
                AttendanceTransactionType.CheckIn,


            AttendanceLogType.CheckOut =>
                AttendanceTransactionType.CheckOut,


            _ => throw new ArgumentOutOfRangeException(
                nameof(type),
                type,
                "Unsupported attendance log type.")
        };
    }


    private static ShiftNumber FindLastShift(
     ICollection<AttendanceTransaction> transactions)
    {
        return transactions
            .Where(x =>
                x.Type == AttendanceTransactionType.CheckIn &&
                x.ShiftNumber != ShiftNumber.None)
            .OrderByDescending(x => x.TransactionTime)
            .Select(x => x.ShiftNumber)
            .FirstOrDefault();
    }
}