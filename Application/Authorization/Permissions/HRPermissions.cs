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
}