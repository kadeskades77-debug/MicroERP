using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Overtime;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Persistence.Queries;

public class EmployeeOvertimeQueries
    : IEmployeeOvertimeQueries
{
    private readonly IApplicationDbContext _context;


    public EmployeeOvertimeQueries(
        IApplicationDbContext context)
    {
        _context = context;
    }



    public async Task<Result<EmployeeOvertimeDto>> GetByIdAsync(int id,
        CancellationToken cancellationToken = default)
    {
        var result =
            await _context.EmployeeOvertimes
            .AsNoTracking()
            .Where(x =>
                x.Id == id )
            .Select(x => new EmployeeOvertimeDto
            {
                Id = x.Id,

                EmployeeId =
                    x.EmployeeId,

                EmployeeName =
                    x.Employee.User.FullName,

                StartDateTime =
                    x.StartDateTime,

                EndDateTime =
                    x.EndDateTime,

                TotalMinutes =
                    x.TotalMinutes,

                HourlyRate =
                    x.HourlyRate,

                Multiplier =
                    x.Multiplier,

                Amount =
                    x.Amount,

                Type =
                    x.Type,

                Status =
                    x.Status,

                Source=x.Source,

                Reason =
                    x.Reason,

                IsPaid =
                    x.IsPaid
            })
            .FirstOrDefaultAsync(cancellationToken);



        if (result == null)
        {
            return Result<EmployeeOvertimeDto>
                .Failure(
                "Overtime request not found.");
        }



        return Result<EmployeeOvertimeDto>
            .Succeeded(result);
    }


    public async Task<Result<List<EmployeeOvertimeDto>>> GetAllAsync(
    CancellationToken cancellationToken = default)
    {
        var result =
            await Query()
            .OrderByDescending(x => x.StartDateTime)
            .ToListAsync(cancellationToken);


        return Result<List<EmployeeOvertimeDto>>
            .Succeeded(result);
    }



    public async Task<Result<List<EmployeeOvertimeDto>>> GetEmployeeHistoryAsync(int employeeId,
        CancellationToken cancellationToken = default)
    {
        var result =
            await Query()
            .Where(x =>
                x.EmployeeId == employeeId)
            .OrderByDescending(x => x.StartDateTime)
            .ToListAsync(cancellationToken);



        return Result<List<EmployeeOvertimeDto>>
            .Succeeded(result);
    }



    public async Task<Result<List<EmployeeOvertimeDto>>> GetPendingAsync(
        CancellationToken cancellationToken = default)
    {
        var result =
            await Query()
            .Where(x =>
                x.Status == OvertimeStatus.Pending)
            .OrderBy(x => x.StartDateTime)
            .ToListAsync(cancellationToken);



        return Result<List<EmployeeOvertimeDto>>
            .Succeeded(result);
    }



    public async Task<Result<List<EmployeeOvertimeDto>>> GetUnpaidAsync(
        CancellationToken cancellationToken = default)
    {
        var result =
            await Query()
            .Where(x =>
                x.Status == OvertimeStatus.Approved &&
                !x.IsPaid)
            .OrderBy(x => x.StartDateTime)
            .ToListAsync(cancellationToken);



        return Result<List<EmployeeOvertimeDto>>
            .Succeeded(result);
    }


    private IQueryable<EmployeeOvertimeDto> Query()
    {
        return _context.EmployeeOvertimes
            .AsNoTracking()
            .Select(x => new EmployeeOvertimeDto
            {
                Id = x.Id,

                EmployeeId =
                    x.EmployeeId,

                EmployeeName =
                    x.Employee.User.FullName,

                StartDateTime =
                    x.StartDateTime,

                EndDateTime =
                    x.EndDateTime,

                TotalMinutes =
                    x.TotalMinutes,

                HourlyRate =
                    x.HourlyRate,

                Multiplier =
                    x.Multiplier,

                Amount =
                    x.Amount,

                Type =
                    x.Type,

                Status =
                    x.Status,
                Source = x.Source,

                Reason =
                    x.Reason,

                IsPaid =
                    x.IsPaid
            });
    }

}