using AutoMapper;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.Payrolls.SalaryComponents.DTOs;
using MicroERP.Application.Features.Payrolls.SalaryComponents.Interfaces;
using MicroERP.Domin.Entities.Payrolls;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.Payrolls.SalaryComponents.Services;

public class SalaryComponentService : ISalaryComponentService
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IAuditService _auditService;

    public SalaryComponentService(
        IApplicationDbContext context,
        IMapper mapper,
        IAuditService auditService)
    {
        _context = context;
        _mapper = mapper;
        _auditService = auditService;
    }



    public async Task<Result<SalaryComponentDto>> CreateAsync(
        CreateSalaryComponentDto dto,
        CancellationToken cancellationToken)
    {
        var codeExists = await _context.SalaryComponents
            .AnyAsync(
                x => x.Code == dto.Code,
                cancellationToken);


        if (codeExists)
        {
            return Result<SalaryComponentDto>
                .Failure("Salary component code already exists");
        }

      

        var entity = _mapper.Map<SalaryComponent>(dto);


        await _context.SalaryComponents
            .AddAsync(entity, cancellationToken);


        await _context.SaveChangesAsync(
            cancellationToken);



        var result = _mapper.Map<SalaryComponentDto>(
            entity);


        return Result<SalaryComponentDto>
            .Succeeded(result);
    }





    public async Task<Result<SalaryComponentDto>> UpdateAsync(int id,
      UpdateSalaryComponentDto dto,
      CancellationToken cancellationToken)
    {
        var entity = await _context.SalaryComponents
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);


        if (entity == null)
        {
            return Result<SalaryComponentDto>
                .Failure("Salary component not found");
        }

        var component =
        await _context.SalaryComponents
        .FirstOrDefaultAsync(
            x => x.Id == id,
            cancellationToken);

        var oldValues = new
        {
            component.NameAr,
            component.NameEn,
            component.Code,
            component.Type,
            component.CalculationType,
            component.IsTaxable,
            component.IsAttendanceRelated,
            component.IsDefault
        };

        if (dto.NameAr != null)
            entity.NameAr = dto.NameAr;


        if (dto.NameEn != null)
            entity.NameEn = dto.NameEn;


        if (dto.Code != null)
            entity.Code = dto.Code;


        if (dto.Type.HasValue)
            entity.Type = dto.Type.Value;


        if (dto.CalculationType.HasValue)
            entity.CalculationType = dto.CalculationType.Value;


        if (dto.IsTaxable.HasValue)
            entity.IsTaxable = dto.IsTaxable.Value;


        if (dto.IsAttendanceRelated.HasValue)
            entity.IsAttendanceRelated = dto.IsAttendanceRelated.Value;


        if (dto.IsDefault.HasValue)
            entity.IsDefault = dto.IsDefault.Value;
        var newValues = new
        {
            component.NameAr,
            component.NameEn,
            component.Code,
            component.Type,
            component.CalculationType,
            component.IsTaxable,
            component.IsAttendanceRelated,
            component.IsDefault
        };
        await _auditService.LogAsync(
               "Update",
               "SalaryComponent",
               component.Id.ToString(),
               oldValues,
               newValues);


        await _context.SaveChangesAsync(
            cancellationToken);



        return Result<SalaryComponentDto>
            .Succeeded(
                _mapper.Map<SalaryComponentDto>(entity));
    }





    public async Task<Result> DeleteAsync(int id,
        CancellationToken cancellationToken)
    {
        var component =
         await _context.SalaryComponents
         .FirstOrDefaultAsync(
             x => x.Id == id,
             cancellationToken);



        if (component == null)
            return Result.Failure(
                "Salary component not found");



        var usedInPayroll = await _context.PayrollItems
            .AnyAsync(
                x => x.SalaryComponentId == id,
                cancellationToken);



        if (usedInPayroll)
        {
            return Result.Failure(
                "Cannot delete salary component used in payroll");
        }

        var oldValues = new
        {
            component.NameAr,
            component.NameEn,
            component.Code,
            component.Type,
            component.CalculationType,
            component.IsTaxable,
            component.IsAttendanceRelated,
            component.IsDefault
        };

        component.IsDeleted = true;

        await _auditService.LogAsync(
       "Delete",
       "SalaryComponent",
       id.ToString(),
       oldValues,
       null);

        await _context.SaveChangesAsync(
            cancellationToken);



        return Result.Succeeded(
       "Salary component deleted successfully.");
    }
}