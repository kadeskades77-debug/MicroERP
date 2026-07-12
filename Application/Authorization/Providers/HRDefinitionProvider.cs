using MicroERP.Application.Authorization.Interfaces;

namespace MicroERP.Application.Authorization.Providers;

public class HRDefinitionProvider : IPermissionDefinitionProvider
{
    public PermissionGroupDefinition Group =>
     new(
         key: "HR",
         name: "Human Resources",
         description: "Human Resources Module",
         isSystem: true
     );

    public IEnumerable<PermissionDefinition> GetPermissions()
    {
        return
        [
            new(
                Group.Key,  
                HRPermissions.Employee.View,
                "View Employee",
                "Allows viewing employees."
            ),

            new(
                Group.Key,
                HRPermissions.Employee.Create,
                "Create Employee",
                "Allows creating employees."
            ),

            new(Group.Key,
                HRPermissions.Employee.TransferDepartment,
                "Transfer Employee",
                "Allows Transfering employees between Departmints."
            ),
            new(Group.Key,
                HRPermissions.Employee.UpdateSalary,
                "Update Employee Salary",
                "Allows updating employee salaries."
            ),
            new(Group.Key,
                HRPermissions.Employee.Update,
                "Update Employee",
                "Allows updating employees."
            ),

            new(Group.Key,
                HRPermissions.Employee.Delete,
                "Delete Employee",
                "Allows deleting employees."
            ),

            new(Group.Key,
                HRPermissions.Department.View,
                "View Department",
                "Allows viewing departments."
            ),

            new(Group.Key,
                HRPermissions.Department.Create,
                "Create Department",
                "Allows creating departments."
            ),

            new(Group.Key,
                HRPermissions.Department.Update,
                "Update Department",
                "Allows updating departments."
            ),

            new(Group.Key,
                HRPermissions.Department.AssignManager,
                "AssignManager Department",
                "Allows Assigning Manager departments."
            ),
            new(Group.Key,
                HRPermissions.Department.Delete,
                "Delete Department",
                "Allows deleting departments."
            )
        ];
    }
}