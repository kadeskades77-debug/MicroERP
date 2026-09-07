

namespace MicroERP.Application.Features.Authorization.PermissionGroups.DTOs
{
    public class MovePermissionsBetweenGroupsDto
    {
        public string SourceGroupKey { get; set; } = null!;

        public string TargetGroupKey { get; set; } = null!;

        public List<string> PermissionKeys { get; set; } = new();
    }
}
