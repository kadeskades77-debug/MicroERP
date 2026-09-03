using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.Holidays.DTOs;
using MicroERP.Application.Features.EmployeeAttendance.Holidays.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Persistence.Queries;

public class HolidayQueries : IHolidayQueries
{
    private readonly IApplicationDbContext _context;

    public HolidayQueries(
        IApplicationDbContext context)
    {
        _context = context;
    }



    public async Task<Result<HolidayDto>> GetByIdAsync(int id,
        CancellationToken cancellationToken = default)
    {
        var holiday =
            await _context.Holidays
            .Where(x => x.Id == id)
            .Select(x => new HolidayDto
            {
                Id = x.Id,

                Name = x.Name,

                StartDate = x.StartDate,

                EndDate = x.EndDate,

                Type = x.Type,

                IsRecurring = x.IsRecurring,

                Notes = x.Notes
            })
            .FirstOrDefaultAsync(
                cancellationToken);



        if (holiday == null)
        {
            return Result<HolidayDto>
                .Failure("Holiday not found.");
        }



        return Result<HolidayDto>
            .Succeeded(holiday);
    }




    public async Task<Result<PagedResult<HolidayDto>>> GetAllAsync(
        HolidayFilterDto filter,
        PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var query =
            _context.Holidays
            .AsQueryable();



        if (filter.Type.HasValue)
        {
            query =
                query.Where(x =>
                    x.Type == filter.Type.Value);
        }



        if (filter.Year.HasValue)
        {
            query =
                query.Where(x =>
                    x.StartDate.Year <= filter.Year.Value &&
                    x.EndDate.Year >= filter.Year.Value);
        }



        if (filter.IsRecurring.HasValue)
        {
            query =
                query.Where(x =>
                    x.IsRecurring == filter.IsRecurring.Value);
        }



        var totalCount =
            await query.CountAsync(
                cancellationToken);



        var items =
            await query
            .OrderBy(x => x.StartDate)
            .Skip(
                (request.PageNumber - 1) *
                request.PageSize)
            .Take(request.PageSize)
            .Select(x => new HolidayDto
            {
                Id = x.Id,

                Name = x.Name,

                StartDate = x.StartDate,

                EndDate = x.EndDate,

                Type = x.Type,

                IsRecurring = x.IsRecurring,

                Notes = x.Notes
            })
            .ToListAsync(
                cancellationToken);



        return Result<PagedResult<HolidayDto>>
            .Succeeded(
                new PagedResult<HolidayDto>
                {
                    Items = items,

                    TotalCount = totalCount,

                    PageNumber = request.PageNumber,

                    PageSize = request.PageSize
                });
    }




    public async Task<Result<List<HolidayDto>>> GetByDateAsync(DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var holidays =
            await _context.Holidays
            .Where(x =>
                x.StartDate <= date &&
                x.EndDate >= date)
            .Select(x => new HolidayDto
            {
                Id = x.Id,

                Name = x.Name,

                StartDate = x.StartDate,

                EndDate = x.EndDate,

                Type = x.Type,

                IsRecurring = x.IsRecurring,

                Notes = x.Notes
            })
            .ToListAsync(
                cancellationToken);



        return Result<List<HolidayDto>>
            .Succeeded(holidays);
    }
}