using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.AttendanceTransactions.DTOs;
using MicroERP.Application.Features.EmployeeAttendance.AttendanceTransactions.Interfaces;
using MicroERP.Domin.Entities.EmployeeAttendance;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.EmployeeAttendance.AttendanceTransactions.Services;

public class AttendanceTransactionService
    : IAttendanceTransactionService
{
    private readonly IApplicationDbContext _context;


    public AttendanceTransactionService(
        IApplicationDbContext context)
    {
        _context = context;
    }



    public async Task<Result<AttendanceTransactionDto>> CreateAsync(
        CreateAttendanceTransactionDto dto,
        CancellationToken cancellationToken = default)
    {
        var employee = await _context.Employees
            .Include(x => x.WorkSchedule)
            .FirstOrDefaultAsync(
                x => x.Id == dto.EmployeeId,
                cancellationToken);


        if (employee == null)
        {
            return Result<AttendanceTransactionDto>
                .Failure("Employee not found");
        }



        var date =
            DateOnly.FromDateTime(DateTime.Now);



        var attendance =
            await _context.AttendanceRecords
            .FirstOrDefaultAsync(
                x => x.EmployeeId == dto.EmployeeId
                && x.Date == date,
                cancellationToken);



        if (attendance == null)
        {
            attendance = new AttendanceRecord
            {
                EmployeeId = dto.EmployeeId,

                Date = date,

                Status = AttendanceStatus.Present
            };


            _context.AttendanceRecords.Add(attendance);


            await _context.SaveChangesAsync(
                cancellationToken);
        }



        var shift =
            DetermineShift(
                employee.WorkSchedule,
                dto.TransactionTime);



        var transaction = new AttendanceTransaction
        {
            AttendanceRecordId = attendance.Id,

            TransactionTime =
                date.ToDateTime(dto.TransactionTime),

            Type = dto.Type,

            ShiftNumber = shift,

            Notes = dto.Notes
        };



        _context.AttendanceTransactions
            .Add(transaction);



        await _context.SaveChangesAsync(
            cancellationToken);



        return Result<AttendanceTransactionDto>
            .Succeeded(
                new AttendanceTransactionDto
                {
                    Id = transaction.Id,

                    EmployeeId =
                        employee.Id,

                    EmployeeName =
                        employee.User != null
                        ? employee.User.FullName
                        : "",

                    Date = date,

                    TransactionTime =
                        dto.TransactionTime,

                    Type =
                        transaction.Type,

                    ShiftNumber =
                        transaction.ShiftNumber,

                    Notes =
                        transaction.Notes,

                    CreatedOn =
                        transaction.CreatedOn
                });
    }





    public async Task<Result<List<AttendanceTransactionDto>>> GetByAttendanceAsync(
        int attendanceRecordId,
        CancellationToken cancellationToken = default)
    {
        var result = await _context.AttendanceTransactions
            .Where(x =>
                x.AttendanceRecordId == attendanceRecordId)
            .Select(x => new AttendanceTransactionDto
            {
                Id = x.Id,

                EmployeeId =
                    x.AttendanceRecord.EmployeeId,

                EmployeeName =
                    x.AttendanceRecord.Employee.User.FullName,

                Date =
                    x.AttendanceRecord.Date,

                TransactionTime =
                    TimeOnly.FromDateTime(
                        x.TransactionTime),

                Type =
                    x.Type,

                ShiftNumber =
                    x.ShiftNumber,

                Notes =
                    x.Notes,

                CreatedOn =
                    x.CreatedOn
            })
            .OrderBy(x => x.TransactionTime)
            .ToListAsync(cancellationToken);



        return Result<List<AttendanceTransactionDto>>
            .Succeeded(result);
    }





    private static ShiftNumber DetermineShift(
        WorkSchedule? schedule,
        TimeOnly time)
    {
        if (schedule == null)
            return ShiftNumber.First;



        if (time >= schedule.FirstShiftStart
            &&
            time <= schedule.FirstShiftEnd)
        {
            return ShiftNumber.First;
        }



        if (schedule.SecondShiftStart.HasValue
            &&
            schedule.SecondShiftEnd.HasValue
            &&
            time >= schedule.SecondShiftStart.Value
            &&
            time <= schedule.SecondShiftEnd.Value)
        {
            return ShiftNumber.Second;
        }



        // خارج الفترات
        // نرجع الأقرب
        if (schedule.SecondShiftStart.HasValue
            &&
            time > schedule.FirstShiftEnd)
        {
            return ShiftNumber.Second;
        }


        return ShiftNumber.First;
    }
}