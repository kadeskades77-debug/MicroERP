using MicroERP.Application.Authorization.Interfaces;
using MicroERP.Application.Authorization.Permissions;

namespace MicroERP.Application.Authorization.Providers;

public class HRDefinitionProvider
    : IMultiPermissionDefinitionProvider
{
    public IEnumerable<PermissionGroupDefinition> Groups =>
    [
        new(
            key: "Employee",
            name: "Employees",
            description: "Employee Management",
            isSystem: true),

        new(
            key: "EmployeeDepartment",
            name: "Employee Departments",
            description: "Employee Department Management",
            isSystem: true),

        new(
            key: "EmployeeSalary",
            name: "Employee Salaries",
            description: "Employee Salary Management",
            isSystem: true),

        new(
            key: "Department",
            name: "Departments",
            description: "Department Management",
            isSystem: true),

        new(
            key: "DepartmentManager",
            name: "Department Managers",
            description: "Department Manager Management",
            isSystem: true),

        new(
            key: "EmployeeLeave",
            name: "Employee Leaves",
            description: "Employee Leave Management",
            isSystem: true),

        new(
            key: "EmployeeLeaveApproval",
            name: "Employee Leave Approvals",
            description: "Employee Leave Approval Management",
            isSystem: true),

        new(
            key: "EmployeeLeaveBalance",
            name: "Employee Leave Balances",
            description: "Employee Leave Balance Management",
            isSystem: true),

        new(
            key: "EmployeeSpecialLeave",
            name: "Employee Special Leaves",
            description: "Employee Special Leave Management",
            isSystem: true),

        new(
            key: "EmployeeSpecialLeaveApproval",
            name: "Employee Special Leave Approvals",
            description: "Employee Special Leave Approval Management",
            isSystem: true),

        new(
            key: "Attendance",
            name: "Attendance",
            description: "Attendance Management",
            isSystem: true),

        new(
            key: "AttendanceCorrection",
            name: "Attendance Corrections",
            description: "Attendance Correction Management",
            isSystem: true),

        new(
            key: "AttendanceCorrectionApproval",
            name: "Attendance Correction Approvals",
            description: "Attendance Correction Approval Management",
            isSystem: true),

        new(
            key: "AttendanceDevice",
            name: "Attendance Devices",
            description: "Attendance Device Management",
            isSystem: true),

        new(
            key: "WorkSchedule",
            name: "Work Schedules",
            description: "Work Schedule Management",
            isSystem: true),

        new(
            key: "Holiday",
            name: "Holidays",
            description: "Holiday Management",
            isSystem: true),

        new(
            key: "Overtime",
            name: "Overtime",
            description: "Overtime Management",
            isSystem: true),

        new(
            key: "OvertimeApproval",
            name: "Overtime Approvals",
            description: "Overtime Approval Management",
            isSystem: true),

        new(
            key: "Payroll",
            name: "Payroll",
            description: "Payroll Management",
            isSystem: true),

        new(
            key: "PayrollApproval",
            name: "Payroll Approvals",
            description: "Payroll Approval Management",
            isSystem: true),

        new(
            key: "PayrollPayment",
            name: "Payroll Payments",
            description: "Payroll Payment Management",
            isSystem: true),

        new(
            key: "PayrollLoan",
            name: "Payroll Loans",
            description: "Payroll Loan Management",
            isSystem: true),

        new(
            key: "EmployeeEvaluation",
            name: "Employee Evaluations",
            description: "Employee Evaluation Management",
            isSystem: true),

        new(
            key: "EmployeeEvaluationApproval",
            name: "Employee Evaluation Approvals",
            description: "Employee Evaluation Approval Management",
            isSystem: true)
    ];

    public IEnumerable<PermissionDefinition> GetPermissions()
    {
        // =========================================================
        // Employee
        // =========================================================

        yield return new(
            "Employee",
            HRPermissions.Employee.View,
            "View Employees",
            "Allows viewing employees.");

        yield return new(
            "Employee",
            HRPermissions.Employee.Create,
            "Create Employee",
            "Allows creating employees.");

        yield return new(
            "Employee",
            HRPermissions.Employee.Update,
            "Update Employee",
            "Allows updating employees.");

        yield return new(
            "Employee",
            HRPermissions.Employee.Delete,
            "Delete Employee",
            "Allows deleting employees.");


        // =========================================================
        // Employee Department
        // =========================================================

        yield return new(
            "EmployeeDepartment",
            HRPermissions.EmployeeDepartment.Transfer,
            "Transfer Employee Department",
            "Allows transferring employees between departments.");


        // =========================================================
        // Employee Salary
        // =========================================================

        yield return new(
            "EmployeeSalary",
            HRPermissions.EmployeeSalary.Update,
            "Update Employee Salary",
            "Allows updating employee salaries.");


        // =========================================================
        // Department
        // =========================================================

        yield return new(
            "Department",
            HRPermissions.Department.View,
            "View Departments",
            "Allows viewing departments.");

        yield return new(
            "Department",
            HRPermissions.Department.Create,
            "Create Department",
            "Allows creating departments.");

        yield return new(
            "Department",
            HRPermissions.Department.Update,
            "Update Department",
            "Allows updating departments.");

        yield return new(
            "Department",
            HRPermissions.Department.Delete,
            "Delete Department",
            "Allows deleting departments.");


        // =========================================================
        // Department Manager
        // =========================================================

        yield return new(
            "DepartmentManager",
            HRPermissions.DepartmentManager.Assign,
            "Assign Department Manager",
            "Allows assigning department managers.");

        yield return new(
            "DepartmentManager",
            HRPermissions.DepartmentManager.Transfer,
            "Transfer Department Manager",
            "Allows transferring department managers.");


        // =========================================================
        // Employee Leave
        // =========================================================

        yield return new(
            "EmployeeLeave",
            HRPermissions.EmployeeLeave.View,
            "View Employee Leaves",
            "Allows viewing employee leaves.");

        yield return new(
            "EmployeeLeave",
            HRPermissions.EmployeeLeave.Create,
            "Create Employee Leave",
            "Allows creating employee leaves.");

        yield return new(
            "EmployeeLeave",
            HRPermissions.EmployeeLeave.Update,
            "Update Employee Leave",
            "Allows updating employee leaves.");

        yield return new(
            "EmployeeLeave",
            HRPermissions.EmployeeLeave.Delete,
            "Delete Employee Leave",
            "Allows deleting employee leaves.");

        yield return new(
            "EmployeeLeave",
            HRPermissions.EmployeeLeave.Cancel,
            "Cancel Employee Leave",
            "Allows cancelling employee leaves.");


        // =========================================================
        // Employee Leave Approval
        // =========================================================

        yield return new(
            "EmployeeLeaveApproval",
            HRPermissions.EmployeeLeaveApproval.Approve,
            "Approve Employee Leave",
            "Allows approving employee leaves.");

        yield return new(
            "EmployeeLeaveApproval",
            HRPermissions.EmployeeLeaveApproval.Reject,
            "Reject Employee Leave",
            "Allows rejecting employee leaves.");


        // =========================================================
        // Employee Leave Balance
        // =========================================================

        yield return new(
            "EmployeeLeaveBalance",
            HRPermissions.EmployeeLeaveBalance.View,
            "View Employee Leave Balance",
            "Allows viewing employee leave balances.");


        // =========================================================
        // Employee Special Leave
        // =========================================================

        yield return new(
            "EmployeeSpecialLeave",
            HRPermissions.EmployeeSpecialLeave.View,
            "View Employee Special Leaves",
            "Allows viewing employee special leaves.");

        yield return new(
            "EmployeeSpecialLeave",
            HRPermissions.EmployeeSpecialLeave.Create,
            "Create Employee Special Leave",
            "Allows creating employee special leaves.");

        yield return new(
            "EmployeeSpecialLeave",
            HRPermissions.EmployeeSpecialLeave.Cancel,
            "Cancel Employee Special Leave",
            "Allows cancelling employee special leaves.");


        // =========================================================
        // Employee Special Leave Approval
        // =========================================================

        yield return new(
            "EmployeeSpecialLeaveApproval",
            HRPermissions.EmployeeSpecialLeaveApproval.Approve,
            "Approve Employee Special Leave",
            "Allows approving employee special leaves.");

        yield return new(
            "EmployeeSpecialLeaveApproval",
            HRPermissions.EmployeeSpecialLeaveApproval.Reject,
            "Reject Employee Special Leave",
            "Allows rejecting employee special leaves.");


        // =========================================================
        // Attendance
        // =========================================================

        yield return new(
            "Attendance",
            HRPermissions.Attendance.View,
            "View Attendance",
            "Allows viewing attendance records.");

        yield return new(
            "Attendance",
            HRPermissions.Attendance.Create,
            "Create Attendance",
            "Allows creating attendance records.");

        yield return new(
            "Attendance",
            HRPermissions.Attendance.Update,
            "Update Attendance",
            "Allows updating attendance records.");

        yield return new(
            "Attendance",
            HRPermissions.Attendance.Export,
            "Export Attendance",
            "Allows exporting attendance records.");

        yield return new(
            "Attendance",
            HRPermissions.Attendance.CheckIn,
            "Check In",
            "Allows checking employees in.");

        yield return new(
            "Attendance",
            HRPermissions.Attendance.CheckOut,
            "Check Out",
            "Allows checking employees out.");


        // =========================================================
        // Attendance Correction
        // =========================================================

        yield return new(
            "AttendanceCorrection",
            HRPermissions.AttendanceCorrection.View,
            "View Attendance Corrections",
            "Allows viewing attendance corrections.");

        yield return new(
            "AttendanceCorrection",
            HRPermissions.AttendanceCorrection.Create,
            "Create Attendance Correction",
            "Allows creating attendance corrections.");


        // =========================================================
        // Attendance Correction Approval
        // =========================================================

        yield return new(
            "AttendanceCorrectionApproval",
            HRPermissions.AttendanceCorrectionApproval.Approve,
            "Approve Attendance Correction",
            "Allows approving attendance corrections.");

        yield return new(
            "AttendanceCorrectionApproval",
            HRPermissions.AttendanceCorrectionApproval.Reject,
            "Reject Attendance Correction",
            "Allows rejecting attendance corrections.");


        // =========================================================
        // Attendance Device
        // =========================================================

        yield return new(
            "AttendanceDevice",
            HRPermissions.AttendanceDevice.View,
            "View Attendance Devices",
            "Allows viewing attendance devices.");

        yield return new(
            "AttendanceDevice",
            HRPermissions.AttendanceDevice.Create,
            "Create Attendance Device",
            "Allows creating attendance devices.");

        yield return new(
            "AttendanceDevice",
            HRPermissions.AttendanceDevice.Update,
            "Update Attendance Device",
            "Allows updating attendance devices.");

        yield return new(
            "AttendanceDevice",
            HRPermissions.AttendanceDevice.Delete,
            "Delete Attendance Device",
            "Allows deleting attendance devices.");

        yield return new(
            "AttendanceDevice",
            HRPermissions.AttendanceDevice.ReceiveLogs,
            "Receive Attendance Logs",
            "Allows receiving attendance logs from devices.");


        // =========================================================
        // Work Schedule
        // =========================================================

        yield return new(
            "WorkSchedule",
            HRPermissions.WorkSchedule.View,
            "View Work Schedules",
            "Allows viewing work schedules.");

        yield return new(
            "WorkSchedule",
            HRPermissions.WorkSchedule.Create,
            "Create Work Schedule",
            "Allows creating work schedules.");

        yield return new(
            "WorkSchedule",
            HRPermissions.WorkSchedule.Update,
            "Update Work Schedule",
            "Allows updating work schedules.");

        yield return new(
            "WorkSchedule",
            HRPermissions.WorkSchedule.Delete,
            "Delete Work Schedule",
            "Allows deleting work schedules.");


        // =========================================================
        // Holiday
        // =========================================================

        yield return new(
            "Holiday",
            HRPermissions.Holiday.View,
            "View Holidays",
            "Allows viewing holidays.");

        yield return new(
            "Holiday",
            HRPermissions.Holiday.Create,
            "Create Holiday",
            "Allows creating holidays.");

        yield return new(
            "Holiday",
            HRPermissions.Holiday.Update,
            "Update Holiday",
            "Allows updating holidays.");

        yield return new(
            "Holiday",
            HRPermissions.Holiday.Delete,
            "Delete Holiday",
            "Allows deleting holidays.");


        // =========================================================
        // Overtime
        // =========================================================

        yield return new(
            "Overtime",
            HRPermissions.Overtime.View,
            "View Overtime",
            "Allows viewing overtime.");

        yield return new(
            "Overtime",
            HRPermissions.Overtime.Create,
            "Create Overtime",
            "Allows creating overtime.");

        yield return new(
            "Overtime",
            HRPermissions.Overtime.Update,
            "Update Overtime",
            "Allows updating overtime.");

        yield return new(
            "Overtime",
            HRPermissions.Overtime.Delete,
            "Delete Overtime",
            "Allows deleting overtime.");


        // =========================================================
        // Overtime Approval
        // =========================================================

        yield return new(
            "OvertimeApproval",
            HRPermissions.OvertimeApproval.Approve,
            "Approve Overtime",
            "Allows approving overtime.");

        yield return new(
            "OvertimeApproval",
            HRPermissions.OvertimeApproval.Reject,
            "Reject Overtime",
            "Allows rejecting overtime.");


        // =========================================================
        // Payroll
        // =========================================================

        yield return new(
            "Payroll",
            HRPermissions.Payroll.View,
            "View Payroll",
            "Allows viewing payroll.");

        yield return new(
            "Payroll",
            HRPermissions.Payroll.Create,
            "Create Payroll",
            "Allows creating payroll.");

        yield return new(
            "Payroll",
            HRPermissions.Payroll.Update,
            "Update Payroll",
            "Allows updating payroll.");

        yield return new(
            "Payroll",
            HRPermissions.Payroll.Delete,
            "Delete Payroll",
            "Allows deleting payroll.");


        // =========================================================
        // Payroll Approval
        // =========================================================

        yield return new(
            "PayrollApproval",
            HRPermissions.PayrollApproval.Approve,
            "Approve Payroll",
            "Allows approving payroll.");

        yield return new(
            "PayrollApproval",
            HRPermissions.PayrollApproval.Reject,
            "Reject Payroll",
            "Allows rejecting payroll.");


        // =========================================================
        // Payroll Payment
        // =========================================================

        yield return new(
            "PayrollPayment",
            HRPermissions.PayrollPayment.Pay,
            "Pay Payroll",
            "Allows paying payroll.");


        // =========================================================
        // Payroll Loan
        // =========================================================

        yield return new(
            "PayrollLoan",
            HRPermissions.PayrollLoan.View,
            "View Payroll Loans",
            "Allows viewing payroll loans.");

        yield return new(
            "PayrollLoan",
            HRPermissions.PayrollLoan.Create,
            "Create Payroll Loan",
            "Allows creating payroll loans.");

        yield return new(
            "PayrollLoan",
            HRPermissions.PayrollLoan.Update,
            "Update Payroll Loan",
            "Allows updating payroll loans.");

        yield return new(
            "PayrollLoan",
            HRPermissions.PayrollLoan.Cancel,
            "Cancel Payroll Loan",
            "Allows cancelling payroll loans.");

        yield return new(
            "PayrollLoan",
            HRPermissions.PayrollLoan.Suspend,
            "Suspend Payroll Loan",
            "Allows suspending payroll loans.");

        yield return new(
            "PayrollLoan",
            HRPermissions.PayrollLoan.Resume,
            "Resume Payroll Loan",
            "Allows resuming payroll loans.");


        // =========================================================
        // Employee Evaluation
        // =========================================================

        yield return new(
            "EmployeeEvaluation",
            HRPermissions.EmployeeEvaluation.View,
            "View Employee Evaluations",
            "Allows viewing employee evaluations.");

        yield return new(
            "EmployeeEvaluation",
            HRPermissions.EmployeeEvaluation.Create,
            "Create Employee Evaluation",
            "Allows creating employee evaluations.");

        yield return new(
            "EmployeeEvaluation",
            HRPermissions.EmployeeEvaluation.Update,
            "Update Employee Evaluation",
            "Allows updating employee evaluations.");

        yield return new(
            "EmployeeEvaluation",
            HRPermissions.EmployeeEvaluation.Delete,
            "Delete Employee Evaluation",
            "Allows deleting employee evaluations.");

        yield return new(
            "EmployeeEvaluation",
            HRPermissions.EmployeeEvaluation.Submit,
            "Submit Employee Evaluation",
            "Allows submitting employee evaluations.");

        yield return new(
            "EmployeeEvaluation",
            HRPermissions.EmployeeEvaluation.Export,
            "Export Employee Evaluations",
            "Allows exporting employee evaluations.");


        // =========================================================
        // Employee Evaluation Approval
        // =========================================================

        yield return new(
            "EmployeeEvaluationApproval",
            HRPermissions.EmployeeEvaluationApproval.Approve,
            "Approve Employee Evaluation",
            "Allows approving employee evaluations.");

        yield return new(
            "EmployeeEvaluationApproval",
            HRPermissions.EmployeeEvaluationApproval.Reject,
            "Reject Employee Evaluation",
            "Allows rejecting employee evaluations.");
    }
}