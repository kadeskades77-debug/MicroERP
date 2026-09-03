using AutoMapper;
using AutoMapper.QueryableExtensions;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.Payrolls.DTOs;
using MicroERP.Application.Features.Payrolls.Interfaces;
using MicroERP.Domin.Entities.Payrolls;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.Payrolls.Services
{
    public class EmployeeSalaryComponentService
     : IEmployeeSalaryComponentService
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAuditService _auditService;


        public EmployeeSalaryComponentService(
            IApplicationDbContext context,
            IMapper mapper,
            IAuditService auditService)
        {
            _context = context;
            _mapper = mapper;
            _auditService = auditService;
        }



        public async Task<Result<EmployeeSalaryComponentDto>> CreateAsync(
            CreateEmployeeSalaryComponentDto dto,
            CancellationToken cancellationToken)
        {

            var exists = await _context.EmployeeSalaryComponents
                .AnyAsync(x =>
                    x.EmployeeId == dto.EmployeeId &&
                    x.SalaryComponentId == dto.SalaryComponentId &&
                    x.IsActiveComponent,
                    cancellationToken);


            if (exists)
            {
                return Result<EmployeeSalaryComponentDto>
                    .Failure("Employee already has this salary component");
            }



            var entity = new EmployeeSalaryComponent
            {
                EmployeeId = dto.EmployeeId,

                SalaryComponentId = dto.SalaryComponentId,

                Amount = dto.Amount,

                EffectiveFrom = dto.EffectiveFrom,

                EffectiveTo = dto.EffectiveTo,

                IsActiveComponent = true
            };


            await _context.EmployeeSalaryComponents
                .AddAsync(entity, cancellationToken);


            await _context.SaveChangesAsync(
                cancellationToken);



            var result = await _context.EmployeeSalaryComponents
                .Where(x => x.Id == entity.Id)
                .ProjectTo<EmployeeSalaryComponentDto>(
                    _mapper.ConfigurationProvider)
                .FirstAsync(cancellationToken);



            return Result<EmployeeSalaryComponentDto>
                .Succeeded(result);
        }

        public async Task<Result> UpdateAsync(int id,
       UpdateEmployeeSalaryComponentDto dto,
        CancellationToken cancellationToken = default)
        {
            var component =
                await _context.EmployeeSalaryComponents
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);


            if (component == null)
            {
                return Result.Failure(
                    "Employee salary component not found.");
            }


            var oldValues = new
            {
                component.Amount,
                component.IsActiveComponent,
                component.EffectiveFrom,
                component.EffectiveTo
            };



            if (dto.Amount.HasValue)
            {
                component.Amount =
                    dto.Amount.Value;
            }


            if (dto.IsActiveComponent.HasValue)
            {
                component.IsActiveComponent =
                    dto.IsActiveComponent.Value;
            }


            if (dto.EffectiveFrom.HasValue)
            {
                component.EffectiveFrom =
                    dto.EffectiveFrom.Value;
            }


            if (dto.EffectiveTo.HasValue)
            {
                component.EffectiveTo =
                    dto.EffectiveTo.Value;
            }



            var newValues = new
            {
                component.Amount,
                component.IsActiveComponent,
                component.EffectiveFrom,
                component.EffectiveTo
            };



            await _context.SaveChangesAsync(
                cancellationToken);



            await _auditService.LogAsync(
                "Update",
                "EmployeeSalaryComponent",
                component.Id.ToString(),
                oldValues,
                newValues);



            return Result.Succeeded(
                "Employee salary component updated successfully.");
        }


        public async Task<Result> DeleteAsync(int id,
      CancellationToken cancellationToken)
        {
            var entity =
                await _context.EmployeeSalaryComponents
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);


            if (entity == null)
            {
                return Result.Failure(
                    "Salary component not found");
            }


            var oldValues = new
            {
                entity.EmployeeId,
                entity.SalaryComponentId,
                entity.Amount,
                entity.IsActiveComponent,
                entity.EffectiveFrom,
                entity.EffectiveTo
            };


            entity.IsActiveComponent = false;

            entity.EffectiveTo =
                DateOnly.FromDateTime(
                    DateTime.UtcNow);



            var newValues = new
            {
                entity.EmployeeId,
                entity.SalaryComponentId,
                entity.Amount,
                entity.IsActiveComponent,
                entity.EffectiveFrom,
                entity.EffectiveTo
            };

            await _auditService.LogAsync(
              "Deactivate",
              "EmployeeSalaryComponent",
              entity.Id.ToString(),
              oldValues,
              newValues);

            await _context.SaveChangesAsync(
                cancellationToken);



          



            return Result.Succeeded(
                "Employee salary component deactivated successfully.");
        }
    }
}
