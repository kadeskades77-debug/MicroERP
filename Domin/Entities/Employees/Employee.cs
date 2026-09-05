using MicroERP.Domin.Common;
using MicroERP.Domin.Entities.EmployeeAttendance;
using MicroERP.Domin.Entities.EmployeeLeaves;
using MicroERP.Domin.Entities.Payrolls;
using MicroERP.Domin.Enums;
using MicroERP.Domin.Identity;
namespace MicroERP.Domin.Entities.Employees
{
    public class Employee : BaseEntity
    {
        public string Phone { get; set; } = null!;
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;


        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;

        public int? WorkScheduleId { get; set; }
        public WorkSchedule? WorkSchedule { get; set; }

        public ICollection<EmployeeDocument> Documents { get; set; } = new List<EmployeeDocument>();
        public int? PositionId { get; set; }
        public Position? Position { get; set; }
        public ICollection<EmployeeLeave> EmployeeLeaves { get; set; } = [];
        public ICollection<EmployeeLeaveBalance> LeaveBalances { get; set; } = [];
        public ICollection<AttendanceRecord> AttendanceRecords { get; set; }= new List<AttendanceRecord>();
        public ICollection<EmployeeAttendanceDevice> AttendanceDevices { get; set; }= new List<EmployeeAttendanceDevice>();
        public ICollection<EmployeeBankAccount> BankAccounts { get; set; }= new List<EmployeeBankAccount>();

    }
 
}
