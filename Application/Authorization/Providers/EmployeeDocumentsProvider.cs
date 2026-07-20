using MicroERP.Application.Authorization.Interfaces;
using MicroERP.Application.Authorization.Permissions;

namespace MicroERP.Application.Authorization.Providers
{
    public class EmployeeDocumentsProvider
        : IPermissionDefinitionProvider
    {
        public PermissionGroupDefinition Group =>
      new(
          key: "EmployeeDocuments",
          name: "Employee Documents",
          description: " EmployeeDocuments Module.",
          isSystem: true
      );
        public IEnumerable<PermissionDefinition> GetPermissions()
        {
            return
            [
                new(
                Group.Key,
                EmployeeDocumentsPermissions.EmployeeDocuments.View,
                "View EmployeeDocuments",
                "Allows viewing EmployeeDocuments."
            ),

            new(
                Group.Key,
                EmployeeDocumentsPermissions.EmployeeDocuments.Create,
                "Create EmployeeDocuments",
                "Allows creating EmployeeDocuments."
            ),

            new(Group.Key,
                EmployeeDocumentsPermissions.EmployeeDocuments.Update,
                "Update EmployeeDocuments",
                "Allows updating EmployeeDocuments."
            ),

            new(Group.Key,
                EmployeeDocumentsPermissions.EmployeeDocuments.Delete,
                "Delete EmployeeDocuments",
                "Allows deleting EmployeeDocuments."
            )

            ];
        }
    }
}
