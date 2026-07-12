using MicroERP.Domin.Common;

namespace Domin.Entities
{
    public class Department : BaseEntity
    {
        public string Code { get; set; } = null!;

        public string NameAr { get; set; } = null!;

        public string? NameEn { get; set; }

        public int? ManagerEmployeeId { get; set; }
        public bool HasManager { get; set; }
        public Employee? ManagerEmployee { get; set; }

        public ICollection<Employee> Employees { get; set; }
            = new List<Employee>();
    }
}
