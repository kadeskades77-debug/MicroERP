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
}