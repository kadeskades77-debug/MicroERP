namespace MicroERP.Application.Authorization.Interfaces;

public interface IAuthorizationManager
{
    Task<IReadOnlyList<string>> GetPermissionsByUserAsync(string userId);
    
    Task ClearUserPermissionsCacheAsync(string userId);
    Task ClearUsersPermissionsCacheAsync(IEnumerable<string> userIds);

}