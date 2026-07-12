using Domin.Entities;
using MicroERP.Application.Authorization.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace MicroERP.Persistence.Authorization;

public class AuthorizationManager : IAuthorizationManager
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthorizationManager(
        ApplicationDbContext context,
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


        var permissions = await _context.UserPermissionAssignments
            .Where(x => x.UserId == userId)
            .SelectMany(x =>
                x.PermissionGroup.PermissionGroupPermissions)
            .Select(x => x.Permission.Key)
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

    // ================= HELPER  =======================
    private static string GetUserCacheKey(string userId)
    {
        return $"permissions:user:{userId}";
    }

}