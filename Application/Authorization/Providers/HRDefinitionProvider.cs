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
            //==============================Employees=======================
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


//==============================Departments=======================
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
            ),

            //==============================EmployeeLeave=======================

             new(
                Group.Key,
                HRPermissions.EmployeeLeave.View,
                "View Employee Leave",
                "Allows viewing Employee Leave."
            ),

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
              new(Group.Key,
                HRPermissions.EmployeeLeave.Approve ,
                "Approve  Employee Leave",
                "Allows Approveing Employee Leave."
            ),
              new(Group.Key,
                HRPermissions.EmployeeLeave.Cancel ,
                "Cancel  Employee Leave",
                "Allows Canceling Employee Leave."
            ),
              new(Group.Key,
                HRPermissions.EmployeeLeave.ViewBalance ,
                "View Balance  Employee Leave",
                "Allows Viewing Balance Employee Leave."
            ),

            new(Group.Key,
                HRPermissions.EmployeeLeave.Reject ,
                "Reject  Employee Leave",
                "Allows Rejecting Employee Leave."
            ),


            //==============================EmployeeSpecialLeave=======================

             new(
                Group.Key,
                HRPermissions.EmployeeSpecialLeave.View,
                "View Employee Special Leave",
                "Allows viewing Employee Special Leave."
            ),

            new(
                Group.Key,
                HRPermissions.EmployeeSpecialLeave.Create,
                "Create Employee Special Leave",
                "Allows creating Employee Special Leave."
            ),
              new(Group.Key,
                HRPermissions.EmployeeSpecialLeave.Approve ,
                "Approve  Employee Special Leave",
                "Allows Approveing Employee Special Leave."
            ),
              new(Group.Key,
                HRPermissions.EmployeeSpecialLeave.Cancel ,
                "Cancel  Employee Special Leave",
                "Allows Canceling Employee Special Leave."
            ),
            new(Group.Key,
                HRPermissions.EmployeeSpecialLeave.Reject ,
                "Reject  Employee Special Leave",
                "Allows Rejecting Employee Special Leave."
            )

        ];
    }
}