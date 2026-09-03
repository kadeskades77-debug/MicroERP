

namespace MicroERP.Application.Authorization.Requirements
{
    public class PermissionDefinition
    {
        public PermissionDefinition(
            string key,
            string name,
            string groupKey,
            string? description = null)
        {
            Key = key;
            Name = name;
            GroupKey = groupKey;
            Description = description;
        }

        public string Key { get; }

        public string Name { get; }

        public string GroupKey { get; }

        public string? Description { get; }
    }
}
