
using MicroERP.Domin.Common;
using MicroERP.Domin.Enums;

namespace MicroERP.Domin.Entities.Employees
{
    public class Position : BaseEntity
    {
        public string NameAr { get; set; } = null!;
        public string NameEn { get; set; } = null!;
        public string Code { get; set; } = null!;
        public PositionAssignmentType AssignmentType { get; set; }
    = PositionAssignmentType.Multiple;
        public string? Description { get; set; }

        public ICollection<Employee> Employees { get; set; }
            = new List<Employee>();
    }
}
