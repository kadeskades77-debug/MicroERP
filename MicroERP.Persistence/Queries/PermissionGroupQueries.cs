using MicroERP.Application.Common.Interfaces;
using MicroERP.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using MicroERP.Application.Common.Mappings;
using MicroERP.Application.Features.Authorization.PermissionGroups.DTOs;
using MicroERP.Application.Features.Authorization.PermissionGroups.Interfaces;
namespace MicroERP.Persistence.Queries
{
    public class PermissionGroupQueries : IPermissionGroupQueries
    {
        private readonly IApplicationDbContext _context;

        public PermissionGroupQueries(
            IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PermissionGroup?> GetById(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _context.PermissionGroups
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        public async Task<List<PermissionGroupDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var groups =
                await _context.PermissionGroups
                    .AsNoTracking()
                    .Include(x => x.PermissionGroupPermissions)
                        .ThenInclude(x => x.Permission)
                    .OrderBy(x => x.Name)
                    .ToListAsync(cancellationToken);

            return groups
                .Select(x => x.ToDto())
                .ToList();
        }

        public async Task<PermissionGroupDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var group =
                await _context.PermissionGroups
                    .AsNoTracking()
                    .Include(x => x.PermissionGroupPermissions)
                        .ThenInclude(x => x.Permission)
                    .FirstOrDefaultAsync(
                        x => x.Id == id,
                        cancellationToken);

            if (group is null)
                return null;

            return group.ToDto();
        }

        public async Task<bool> ExistsByKeyAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            return await _context.PermissionGroups
                .AnyAsync(
                    x => x.Key.ToUpper() == key.ToUpper(),
                    cancellationToken);
        }

        public async Task<bool> ExistsByKeyAsync(string key,int excludeId,
            CancellationToken cancellationToken = default)
        {
            return await _context.PermissionGroups
                .AnyAsync(
                    x =>
                        x.Id != excludeId &&
                        x.Key.ToUpper() == key.ToUpper(),
                    cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(string name,
            CancellationToken cancellationToken = default)
        {
            return await _context.PermissionGroups
                .AnyAsync(
                    x => x.Name.ToUpper() == name.ToUpper(),
                    cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(string name,int excludeId,
            CancellationToken cancellationToken = default)
        {
            return await _context.PermissionGroups
                .AnyAsync(
                    x =>
                        x.Id != excludeId &&
                        x.Name.ToUpper() == name.ToUpper(),
                    cancellationToken);
        }
    }
}