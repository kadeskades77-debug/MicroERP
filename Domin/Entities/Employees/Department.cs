using MicroERP.Domin.Common;

namespace MicroERP.Domin.Entities.Employees
{
    public class Department : BaseEntity
    {
        public string Code { get; set; } = null!;

        public string NameAr { get; set; } = null!;

        public string NameEn { get; set; } = null!;

        public int? ManagerEmployeeId { get; set; }
        public bool HasManager { get; set; }
        public Employee? ManagerEmployee { get; set; }

        public ICollection<Employee> Employees { get; set; }
            = new List<Employee>();
    }
}
