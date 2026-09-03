using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using MicroERP.Domin.Entities.EmployeeAttendance;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Services;

public class AttendanceService : IAttendanceService
{
    private readonly IApplicationDbContext _context;


    public AttendanceService(
        IApplicationDbContext context)
    {
        _context = context;
    }



    public async Task<Result<AttendanceDto>> CreateAsync(
        CreateAttendanceDto dto,
        CancellationToken cancellationToken = default)
    {
        var employeeExists = await _context.Employees
            .AnyAsync(
                x => x.Id == dto.EmployeeId,
                cancellationToken);


        if (!employeeExists)
        {
            return Result<AttendanceDto>
                .Failure("Employee not found");
        }



        var exists = await _context.AttendanceRecords
            .AnyAsync(
                x => x.EmployeeId == dto.EmployeeId
                && x.Date == dto.Date,
                cancellationToken);



        if (exists)
        {
            return Result<AttendanceDto>
                .Failure("Attendance already exists");
        }



        var attendance = new AttendanceRecord
        {
            EmployeeId = dto.EmployeeId,

            Date = dto.Date,

            Status = AttendanceStatus.Present,

            Notes = dto.Notes
        };



        _context.AttendanceRecords.Add(attendance);


        await _context.SaveChangesAsync(
            cancellationToken);



        return Result<AttendanceDto>
            .Succeeded(
                MapToDto(attendance));
    }





    public async Task<Result<AttendanceDto>> UpdateAsync(
        int id,
        UpdateAttendanceDto dto,
        CancellationToken cancellationToken = default)
    {
        var attendance =
            await _context.AttendanceRecords
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);



        if (attendance == null)
        {
            return Result<AttendanceDto>
                .Failure("Attendance not found");
        }



        if (!string.IsNullOrWhiteSpace(dto.Notes))
        {
            attendance.Notes = dto.Notes;
        }



        if (dto.Status.HasValue)
        {
            attendance.Status = dto.Status.Value;
        }



        await _context.SaveChangesAsync(
            cancellationToken);



        return Result<AttendanceDto>
            .Succeeded(
                MapToDto(attendance));
    }





    public async Task<Result<bool>> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var attendance =
            await _context.AttendanceRecords
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);



        if (attendance == null)
        {
            return Result<bool>
                .Failure("Attendance not found");
        }



        attendance.IsDeleted = true;



        await _context.SaveChangesAsync(
            cancellationToken);



        return Result<bool>
            .Succeeded(true);
    }




    private static AttendanceDto MapToDto(
        AttendanceRecord attendance)
    {
        return new AttendanceDto
        {
            Id = attendance.Id,

            EmployeeId = attendance.EmployeeId,

            Date = attendance.Date,

            WorkedMinutes =
                attendance.WorkedMinutes,

            LateMinutes =
                attendance.LateMinutes,

            LostTimeMinutes =
                attendance.LostTimeMinutes,

            EarlyLeaveMinutes =
                attendance.EarlyLeaveMinutes,

            Status = attendance.Status,

            Notes = attendance.Notes,

            CreatedOn = attendance.CreatedOn
        };
    }
}