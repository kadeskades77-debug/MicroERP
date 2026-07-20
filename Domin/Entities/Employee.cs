using MicroERP.Domin.Common;
using MicroERP.Domin.Entities;
using MicroERP.Domin.Enums;
namespace Domin.Entities
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

        public ICollection<EmployeeDocument> Documents { get; set; } = new List<EmployeeDocument>();
        public ICollection<EmployeeLeave> EmployeeLeaves { get; set; } = [];
        public ICollection<EmployeeLeaveBalance> LeaveBalances { get; set; } = [];
    }
 
}
