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
            ),

       //============================== Attendance =======================
            
            new(
                Group.Key,
                HRPermissions.Attendance.View,
                "View Attendance Records",
                "Allows viewing attendance records."
            ),
            
            new(
                Group.Key,
                HRPermissions.Attendance.Create,
                "Create Attendance Record",
                "Allows creating attendance records manually."
            ),
            
            new(
                Group.Key,
                HRPermissions.Attendance.Update,
                "Update Attendance Record",
                "Allows updating attendance records."
            ),
            
            new(
                Group.Key,
                HRPermissions.Attendance.Export,
                "Export Attendance Records",
                "Allows exporting attendance records."
            ),
            
            new(
                Group.Key,
                HRPermissions.Attendance.CheckIn,
                "Record Employee Check-In",
                "Allows recording employee check-in."
            ),
            
            new(
                Group.Key,
                HRPermissions.Attendance.CheckOut,
                "Record Employee Check-Out",
                "Allows recording employee check-out."
            ),
            
            new(
                Group.Key,
                HRPermissions.Attendance.Approve,
                "Approve Attendance Requests",
                "Allows approving attendance adjustment requests."
            ),
            
            new(
                Group.Key,
                HRPermissions.Attendance.Reject,
                "Reject Attendance Requests",
                "Allows rejecting attendance adjustment requests."
            ),
            new(
                Group.Key,
                HRPermissions.AttendanceCorrection.View,
                "View Attendance Corrections",
                "Allows viewing attendance correction requests."
            ),
            
            
            new(
                Group.Key,
                HRPermissions.AttendanceCorrection.Create,
                "Create Attendance Correction",
                "Allows creating attendance correction requests."
            ),
            
            
            new(
                Group.Key,
                HRPermissions.AttendanceCorrection.Approve,
                "Approve Attendance Correction",
                "Allows approving attendance correction requests."
            ),
            
            
            new(
                Group.Key,
                HRPermissions.AttendanceCorrection.Reject,
                "Reject Attendance Correction",
                "Allows rejecting attendance correction requests."
            ),
            
            //============================== Work Schedule =======================

            new(
                Group.Key,
                HRPermissions.WorkSchedule.View,
                "View Work Schedules",
                "Allows viewing employee work schedules."
            ),
            
            new(
                Group.Key,
                HRPermissions.WorkSchedule.Create,
                "Create Work Schedule",
                "Allows creating new work schedules."
            ),
            
            new(
                Group.Key,
                HRPermissions.WorkSchedule.Update,
                "Update Work Schedule",
                "Allows updating existing work schedules."
            ),
            
            new(
                Group.Key,
                HRPermissions.WorkSchedule.Delete,
                "Delete Work Schedule",
                "Allows deleting work schedules."
            ),

            //============================== Attendance Devices =======================

            new(
                Group.Key,
                HRPermissions.AttendanceDevice.View,
                "View Attendance Devices",
                "Allows viewing attendance devices."
            ),
            
            new(
                Group.Key,
                HRPermissions.AttendanceDevice.Create,
                "Create Attendance Device",
                "Allows creating attendance devices."
            ),
            
            new(
                Group.Key,
                HRPermissions.AttendanceDevice.Update,
                "Update Attendance Device",
                "Allows updating attendance devices."
            ),
            
            new(
                Group.Key,
                HRPermissions.AttendanceDevice.Delete,
                "Delete Attendance Device",
                "Allows deleting attendance devices."
            ),
            
            new(
                Group.Key,
                HRPermissions.AttendanceDevice.ReceiveLogs,
                "Receive Attendance Device Logs",
                "Allows receiving attendance logs from devices."
            ),
            //==============================Holidays=======================

            new(
                Group.Key,
                HRPermissions.Holidays.View,
                "View Holidays",
                "Allows viewing holidays."
            ),
            
            new(
                Group.Key,
                HRPermissions.Holidays.Create,
                "Create Holiday",
                "Allows creating holidays."
            ),
            
            new(
                Group.Key,
                HRPermissions.Holidays.Update,
                "Update Holiday",
                "Allows updating holidays."
            ),
            
            new(
                Group.Key,
                HRPermissions.Holidays.Delete,
                "Delete Holiday",
                "Allows deleting holidays."
            ),

            //==============================Payroll=======================
            
            new(
                Group.Key,
                HRPermissions.Payroll.View,
                "View Payroll",
                "Allows viewing payroll records."
            ),
            
            new(
                Group.Key,
                HRPermissions.Payroll.Create,
                "Create Payroll",
                "Allows creating and generating payroll."
            ),
            
            new(
                Group.Key,
                HRPermissions.Payroll.Update,
                "Update Payroll",
                "Allows Updateing and generating payroll."
            ),
            
            new(
                Group.Key,
                HRPermissions.Payroll.Delete,
                "Delete Payroll",
                "Allows Deleteing and generating payroll."
            ),
            
            new(
                Group.Key,
                HRPermissions.Payroll.Approve,
                "Approve Payroll",
                "Allows approving payroll records."
            ),
            
            new(
                Group.Key,
                HRPermissions.Payroll.Pay,
                "Pay Payroll",
                "Allows marking payroll as paid."
            ),
            //============================== Payroll Loans =======================
            
            new(
                Group.Key,
                HRPermissions.PayrollLoans.View,
                "View Payroll Loans",
                "Allows viewing employee loan records and installments."
            ),
            
            new(
                Group.Key,
                HRPermissions.PayrollLoans.Create,
                "Create Payroll Loan",
                "Allows creating employee loans and generating their installments."
            ),
            
            new(
                Group.Key,
                HRPermissions.PayrollLoans.Update,
                "Update Payroll Loan",
                "Allows updating employee loan information."
            ),
            
            new(
                Group.Key,
                HRPermissions.PayrollLoans.Cancel,
                "Cancel Payroll Loan",
                "Allows cancelling employee loans."
            ),
            
            new(
                Group.Key,
                HRPermissions.PayrollLoans.Suspend,
                "Suspend Payroll Loan",
                "Allows suspending active employee loans."
            ),
            
            new(
                Group.Key,
                HRPermissions.PayrollLoans.Resume,
                "Resume Payroll Loan",
                "Allows resuming suspended employee loans and rescheduling skipped installments."
            ),
            //==============================
            // Overtime
            //==============================
            
            new(
                Group.Key,
                HRPermissions.Overtime.View,
                "View Overtime",
                "Allows viewing employee overtime records."
            ),
            
            
            new(
                Group.Key,
                HRPermissions.Overtime.Create,
                "Create Overtime",
                "Allows creating overtime requests."
            ),
            
            
            new(
                Group.Key,
                HRPermissions.Overtime.Update,
                "Update Overtime",
                "Allows updating overtime requests."
            ),
            
            
            new(
                Group.Key,
                HRPermissions.Overtime.Delete,
                "Delete Overtime",
                "Allows deleting overtime requests."
            ),
            
            
            new(
                Group.Key,
                HRPermissions.Overtime.Approve,
                "Approve Overtime",
                "Allows approving overtime requests."
            ),
            
            
            new(
                Group.Key,
                HRPermissions.Overtime.Reject,
                "Reject Overtime",
                "Allows rejecting overtime requests."
            ),
            //============================== Employee Evaluations =======================
               
               new(
                   Group.Key,
                   HRPermissions.EmployeeEvaluations.View,
                   "View Employee Evaluations",
                   "Allows viewing employee evaluation records."
               ),
               
               new(
                   Group.Key,
                   HRPermissions.EmployeeEvaluations.Create,
                   "Create Employee Evaluation",
                   "Allows creating employee evaluations."
               ),
               
               new(
                   Group.Key,
                   HRPermissions.EmployeeEvaluations.Update,
                   "Update Employee Evaluation",
                   "Allows updating employee evaluation records."
               ),
               
               new(
                   Group.Key,
                   HRPermissions.EmployeeEvaluations.Delete,
                   "Delete Employee Evaluation",
                   "Allows deleting employee evaluation records."
               ),
               
               new(
                   Group.Key,
                   HRPermissions.EmployeeEvaluations.Submit,
                   "Submit Employee Evaluation",
                   "Allows submitting employee evaluations for review."
               ),
               
               new(
                   Group.Key,
                   HRPermissions.EmployeeEvaluations.Approve,
                   "Approve Employee Evaluation",
                   "Allows approving employee evaluations."
               ),
               
               new(
                   Group.Key,
                   HRPermissions.EmployeeEvaluations.Reject,
                   "Reject Employee Evaluation",
                   "Allows rejecting employee evaluations."
               ),
               
               new(
                   Group.Key,
                   HRPermissions.EmployeeEvaluations.Export,
                   "Export Employee Evaluations",
                   "Allows exporting employee evaluation reports."
               ),

        ];
    }
}