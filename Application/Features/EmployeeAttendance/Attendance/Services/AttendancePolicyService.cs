using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using MicroERP.Domin.Entities.Policies;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Services;

public class AttendancePolicyService : IAttendancePolicyService
{
    private readonly IApplicationDbContext _context;


    public AttendancePolicyService(
        IApplicationDbContext context)
    {
        _context = context;
    }



    public async Task<Result<List<AttendancePolicyDto>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var policies = await _context.AttendancePolicies
            .AsNoTracking()
            .Select(x => new AttendancePolicyDto
            {
                Id = x.Id,

                Name = x.Name,

                AbsentPenaltyPoints =
                    x.AbsentPenaltyPoints,

                MissingCheckInPenaltyPoints =
                    x.MissingCheckInPenaltyPoints,

                MissingCheckOutPenaltyPoints =
                    x.MissingCheckOutPenaltyPoints,

                LateMinutesPerPenaltyPoint =
                    x.LateMinutesPerPenaltyPoint,

                LostMinutesPerPenaltyPoint =
                    x.LostMinutesPerPenaltyPoint,

                MinimumPerformanceScore =
                    x.MinimumPerformanceScore,

                IsDefault = x.IsDefault
            })
            .ToListAsync(cancellationToken);


        return Result<List<AttendancePolicyDto>>
            .Succeeded(policies);
    }


    public async Task<Result<AttendancePolicyDto>> GetByIdAsync(int id,
        CancellationToken cancellationToken = default)
    {
        var policy = await _context.AttendancePolicies
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new AttendancePolicyDto
            {
                Id = x.Id,

                Name = x.Name,

                AbsentPenaltyPoints =
                    x.AbsentPenaltyPoints,

                MissingCheckInPenaltyPoints =
                    x.MissingCheckInPenaltyPoints,

                MissingCheckOutPenaltyPoints =
                    x.MissingCheckOutPenaltyPoints,

                LateMinutesPerPenaltyPoint =
                    x.LateMinutesPerPenaltyPoint,

                LostMinutesPerPenaltyPoint =
                    x.LostMinutesPerPenaltyPoint,

                MinimumPerformanceScore =
                    x.MinimumPerformanceScore,

                IsDefault = x.IsDefault
            })
            .FirstOrDefaultAsync(cancellationToken);



        if (policy == null)
            return Result<AttendancePolicyDto>
                .Failure("Attendance policy not found");


        return Result<AttendancePolicyDto>
            .Succeeded(policy);
    }



    public async Task<Result<AttendancePolicyDto>> CreateAsync(
        CreateAttendancePolicyDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.IsDefault)
        {
            var currentDefault =
                await _context.AttendancePolicies
                .Where(x => x.IsDefault)
                .ToListAsync(cancellationToken);


            foreach (var item in currentDefault)
            {
                item.IsDefault = false;
            }
        }



        var policy = new AttendancePolicy
        {
            Name = dto.Name,

            AbsentPenaltyPoints =
                dto.AbsentPenaltyPoints,

            PartialAttendancePenaltyPoints =
                dto.PartialAttendancePenaltyPoints,

            MissingCheckInPenaltyPoints =
                dto.MissingCheckInPenaltyPoints,

            MissingCheckOutPenaltyPoints =
                dto.MissingCheckOutPenaltyPoints,

            LateMinutesPerPenaltyPoint =
                dto.LateMinutesPerPenaltyPoint,

            LostMinutesPerPenaltyPoint =
                dto.LostMinutesPerPenaltyPoint,

            MinimumWorkMinutes =
                dto.MinimumWorkMinutes,

            MinimumPerformanceScore =
                dto.MinimumPerformanceScore,

            MaximumPerformanceScore =
                dto.MaximumPerformanceScore,

            IsDefault =
                dto.IsDefault
        };


        _context.AttendancePolicies.Add(policy);


        await _context.SaveChangesAsync(
            cancellationToken);


        return Result<AttendancePolicyDto>
            .Succeeded(Map(policy));
    }



    public async Task<Result<AttendancePolicyDto>> UpdateAsync(int id,
      UpdateAttendancePolicyDto dto,
      CancellationToken cancellationToken = default)
    {
        var policy =
            await _context.AttendancePolicies
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);



        if (policy == null)
        {
            return Result<AttendancePolicyDto>
                .Failure("Attendance policy not found");
        }



        if (!string.IsNullOrWhiteSpace(dto.Name))
            policy.Name = dto.Name;


        if (dto.AbsentPenaltyPoints.HasValue)
            policy.AbsentPenaltyPoints =
                dto.AbsentPenaltyPoints.Value;


        if (dto.PartialAttendancePenaltyPoints.HasValue)
            policy.PartialAttendancePenaltyPoints =
                dto.PartialAttendancePenaltyPoints.Value;


        if (dto.MissingCheckInPenaltyPoints.HasValue)
            policy.MissingCheckInPenaltyPoints =
                dto.MissingCheckInPenaltyPoints.Value;


        if (dto.MissingCheckOutPenaltyPoints.HasValue)
            policy.MissingCheckOutPenaltyPoints =
                dto.MissingCheckOutPenaltyPoints.Value;


        if (dto.LateMinutesPerPenaltyPoint.HasValue)
            policy.LateMinutesPerPenaltyPoint =
                dto.LateMinutesPerPenaltyPoint.Value;

        if (dto.LostMinutesPerPenaltyPoint.HasValue)
            policy.LostMinutesPerPenaltyPoint =
                dto.LostMinutesPerPenaltyPoint.Value;


        if (dto.MinimumWorkMinutes.HasValue)
            policy.MinimumWorkMinutes =
                dto.MinimumWorkMinutes.Value;


        if (dto.MinimumPerformanceScore.HasValue)
            policy.MinimumPerformanceScore =
                dto.MinimumPerformanceScore.Value;


        if (dto.MaximumPerformanceScore.HasValue)
            policy.MaximumPerformanceScore =
                dto.MaximumPerformanceScore.Value;


        // تغيير السياسة الافتراضية
        if (dto.IsDefault.HasValue)
        {
            if (dto.IsDefault.Value)
            {
                var oldDefault =
                    await _context.AttendancePolicies
                    .Where(x =>
                        x.IsDefault &&
                        x.Id != policy.Id)
                    .FirstOrDefaultAsync(cancellationToken);


                if (oldDefault != null)
                {
                    oldDefault.IsDefault = false;

                    await _context.SaveChangesAsync(
                        cancellationToken);
                }


                policy.IsDefault = true;
            }
            else
            {
                policy.IsDefault = false;
            }
        }



        await _context.SaveChangesAsync(
            cancellationToken);



        return Result<AttendancePolicyDto>
            .Succeeded(Map(policy));
    }



    public async Task<Result<bool>> DeleteAsync(int id,
        CancellationToken cancellationToken = default)
    {
        var policy =
            await _context.AttendancePolicies
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);



        if (policy == null)
            return Result<bool>
                .Failure("Attendance policy not found");



        if (policy.IsDefault)
        {
            return Result<bool>
                .Failure(
                "Cannot delete default attendance policy");
        }



        policy.IsDeleted = true;


        await _context.SaveChangesAsync(
            cancellationToken);


        return Result<bool>
            .Succeeded(true);
    }


    private static AttendancePolicyDto Map(
     AttendancePolicy policy)
    {
        return new AttendancePolicyDto
        {
            Id = policy.Id,

            Name = policy.Name,


            // خصم الغياب
            AbsentPenaltyPoints =
                policy.AbsentPenaltyPoints,


            // خصم الحضور الجزئي
            PartialAttendancePenaltyPoints =
                policy.PartialAttendancePenaltyPoints,


            // خصم نسيان تسجيل الدخول
            MissingCheckInPenaltyPoints =
                policy.MissingCheckInPenaltyPoints,


            // خصم نسيان تسجيل الخروج
            MissingCheckOutPenaltyPoints =
                policy.MissingCheckOutPenaltyPoints,

            LostMinutesPerPenaltyPoint =
                policy.LostMinutesPerPenaltyPoint,


            // كل كم دقيقة تأخير = نقطة
            LateMinutesPerPenaltyPoint =
                policy.LateMinutesPerPenaltyPoint,


            // الحد الأدنى لدقائق العمل
            MinimumWorkMinutes =
                policy.MinimumWorkMinutes,


            // أقل تقييم
            MinimumPerformanceScore =
                policy.MinimumPerformanceScore,


            // أعلى تقييم
            MaximumPerformanceScore =
                policy.MaximumPerformanceScore,


            // السياسة الافتراضية
            IsDefault =
                policy.IsDefault
        };
    }
}