using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using MicroERP.Domin.Entities.EmployeeAttendance;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Services;

public class AttendanceAbsentService: IAttendanceAbsentService
{
    private readonly IApplicationDbContext _context;


    public AttendanceAbsentService(
        IApplicationDbContext context)
    {
        _context = context;
    }



    public async Task CreateAbsentRecordsAsync(
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var employees =
            await _context.Employees
            .Where(x =>
                x.IsActive &&
                x.WorkScheduleId != null)
            .Select(x => new
            {
                x.Id
            })
            .ToListAsync(cancellationToken);



        if (!employees.Any())
            return;



        var employeeIds =
            employees
            .Select(x => x.Id)
            .ToList();



        // الموظفون الذين لديهم سجل حضور
        var existingRecords =
            await _context.AttendanceRecords
            .Where(x =>
                x.Date == date &&
                employeeIds.Contains(x.EmployeeId))
            .Select(x => x.EmployeeId)
            .ToListAsync(cancellationToken);



        // ==========================
        // Holiday
        // ==========================

        var isHoliday =
            await _context.Holidays
            .AnyAsync(
                x =>
                x.StartDate <= date &&
                x.EndDate >= date,
                cancellationToken);



        if (isHoliday)
        {
            await CreateSpecialRecordsAsync(
                employeeIds
                    .Except(existingRecords)
                    .ToList(),
                date,
                AttendanceStatus.Holiday,
                cancellationToken);

            return;
        }


        // ==========================
        // Weekend
        // ==========================

        if (date.DayOfWeek ==
            DayOfWeek.Friday)
        {
            await CreateSpecialRecordsAsync(
                employeeIds,
                date,
                AttendanceStatus.Weekend,
                cancellationToken);

            return;
        }

        // ==========================
        // Leave
        // ==========================

        var employeesOnLeave =
            await _context.EmployeeLeaves
            .Where(x =>
                x.Status == LeaveStatus.Approved &&
                x.StartDate <= date &&
                x.EndDate >= date &&
                employeeIds.Contains(x.EmployeeId))
            .Select(x => x.EmployeeId)
            .ToListAsync(cancellationToken);



        // ==========================
        // Special Leave
        // ==========================

        var employeesOnSpecialLeave =
            await _context.EmployeeSpecialLeaves
            .Where(x =>
                x.Status == SpecialLeaveStatus.Approved &&
                x.StartDate <= date &&
                x.EndDate >= date &&
                employeeIds.Contains(x.EmployeeId))
            .Select(x => x.EmployeeId)
            .ToListAsync(cancellationToken);



        // دمج الإجازات العادية والخاصة
        var employeesInLeave =
            employeesOnLeave
                .Union(employeesOnSpecialLeave)
                .Distinct()
                .ToList();



        if (employeesInLeave.Any())
        {
            await CreateSpecialRecordsAsync(
                employeesInLeave
                    .Except(existingRecords)
                    .ToList(),
                date,
                AttendanceStatus.OnLeave,
                cancellationToken);
        }



        // إزالة موظفي الإجازة
        var remainingEmployees =
            employeeIds
                .Except(existingRecords)
                .Except(employeesInLeave)
                .ToList();



        if (!remainingEmployees.Any())
            return;



      



        // ==========================
        // Absent
        // ==========================

        await CreateSpecialRecordsAsync(
            remainingEmployees,
            date,
            AttendanceStatus.Absent,
            cancellationToken);
    }





    private async Task CreateSpecialRecordsAsync(
        List<int> employeeIds,
        DateOnly date,
        AttendanceStatus status,
        CancellationToken cancellationToken)
    {
        if (!employeeIds.Any())
            return;



        var records =
            employeeIds
            .Select(employeeId =>
                new AttendanceRecord
                {
                    EmployeeId = employeeId,

                    Date = date,

                    ExpectedMinutes = 0,

                    LostTimeMinutes = 0,

                    LateMinutes = 0,

                    EarlyLeaveMinutes = 0,

                    Status = status
                })
            .ToList();



        await _context.AttendanceRecords
            .AddRangeAsync(
                records,
                cancellationToken);



        await _context.SaveChangesAsync(
            cancellationToken);
    }
}