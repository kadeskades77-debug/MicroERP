using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using Microsoft.EntityFrameworkCore;
using MicroERP.Application.Features.EmployeeAttendance.Holidays.DTOs;
using MicroERP.Application.Features.EmployeeAttendance.Holidays.Interfaces;
using MicroERP.Domin.Entities.EmployeeAttendance;

namespace MicroERP.Application.Features.EmployeeAttendance.Holidays.Services;

public class HolidayService : IHolidayService
{
    private readonly IApplicationDbContext _context;

    public HolidayService(
        IApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<Result<HolidayDto>> CreateAsync(CreateHolidayDto dto,
        CancellationToken cancellationToken = default)
    {
        var exists =
            await _context.Holidays
            .AnyAsync(
                x => x.Name == dto.Name &&
                     x.StartDate == dto.StartDate &&
                     x.EndDate == dto.EndDate,
                cancellationToken);


        if (exists)
        {
            return Result<HolidayDto>
                .Failure("Holiday already exists.");
        }



        var holiday = new Holiday
        {
            Name = dto.Name,

            StartDate = dto.StartDate,

            EndDate = dto.EndDate,

            Type = dto.Type,

            IsRecurring = dto.IsRecurring,

            Notes = dto.Notes
        };



        _context.Holidays.Add(holiday);



        await _context.SaveChangesAsync(
            cancellationToken);



        return Result<HolidayDto>
            .Succeeded(
                Map(holiday));
    }



    public async Task<Result<HolidayDto>> UpdateAsync(int id,UpdateHolidayDto dto,
        CancellationToken cancellationToken = default)
    {
        var holiday =
            await _context.Holidays
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);



        if (holiday == null)
        {
            return Result<HolidayDto>
                .Failure("Holiday not found.");
        }



        if (dto.Name is not null)
            holiday.Name = dto.Name;



        if (dto.StartDate.HasValue)
            holiday.StartDate = dto.StartDate.Value;



        if (dto.EndDate.HasValue)
            holiday.EndDate = dto.EndDate.Value;



        if (dto.Type.HasValue)
            holiday.Type = dto.Type.Value;



        if (dto.IsRecurring.HasValue)
            holiday.IsRecurring = dto.IsRecurring.Value;



        if (dto.Notes is not null)
            holiday.Notes = dto.Notes;



        if (holiday.EndDate < holiday.StartDate)
        {
            return Result<HolidayDto>
                .Failure(
                "End date must be greater than or equal to start date.");
        }



        await _context.SaveChangesAsync(
            cancellationToken);



        return Result<HolidayDto>
            .Succeeded(
                Map(holiday));
    }



    public async Task<Result<bool>> DeleteAsync(int id,
        CancellationToken cancellationToken = default)
    {
        var holiday =
            await _context.Holidays
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);



        if (holiday == null)
        {
            return Result<bool>
                .Failure("Holiday not found.");
        }



        // Soft Delete حسب BaseEntity
        holiday.IsDeleted = true;

        holiday.IsActive = false;



        await _context.SaveChangesAsync(
            cancellationToken);



        return Result<bool>
            .Succeeded(true);
    }



    private static HolidayDto Map(
        Holiday holiday)
    {
        return new HolidayDto
        {
            Id = holiday.Id,

            Name = holiday.Name,

            StartDate = holiday.StartDate,

            EndDate = holiday.EndDate,

            Type = holiday.Type,

            IsRecurring = holiday.IsRecurring,

            Notes = holiday.Notes
        };
    }
}