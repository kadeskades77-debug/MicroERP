using MicroERP.Domin.Common;
using MicroERP.Domin.Enums;

namespace MicroERP.Domin.Entities.EmployeeLeaves
{
    public class LeaveAttachment : BaseEntity
    {
        public string FileName { get; set; } = null!;

        public string FilePath { get; set; } = null!;


        public int? EmployeeLeaveId { get; set; }

        public EmployeeLeave? EmployeeLeave { get; set; }


        public int? EmployeeSpecialLeaveId { get; set; }

        public EmployeeSpecialLeave? EmployeeSpecialLeave { get; set; }
    }
}
