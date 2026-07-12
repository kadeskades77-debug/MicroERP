namespace MicroERP.Application.Authorization;

public class PermissionDefinition
{
    public PermissionDefinition(
        string group,
        string key,
        string name,
        string? description = null)
    {
        GroupName = group;
        Key = key;
        Name = name;
        Description = description;
    }

    public string GroupName { get; }

    public string Key { get; }

    public string Name { get; }

    public string? Description { get; }
}