using MicroERP.Application.Common.DTOs;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.Roles.DTOs;
using MicroERP.Application.Features.Roles.Interfaces;
using MicroERP.Domain.Audit;
using MicroERP.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Infrastructure.Services;

public class RoleService : IRoleService
{
    private readonly IApplicationDbContext _context;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IAuditService _auditService;

    public RoleService(
        IApplicationDbContext context,
        RoleManager<ApplicationRole> roleManager,
        IAuditService auditService)
    {
        _context = context;
        _roleManager = roleManager;
        _auditService = auditService;
    }



    //================ GET ALL =================

    public async Task<Result<List<RoleDto>>> GetAllAsync()
    {
        var roles = await _roleManager.Roles
            .OrderBy(x => x.Name)
            .ToListAsync();


        var result = roles.Select(x => new RoleDto
        {
            Id = x.Id,
            Name = x.Name!,
            Description = x.Description,
            IsSystem = x.IsSystem,
            PermissionGroupKeys = new List<string>()
        })
        .ToList();


        return Result<List<RoleDto>>
            .Succeeded(result);
    }

    //================ GET BY ID =================

    public async Task<Result<RoleDto>> GetByIdAsync(string id)
    {
        var role = await _roleManager.Roles
            .FirstOrDefaultAsync(x => x.Id == id);


        if (role is null)
            return Result<RoleDto>
                .Failure("Role not found.");


        return Result<RoleDto>.Succeeded(
            new RoleDto
            {
                Id = role.Id,
                Name = role.Name!,
                Description = role.Description,
                IsSystem = role.IsSystem,
                PermissionGroupKeys = new List<string>()
            });
    }

    //================ CREATE =================

    public async Task<Result> CreateAsync(CreateRoleDto dto)
    {
        return await ExecuteInTransaction(async () =>
        {
            var normalizedName =
                _roleManager.NormalizeKey(
                    dto.Name.Trim());



            var exists =
                await _roleManager.Roles
                .AnyAsync(x =>
                    x.NormalizedName == normalizedName);



            if (exists)
                return Result.Failure(
                    "Role already exists.");



            var role = new ApplicationRole
            {
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim(),
                IsSystem = false
            };



            var identityResult =
                await _roleManager.CreateAsync(role);



            var result =
                HandleIdentityResult(identityResult);



            if (!result.Success)
                return result;
            await _auditService.LogAsync(
      AuditActions.Create,
      nameof(ApplicationRole),
      role.Id,
      null,
      new
      {
          role.Name,
          role.Description,
      });


            return Result.Succeeded(
                "Role created successfully.");
        });
    }


    //================ UPDATE =================

    public async Task<Result> UpdateAsync(string id,UpdateRoleDto dto)
    {
        return await ExecuteInTransaction(async () =>
        {
            var role = await _roleManager.Roles
                .FirstOrDefaultAsync(x => x.Id == id);

            if (role is null)
                return Result.Failure(
                    "Role not found.");

            if (role.IsSystem)
                return Result.Failure(
                    "System roles cannot be modified.");


            var oldValues = new
            {
                role.Name,
                role.Description,

                PermissionGroups = await _context.RolePermissionGroups
                    .Where(x => x.RoleId == role.Id)
                    .Select(x => x.PermissionGroup.Key)
                    .ToListAsync()
            };


            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                var name = dto.Name.Trim();

                var normalizedName =
                    _roleManager.NormalizeKey(name);

                var exists = await _roleManager.Roles
                    .AnyAsync(x =>
                        x.Id != id &&
                        x.NormalizedName == normalizedName);

                if (exists)
                    return Result.Failure(
                        "Role name already exists.");

                role.Name = name;
            }


            if (dto.Description != null)
            {
                role.Description = dto.Description.Trim();
            }


            var identityResult =
                await _roleManager.UpdateAsync(role);

            var result =
                HandleIdentityResult(identityResult);

            if (!result.Success)
                return result;


            await _auditService.LogAsync(
                AuditActions.Update,
                nameof(ApplicationRole),
                role.Id,
                oldValues,
                new
                {
                    role.Name,
                    role.Description
                });

            return Result.Succeeded(
                "Role updated successfully.");
        });
    }

    //================ DELETE =================

    public async Task<Result> DeleteAsync(string id)
    {
        return await ExecuteInTransaction(async () =>
        {
            var role =
                await _roleManager.Roles
                .FirstOrDefaultAsync(x =>
                    x.Id == id);



            if (role is null)
                return Result.Failure(
                    "Role not found.");



            if (role.IsSystem)
                return Result.Failure(
                    "System roles cannot be deleted.");
            var oldValues = new
            {
                role.Name,
                role.Description,

                PermissionGroups = await _context.RolePermissionGroups
        .Where(x => x.RoleId == role.Id)
        .Select(x => x.PermissionGroup.Key)
        .ToListAsync()
            };


            var hasUsers =
                await _context.UserRoles
                .AnyAsync(x =>
                    x.RoleId == id);



            if (hasUsers)
                return Result.Failure(
                    "Role is assigned to users.");



            var identityResult =
                await _roleManager.DeleteAsync(role);



            if (!identityResult.Succeeded)
                return HandleIdentityResult(
                    identityResult);

            await _auditService.LogAsync(
                AuditActions.Delete,
                nameof(ApplicationRole),
                role.Id,
                oldValues,
                null);


            return Result.Succeeded(
                "Role deleted successfully.");
        });
    }


    //================ HELPERS =================

    public async Task<List<LookupDto>> GetLookupAsync()
    {
        return await _roleManager.Roles
            .OrderBy(x => x.Name)
            .Select(x => new LookupDto
            {
                Value = x.Id,
                Text = x.Name!
            })
            .ToListAsync();
    }

    private static Result HandleIdentityResult(IdentityResult result)
    {
        if (result.Succeeded)
            return Result.Succeeded();


        return Result.Failure(
            string.Join(
                Environment.NewLine,
                result.Errors
                .Select(x => x.Description)));
    }

    private async Task<Result> ExecuteInTransaction(Func<Task<Result>> action)
    {
        var strategy =
            _context.Database
            .CreateExecutionStrategy();



        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction =
                await _context.Database
                .BeginTransactionAsync();



            try
            {
                var result =
                    await action();



                if (!result.Success)
                    return result;



                await _context.SaveChangesAsync();


                await transaction.CommitAsync();



                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }
}