namespace MicroERP.Application.Authorization;

public class PermissionDefinition
{
    public PermissionDefinition(
        string groupKey,
        string key,
        string name,
        string? description = null)
    {
        GroupKey = groupKey;
        Key = key;
        Name = name;
        Description = description;
    }

    public string GroupKey { get; }

    public string Key { get; }

    public string Name { get; }

    public string? Description { get; }
}