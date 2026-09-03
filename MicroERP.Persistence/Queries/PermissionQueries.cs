using MicroERP.Application.Common.Interfaces;
using MicroERP.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using MicroERP.Application.Common.Mappings;
using MicroERP.Application.Features.Authorization.Permissions.DTOs;
using MicroERP.Application.Features.Authorization.Permissions.Interfaces;
namespace MicroERP.Persistence.Queries
{
    public class PermissionQueries : IPermissionQueries
    {
        private readonly IApplicationDbContext _context;

        public PermissionQueries(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<PermissionDto>> GetAllAsync()
        {
            return await _context.Permissions
            .OrderBy(x => x.Key)
            .Select(x => x.ToDto())
            .ToListAsync();
        }

        public async Task<List<PermissionDto>> GetAvailablePermissionsAsync()
        {
            var superAdminGroupId = await _context.PermissionGroups
               .Where(x => x.Name == "SuperAdmin")
               .Select(x => x.Id)
               .FirstOrDefaultAsync();
            return await _context.Permissions
            .AsNoTracking()
            .Where(p => !_context.PermissionGroupPermissions
                .Any(pg =>
                    pg.PermissionId == p.Id &&
                    pg.PermissionGroupId != superAdminGroupId))
            .OrderBy(p => p.Name)
           .Select(x => x.ToDto())
            .ToListAsync();
        }

        public async Task<Permission?> GetById(int id)
        {
            return await _context.Permissions
                .FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<PermissionDto> GetByIdAsync(int id)
        {
            return await _context.Permissions
            .Where(x => x.Id == id)
            .Select(x => x.ToDto())
            .FirstOrDefaultAsync();
        }

        public async Task<Permission?> GetByKeyAsync(string key)
        {
            return await _context.Permissions
                .FirstOrDefaultAsync(x => x.Key == key);
        }

        public async Task<List<Permission>> GetByKeysAsync(IEnumerable<string> keys)
        {
            return await _context.Permissions
                .Where(x => keys.Contains(x.Key))
                .ToListAsync();
        }

        public async Task<bool> ExistsByKeyAsync(string key)
        {
            return await _context.Permissions
                .AnyAsync(x => x.Key == key);
        }

        public async Task<bool> ExistsByKeyAsync(string key, int excludeId)
        {
            return await _context.Permissions
                .AnyAsync(x =>
                    x.Key == key &&
                    x.Id != excludeId);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Permissions
                .AnyAsync(x => x.Name == name);
        }

        public async Task<bool> ExistsByNameAsync(string name, int excludeId)
        {
            return await _context.Permissions
                .AnyAsync(x =>
                    x.Name == name &&
                    x.Id != excludeId);
        }
    }
}