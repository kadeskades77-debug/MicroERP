using MicroERP.Application.Authorization.Interfaces;


namespace MicroERP.Application.Authorization.Services;

public class PermissionDefinitionService
    : IPermissionDefinitionService
{
    private readonly IEnumerable<IPermissionDefinitionProvider>
        _providers;

    private readonly IEnumerable<IMultiPermissionDefinitionProvider>
        _multiProviders;

    public PermissionDefinitionService(
        IEnumerable<IPermissionDefinitionProvider> providers,
        IEnumerable<IMultiPermissionDefinitionProvider> multiProviders)
    {
        _providers = providers;
        _multiProviders = multiProviders;
    }

    // =========================================================
    // Get All Permission Groups
    // =========================================================

    public IEnumerable<PermissionGroupDefinition> GetGroups()
    {
        var singleGroups =
            _providers
                .Select(x => x.Group);

        var multipleGroups =
            _multiProviders
                .SelectMany(x => x.Groups);

        return singleGroups
            .Concat(multipleGroups)
            .GroupBy(x => x.Key)
            .Select(x => x.First());
    }

    // =========================================================
    // Get All Permissions
    // =========================================================

    public IEnumerable<PermissionDefinition> GetPermissions()
    {
        var singleProviderPermissions =
            _providers
                .SelectMany(x => x.GetPermissions());

        var multipleProviderPermissions =
            _multiProviders
                .SelectMany(x => x.GetPermissions());

        return singleProviderPermissions
            .Concat(multipleProviderPermissions)
            .GroupBy(x => x.Key)
            .Select(x => x.First());
    }

    // =========================================================
    // Get Group By Key
    // =========================================================

    public PermissionGroupDefinition? GetGroup(
        string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return null;

        return GetGroups()
            .FirstOrDefault(x =>
                x.Key.Equals(
                    key,
                    StringComparison.OrdinalIgnoreCase));
    }

    // =========================================================
    // Get Permission By Key
    // =========================================================

    public PermissionDefinition? GetPermission(
        string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return null;

        return GetPermissions()
            .FirstOrDefault(x =>
                x.Key.Equals(
                    key,
                    StringComparison.OrdinalIgnoreCase));
    }
}