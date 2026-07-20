using MicroERP.Application.Authorization.Interfaces;
using MicroERP.Application.Authorization.Permissions;

namespace MicroERP.Application.Authorization.Providers;

public class EmployeeDefinitionProvider
    : IPermissionDefinitionProvider
{
    public PermissionGroupDefinition Group =>
        new(
            key: "Employee",
            name: "Employee",
            description: "Basic employee permissions.",
            isSystem: true
        );


    public IEnumerable<PermissionDefinition> GetPermissions()
    {
        return
        [
            new(
                group: Group.Key,
                key: EmployeePermissions.Profile.View,
                name: "View Profile",
                description: "Allows employee to view own profile."
            ),

            new(
                group: Group.Key,
                key: EmployeePermissions.Profile.ChangePassword,
                name: "Change Password",
                description: "Allows employee to change own password."
            ),
            //==============================EmployeeLeave=======================
             new(
                Group.Key,
                HRPermissions.EmployeeLeave.Create,
                "Create Employee Leave",
                "Allows creating Employee Leave."
            ),
              new(Group.Key,
                HRPermissions.EmployeeLeave.Update,
                "Update Employee Leave",
                "Allows updating Employee Leave."
            ),

            new(Group.Key,
                HRPermissions.EmployeeLeave.Delete,
                "Delete Employee Leave",
                "Allows deleting Employee Leave."
            ),
        ];
    }
}