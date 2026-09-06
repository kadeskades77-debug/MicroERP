public static class HRPermissions
{
   
        // =========================================================
        // Employee
        // =========================================================

        public static class Employee
        {
            public const string View = "HR.Employee.View";
            public const string Create = "HR.Employee.Create";
            public const string Update = "HR.Employee.Update";
            public const string Delete = "HR.Employee.Delete";
        }


        // =========================================================
        // Employee Department
        // =========================================================

        public static class EmployeeDepartment
        {
            public const string Transfer =
                "HR.EmployeeDepartment.Transfer";
        }


        // =========================================================
        // Employee Salary
        // =========================================================

        public static class EmployeeSalary
        {
            public const string Update =
                "HR.EmployeeSalary.Update";
        }


        // =========================================================
        // Department
        // =========================================================

        public static class Department
        {
            public const string View = "HR.Department.View";
            public const string Create = "HR.Department.Create";
            public const string Update = "HR.Department.Update";
            public const string Delete = "HR.Department.Delete";
        }


        // =========================================================
        // Department Manager
        // =========================================================

        public static class DepartmentManager
        {
            public const string Assign =
                "HR.DepartmentManager.Assign";

            public const string Transfer =
                "HR.DepartmentManager.Transfer";
        }


    public static class EmployeeLeave
    {
        public const string View = "HR.EmployeeLeave.View";
        public const string Create = "HR.EmployeeLeave.Create";
        public const string Update = "HR.EmployeeLeave.Update";
        public const string Delete = "HR.EmployeeLeave.Delete";
        public const string Cancel = "HR.EmployeeLeave.Cancel";
    }

    public static class EmployeeLeaveApproval
    {
        public const string Approve = "HR.EmployeeLeaveApproval.Approve";
        public const string Reject = "HR.EmployeeLeaveApproval.Reject";
    }
    public static class EmployeeLeaveBalance
    {
        public const string View = "HR.EmployeeLeaveBalance.View";
    }
    public static class EmployeeSpecialLeave
    {
        public const string View = "HR.EmployeeSpecialLeave.View";
        public const string Create = "HR.EmployeeSpecialLeave.Create";
        public const string Cancel = "HR.EmployeeSpecialLeave.Cancel";
    }

    public static class EmployeeSpecialLeaveApproval
    {
        public const string Approve = "HR.EmployeeSpecialLeaveApproval.Approve";
        public const string Reject = "HR.EmployeeSpecialLeaveApproval.Reject";
    }
    public static class Attendance
    {
        public const string View = "HR.Attendance.View";
        public const string Create = "HR.Attendance.Create";
        public const string Update = "HR.Attendance.Update";
        public const string Export = "HR.Attendance.Export";
        public const string CheckIn = "HR.Attendance.CheckIn";
        public const string CheckOut = "HR.Attendance.CheckOut";
    }
    public static class AttendanceCorrection
    {
        public const string View = "HR.AttendanceCorrection.View";
        public const string Create = "HR.AttendanceCorrection.Create";
    }
    public static class AttendanceCorrectionApproval
    {
        public const string Approve = "HR.AttendanceCorrectionApproval.Approve";
        public const string Reject = "HR.AttendanceCorrectionApproval.Reject";
    }
    public static class WorkSchedule
    {
        public const string View = "HR.WorkSchedule.View";
        public const string Create = "HR.WorkSchedule.Create";
        public const string Update = "HR.WorkSchedule.Update";
        public const string Delete = "HR.WorkSchedule.Delete";
    }
    public static class AttendanceDevice
    {
        public const string View = "HR.Attendance.Device.View";
        public const string Create = "HR.Attendance.Device.Create";
        public const string Update = "HR.Attendance.Device.Update";
        public const string Delete = "HR.Attendance.Device.Delete";
        public const string ReceiveLogs ="HR.Attendance.Device.ReceiveLogs"; // استقبال سجلات البصمة من الجهاز
    }
    public static class Holiday
    {
        public const string View = "HR.Holiday.View";
        public const string Create = "HR.Holiday.Create";
        public const string Update = "HR.Holiday.Update";
        public const string Delete = "HR.Holiday.Delete";
    }
    public static class Payroll
    {
        public const string View = "HR.Payroll.View";
        public const string Create = "HR.Payroll.Create";
        public const string Update = "HR.Payroll.Update";
        public const string Delete = "HR.Payroll.Delete";
    }
    public static class PayrollApproval
    {
        public const string Approve = "HR.PayrollApproval.Approve";
        public const string Reject = "HR.PayrollApproval.Reject";
    }
    public static class PayrollPayment
    {
        public const string Pay = "HR.PayrollPayment.Pay";
    }
    public static class PayrollLoan
    {
        public const string View = "HR.PayrollLoan.View";
        public const string Create = "HR.PayrollLoan.Create";
        public const string Update = "HR.PayrollLoan.Update";
        public const string Cancel = "HR.PayrollLoan.Cancel";
        public const string Suspend = "HR.PayrollLoan.Suspend";
        public const string Resume = "HR.PayrollLoan.Resume";
    }
    public static class Overtime
    {
        public const string View = "HR.Overtime.View";
        public const string Create = "HR.Overtime.Create";
        public const string Update = "HR.Overtime.Update";
        public const string Delete = "HR.Overtime.Delete";
    }
    public static class OvertimeApproval
    {
        public const string Approve = "HR.OvertimeApproval.Approve";
        public const string Reject = "HR.OvertimeApproval.Reject";
    }
    public static class EmployeeEvaluation
    {
        public const string View = "HR.EmployeeEvaluation.View";
        public const string Create = "HR.EmployeeEvaluation.Create";
        public const string Update = "HR.EmployeeEvaluation.Update";
        public const string Delete = "HR.EmployeeEvaluation.Delete";
        public const string Submit = "HR.EmployeeEvaluation.Submit";
        public const string Export = "HR.EmployeeEvaluation.Export";
    }
    public static class EmployeeEvaluationApproval
    {
        public const string Approve = "HR.EmployeeEvaluationApproval.Approve";
        public const string Reject = "HR.EmployeeEvaluationApproval.Reject";
    }
}