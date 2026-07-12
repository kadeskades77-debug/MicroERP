using MicroERP.Application.Authorization.Interfaces;
using MicroERP.Domain.Identity;
using MicroERP.Domin.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Persistence.Authorization;

public class PermissionSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<ApplicationRole> _roleManager;


    public PermissionSeeder(
        ApplicationDbContext context,
        RoleManager<ApplicationRole> roleManager)
    {
        _context = context;
        _roleManager = roleManager;
    }



    public async Task SeedAsync(
        IEnumerable<IPermissionDefinitionProvider> providers)
    {
        var strategy = _context.Database.CreateExecutionStrategy();


        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();


            try
            {
                await SeedPermissionDefinitionsAsync(providers);


                var superAdminGroup =
                    await EnsureSuperAdminGroupAsync();


                await EnsureSuperAdminRoleAsync();


                await LinkSuperAdminPermissionsAsync(
                    superAdminGroup);


                await AssignSuperAdminUsersAsync(
                    superAdminGroup);



                await _context.SaveChangesAsync();


                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }






    private async Task SeedPermissionDefinitionsAsync(IEnumerable<IPermissionDefinitionProvider> providers)
    {
        foreach (var provider in providers)
        {
            var groupDefinition = provider.Group;


            var group = await _context.PermissionGroups
                .FirstOrDefaultAsync(x =>
                    x.Key == groupDefinition.Key);



            if (group is null)
            {
                group = new PermissionGroup
                {
                    Key = groupDefinition.Key,
                    Name = groupDefinition.Name,
                    Description = groupDefinition.Description,
                    IsSystem = groupDefinition.IsSystem
                };


                _context.PermissionGroups.Add(group);

                await _context.SaveChangesAsync();
            }
            else if (group.IsDeleted)
            {
                group.IsDeleted = false;
            }





            foreach (var permissionDefinition
                     in provider.GetPermissions())
            {
                var permission =
                    await _context.Permissions
                    .FirstOrDefaultAsync(x =>
                        x.Key == permissionDefinition.Key);



                if (permission is null)
                {
                    permission = new Permission
                    {
                        Key = permissionDefinition.Key,
                        Name = permissionDefinition.Name,
                        Description = permissionDefinition.Description
                    };


                    _context.Permissions.Add(permission);

                    await _context.SaveChangesAsync();
                }
                else if (permission.IsDeleted)
                {
                    permission.IsDeleted = false;
                }




                var exists =
                    await _context.PermissionGroupPermissions
                    .AnyAsync(x =>
                        x.PermissionGroupId == group.Id &&
                        x.PermissionId == permission.Id);



                if (!exists)
                {
                    _context.PermissionGroupPermissions.Add(
                        new PermissionGroupPermission
                        {
                            PermissionGroupId = group.Id,
                            PermissionId = permission.Id
                        });
                }
            }
        }


        await _context.SaveChangesAsync();
    }







    private async Task<PermissionGroup>EnsureSuperAdminGroupAsync()
    {
        var group =
            await _context.PermissionGroups
            .FirstOrDefaultAsync(x =>
                x.Key == "SuperAdmin");



        if (group is null)
        {
            group = new PermissionGroup
            {
                Key = "SuperAdmin",
                Name = "Super Admin",
                Description =
                    "Full system permissions.",
                IsSystem = true
            };


            _context.PermissionGroups.Add(group);

            await _context.SaveChangesAsync();
        }
        else if (group.IsDeleted)
        {
            group.IsDeleted = false;
        }


        return group;
    }








    private async Task LinkSuperAdminPermissionsAsync(PermissionGroup group)
    {
        var allPermissions =
            await _context.Permissions.ToListAsync();



        var linkedIds =
            await _context.PermissionGroupPermissions
            .Where(x =>
                x.PermissionGroupId == group.Id)
            .Select(x => x.PermissionId)
            .ToListAsync();




        var missing =
            allPermissions
            .Where(x =>
                !linkedIds.Contains(x.Id))
            .Select(x =>
                new PermissionGroupPermission
                {
                    PermissionGroupId = group.Id,
                    PermissionId = x.Id
                });



        _context.PermissionGroupPermissions
            .AddRange(missing);
    }








    private async Task<ApplicationRole>EnsureSuperAdminRoleAsync()
    {
        var role =
            await _roleManager
            .FindByNameAsync("SuperAdmin");



        if (role is not null)
            return role;




        role = new ApplicationRole
        {
            Name = "SuperAdmin",
            Description =
                "Full system administrator role.",
            IsSystem = true
        };



        var result =
            await _roleManager
            .CreateAsync(role);



        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(
                    Environment.NewLine,
                    result.Errors
                    .Select(x => x.Description)));
        }



        return role;
    }









    private async Task AssignSuperAdminUsersAsync(PermissionGroup group)
    {
        var role =
            await _roleManager
            .FindByNameAsync("SuperAdmin");



        if (role is null)
            return;



        // استخراج جميع UserId الذين لديهم Role SuperAdmin
        var userIds =
            await _context.UserRoles
            .Where(x =>
                x.RoleId == role.Id)
            .Select(x =>
                x.UserId)
            .ToListAsync();




        foreach (var userId in userIds)
        {
            var exists =
                await _context.UserPermissionAssignments
                .AnyAsync(x =>
                    x.UserId == userId &&
                    x.PermissionGroupId == group.Id);



            if (!exists)
            {
                _context.UserPermissionAssignments.Add(
                    new UserPermissionAssignment
                    {
                        UserId = userId,
                        PermissionGroupId = group.Id
                    });
            }
        }
    }
}