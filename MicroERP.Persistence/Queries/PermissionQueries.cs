using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Mappings;
using MicroERP.Application.Features.Authorization.Permissions.DTOs;
using MicroERP.Application.Features.Authorization.Permissions.Interfaces;
using MicroERP.Application.Features.Authorization.Permissions.Validators;
using MicroERP.Domain.Identity;
using Microsoft.EntityFrameworkCore;
namespace MicroERP.Persistence.Queries
{
    public class PermissionQueries : IPermissionQueries
    {
        private readonly IApplicationDbContext _context;

        public PermissionQueries(IApplicationDbContext context)
        {
            _context = context
                ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<PermissionDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Permissions
                .AsNoTracking()
                .OrderBy(x => x.Key)
                .Select(x => x.ToDto())
                .ToListAsync(cancellationToken);
        }

        public async Task<List<PermissionDto>> GetAvailablePermissionsAsync(int groupId,
     CancellationToken cancellationToken = default)
        {
            return await _context.Permissions
                .AsNoTracking()
                .Where(p =>
                    !_context.PermissionGroupPermissions
                        .Any(pg =>
                            pg.PermissionId == p.Id &&
                            pg.PermissionGroupId == groupId))
                .OrderBy(p => p.Name)
                .Select(x => x.ToDto())
                .ToListAsync(cancellationToken);
        }

        public async Task<Permission?> GetById(int id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Permissions
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        public async Task<PermissionDto?> GetByIdAsync(int id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Permissions
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => x.ToDto())
                .FirstOrDefaultAsync(
                    cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(
            string name,
            int excludeId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Permissions
                .AnyAsync(
                    x =>
                        x.Name == name &&
                        x.Id != excludeId,
                    cancellationToken);
        }
    }
}