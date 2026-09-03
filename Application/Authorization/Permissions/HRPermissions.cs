public static class HRPermissions
{
    public static class Employee
    {
        public const string View = "HR.Employee.View";
        public const string Create = "HR.Employee.Create";
        public const string TransferDepartment = "HR.Employee.TransferDepartment";
        public const string UpdateSalary = "HR.Employee.UpdateSalary";
        public const string Update = "HR.Employee.Update";
        public const string Delete = "HR.Employee.Delete";
    }

    public static class Department
    {
        public const string View = "HR.Department.View";
        public const string Create = "HR.Department.Create";
        public const string Update = "HR.Department.Update";
        public const string AssignManager = "HR.Department.AssignManager";
        public const string Delete = "HR.Department.Delete";
    }

    public static class EmployeeLeave
    {
        public const string View = "Employees.Leaves.View";
        public const string Create = "Employees.Leaves.Create";
        public const string Update = "Employees.Leaves.Update";
        public const string Delete = "Employees.Leaves.Delete";
        public const string Approve = "Employees.Leaves.Approve";
        public const string Reject = "Employees.Leaves.Reject";
        public const string Cancel = "Employees.Leaves.Cancel";
        public const string ViewBalance = "Employees.Leaves.ViewBalance";
    }
    public static class EmployeeSpecialLeave
    {
        public const string View = "Employees.SpecialLeaves.View";
        public const string Create = "Employees.SpecialLeaves.Create";
        public const string Approve = "Employees.SpecialLeaves.Approve";
        public const string Reject = "Employees.SpecialLeaves.Reject";
        public const string Cancel = "Employees.SpecialLeaves.Cancel";
    }
    public static class Attendance
    {
        public const string View = "HR.Attendance.View";
        public const string Create = "HR.Attendance.Create";
        public const string Update = "HR.Attendance.Update";
        public const string Export = "HR.Attendance.Export";
        public const string CheckIn = "HR.Attendance.CheckIn";
        public const string CheckOut = "HR.Attendance.CheckOut";
        public const string Approve = "HR.Attendance.Approve";
        public const string Reject = "HR.Attendance.Reject";

    }
    public static class AttendanceCorrection
    {
        public const string Create ="HR.AttendanceCorrection.Create";
        public const string Approve ="HR.AttendanceCorrection.Approve";
        public const string Reject ="HR.AttendanceCorrection.Reject";
        public const string View ="HR.AttendanceCorrection.View";
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
    public static class Holidays
    {
        public const string View = "HR.Holidays.View";
        public const string Create = "HR.Holidays.Create";
        public const string Update = "HR.Holidays.Update";
        public const string Delete = "HR.Holidays.Delete";
    }
    public static class Payroll
    {
        public const string View = "HR.Payroll.View";
        public const string Create = "HR.Payroll.Create";
        public const string Update = "HR.Payroll.Update";
        public const string Delete = "HR.Payroll.Delete";
        public const string Approve = "HR.Payroll.Approve";
        public const string Pay = "HR.Payroll.Pay";
    }
    public static class PayrollLoans
    {
        public const string View = "HR.PayrollLoans.View";
        public const string Create = "HR.PayrollLoans.Create";
        public const string Update = "HR.PayrollLoans.Update";
        public const string Cancel = "HR.PayrollLoans.Cancel";
        public const string Suspend = "HR.PayrollLoans.Suspend";
        public const string Resume = "HR.PayrollLoans.Resume";
    }
    public static class Overtime
    {
        public const string View = "HR.Overtime.View";
        public const string Create = "HR.Overtime.Create";
        public const string Update = "HR.Overtime.Update";
        public const string Delete = "HR.Overtime.Delete";
        public const string Approve = "HR.Overtime.Approve";
        public const string Reject = "HR.Overtime.Reject";
    }
    public static class EmployeeEvaluations
    {
        public const string View = "HR.EmployeeEvaluations.View";
        public const string Create = "HR.EmployeeEvaluations.Create";
        public const string Update = "HR.EmployeeEvaluations.Update";
        public const string Delete = "HR.EmployeeEvaluations.Delete";
        public const string Submit = "HR.EmployeeEvaluations.Submit";
        public const string Approve = "HR.EmployeeEvaluations.Approve";
        public const string Reject = "HR.EmployeeEvaluations.Reject";
        public const string Export = "HR.EmployeeEvaluations.Export";
    }
}