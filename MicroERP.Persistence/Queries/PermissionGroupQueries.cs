using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Features.PermissionGroups.DTOs;
using MicroERP.Application.Features.PermissionGroups.Interfaces;
using MicroERP.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using MicroERP.Application.Common.Mappings;
namespace MicroERP.Persistence.Queries
{
    public class PermissionGroupQueries : IPermissionGroupQueries
    {
        private readonly IApplicationDbContext _context;

        public PermissionGroupQueries(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PermissionGroup?> GetById(int id)
        {
            return await _context.PermissionGroups
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<PermissionGroupDto>> GetAllAsync()
        {
            var groups = await _context.PermissionGroups
                .Include(x => x.PermissionGroupPermissions)
                .ThenInclude(x => x.Permission)
                .OrderBy(x => x.Name)
                .ToListAsync();

            return groups
                .Select(x => x.ToDto())
                .ToList();
        }
        public async Task<PermissionGroupDto?> GetByIdAsync(int id)
        {
            var group = await _context.PermissionGroups
                .Include(x => x.PermissionGroupPermissions)
                .ThenInclude(x => x.Permission)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (group is null)
                return null;

            return group.ToDto();
        }
        public async Task<PermissionGroup?> GetByKeyAsync(string key)
        {
            return await _context.PermissionGroups
                .FirstOrDefaultAsync(x => x.Key == key);
        }

        public async Task<PermissionGroup?> GetByNameAsync(string name)
        {
            return await _context.PermissionGroups
                .FirstOrDefaultAsync(x => x.Name == name);
        }

        public async Task<PermissionGroup?> GetByIdWithPermissionsAsync(int id)
        {
            return await _context.PermissionGroups
                .Include(x => x.PermissionGroupPermissions)
                    .ThenInclude(x => x.Permission)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> ExistsByKeyAsync(string key)
        {
            return await _context.PermissionGroups
                .AnyAsync(x =>
                    x.Key.ToUpper() == key.ToUpper());
        }

        public async Task<bool> ExistsByKeyAsync(string key, int excludeId)
        {
            return await _context.PermissionGroups
                .AnyAsync(x =>
                    x.Key == key &&
                    x.Id != excludeId);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.PermissionGroups
                .AnyAsync(x =>
                    x.Name.ToUpper() == name.ToUpper());
            ;
        }

        public async Task<bool> ExistsByNameAsync(string name, int excludeId)
        {
            return await _context.PermissionGroups
                .AnyAsync(x =>
                    x.Name == name &&
                    x.Id != excludeId);
        }
    }
}