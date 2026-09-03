using AutoMapper;
using AutoMapper.QueryableExtensions;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Features.EmployeeAttendance.WorkSchedules.DTOs;
using MicroERP.Application.Features.EmployeeAttendance.WorkSchedules.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Persistence.Queries;

public class WorkScheduleQueries : IWorkScheduleQueries
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;


    public WorkScheduleQueries(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }



    public async Task<WorkScheduleDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.WorkSchedules
            .Where(x => x.Id == id)
            .ProjectTo<WorkScheduleDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }



    public async Task<List<WorkScheduleDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.WorkSchedules
            .ProjectTo<WorkScheduleDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}