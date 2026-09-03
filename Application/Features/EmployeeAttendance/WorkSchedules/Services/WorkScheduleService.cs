using AutoMapper;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.WorkSchedules.DTOs;
using MicroERP.Application.Features.EmployeeAttendance.WorkSchedules.Interfaces;
using MicroERP.Domin.Entities.EmployeeAttendance;
using Microsoft.EntityFrameworkCore;


namespace MicroERP.Application.Features.EmployeeAttendance.WorkSchedules.Services;

public class WorkScheduleService : IWorkScheduleService
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;


    public WorkScheduleService(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<Result<WorkScheduleDto>> CreateAsync(
        CreateWorkScheduleDto dto,
        CancellationToken cancellationToken = default)
    {
        var exists = await _context.WorkSchedules
            .AnyAsync(
                x => x.Name == dto.Name,
                cancellationToken);


        if (exists)
        {
            return Result<WorkScheduleDto>
                .Failure("Work schedule name already exists.");
        }



        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);



        if (dto.IsDefault)
        {
            await _context.WorkSchedules
                .Where(x => x.IsDefault)
                .ExecuteUpdateAsync(
                    x => x.SetProperty(
                        p => p.IsDefault,
                        false),
                    cancellationToken);
        }



        var entity = _mapper.Map<WorkSchedule>(dto);



        _context.WorkSchedules.Add(entity);


        await _context.SaveChangesAsync(
            cancellationToken);



        await transaction.CommitAsync(
            cancellationToken);



        return Result<WorkScheduleDto>.Succeeded(
            _mapper.Map<WorkScheduleDto>(entity));
    }


    public async Task<Result<WorkScheduleDto>> UpdateAsync(int id,UpdateWorkScheduleDto dto,
     CancellationToken cancellationToken = default)
    {
        var schedule = await _context.WorkSchedules
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);


        if (schedule == null)
            return Result<WorkScheduleDto>
                .Failure("Work schedule not found.");



        if (dto.Name != null)
        {
            var exists = await _context.WorkSchedules
                .AnyAsync(
                    x => x.Name == dto.Name && x.Id != id,
                    cancellationToken);

            if (exists)
                return Result<WorkScheduleDto>
                    .Failure("Work schedule name already exists.");

            schedule.Name = dto.Name;
        }



        if (dto.FirstShiftStart.HasValue)
            schedule.FirstShiftStart =
                dto.FirstShiftStart.Value;


        if (dto.FirstShiftEnd.HasValue)
            schedule.FirstShiftEnd =
                dto.FirstShiftEnd.Value;



        if (dto.SecondShiftStart.HasValue)
            schedule.SecondShiftStart =
                dto.SecondShiftStart;


        if (dto.SecondShiftEnd.HasValue)
            schedule.SecondShiftEnd =
                dto.SecondShiftEnd;



        if (dto.LateGraceMinutes.HasValue)
            schedule.LateGraceMinutes =
                dto.LateGraceMinutes.Value;



        if (dto.EarlyLeaveGraceMinutes.HasValue)
            schedule.EarlyLeaveGraceMinutes =
                dto.EarlyLeaveGraceMinutes.Value;



        if (dto.MinimumWorkMinutes.HasValue)
            schedule.MinimumWorkMinutes =
                dto.MinimumWorkMinutes.Value;



        if (dto.MinimumWorkMinutes.HasValue)
            schedule.MinimumWorkMinutes = dto.MinimumWorkMinutes.Value;



        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);



        if (dto.IsDefault.HasValue)
        {
            if (dto.IsDefault.Value)
            {
                await _context.WorkSchedules
                    .Where(x => x.Id != id && x.IsDefault)
                    .ExecuteUpdateAsync(
                        x => x.SetProperty(
                            p => p.IsDefault,
                            false),
                        cancellationToken);
            }

            schedule.IsDefault = dto.IsDefault.Value;
        }



        await _context.SaveChangesAsync(
            cancellationToken);



        await transaction.CommitAsync(
            cancellationToken);



        return Result<WorkScheduleDto>.Succeeded(
            _mapper.Map<WorkScheduleDto>(schedule));
    }


    public async Task<Result> DeleteAsync(int id,
    CancellationToken cancellationToken = default)
    {
        var schedule = await _context.WorkSchedules
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);


        if (schedule == null)
            return Result.Failure(
                "Work schedule not found.");



        schedule.IsDeleted = true;


        await _context.SaveChangesAsync(
            cancellationToken);


        return Result.Succeeded();
    }
   
}