namespace MicroERP.Application.Authorization;

public class PermissionGroupDefinition
{
    public PermissionGroupDefinition(
        string key,
        string name,
        string? description = null,
        bool isSystem = true)
    {
        Key = key;
        Name = name;
        Description = description;
        IsSystem = isSystem;
    }

    public string Key { get; }

    public string Name { get; }

    public string? Description { get; }

    public bool IsSystem { get; }
}