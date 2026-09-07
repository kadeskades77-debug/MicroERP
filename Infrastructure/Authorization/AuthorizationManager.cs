using MicroERP.Application.Authorization.Interfaces;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Domin.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace MicroERP.Infrastructure.Authorization
{
    public class AuthorizationManager : IAuthorizationManager
    {
        private readonly IApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthorizationManager(
            IApplicationDbContext context,
            IMemoryCache cache,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _cache = cache;
            _userManager = userManager;
        }

        // ================= USER PERMISSIONS =================
        public async Task<IReadOnlyList<string>> GetPermissionsByUserAsync(string userId)
        {
            var cacheKey = GetUserCacheKey(userId);

            if (_cache.TryGetValue(
                cacheKey,
                out List<string>? cachedPermissions))
            {
                return cachedPermissions!;
            }

            var directPermissions =
                _context.UserPermissionAssignments
                    .Where(x => x.UserId == userId)
                    .SelectMany(x =>
                        x.PermissionGroup.PermissionGroupPermissions)
                    .Select(x => x.Permission.Key);

            var rolePermissions =
                _context.UserRoles
                    .Where(x => x.UserId == userId)
                    .Join(
                        _context.RolePermissionGroups,
                        userRole => userRole.RoleId,
                        roleGroup => roleGroup.RoleId,
                        (userRole, roleGroup) => roleGroup)
                    .SelectMany(x =>
                        x.PermissionGroup.PermissionGroupPermissions)
                    .Select(x => x.Permission.Key);

            var permissions =
                await directPermissions
                    .Concat(rolePermissions)
                    .Distinct()
                    .ToListAsync();

            _cache.Set(
                cacheKey,
                permissions,
                TimeSpan.FromMinutes(30));

            return permissions;
        }
        
        // ================= CACHE INVALIDATION =================
        public Task ClearUserPermissionsCacheAsync(string userId)
        {
            _cache.Remove(
                GetUserCacheKey(userId));

            return Task.CompletedTask;
        }

        public async Task ClearUsersPermissionsCacheAsync(IEnumerable<string> userIds)
        {
            foreach (var userId in userIds.Distinct())
            {
                _cache.Remove(
                    GetUserCacheKey(userId));
            }

            await Task.CompletedTask;
        }

        public async Task ClearRoleUsersPermissionsCacheAsync(string roleId,
        CancellationToken cancellationToken = default)
        {
            var userIds = await _context.UserRoles
                .Where(x => x.RoleId == roleId)
                .Select(x => x.UserId)
                .Distinct()
                .ToListAsync(cancellationToken);

            await ClearUsersPermissionsCacheAsync(userIds);
        }

        // ================= HELPER  =======================
        private static string GetUserCacheKey(string userId)
        {
            return $"permissions:user:{userId}";
        }

    }
}
