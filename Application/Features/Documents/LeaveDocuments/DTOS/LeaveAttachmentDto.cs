using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Documents.LeaveDocuments.DTOS
{
    public class LeaveAttachmentDto
    {
        public int Id { get; set; }

        public string FileName { get; set; } = null!;

        public string FilePath { get; set; } = null!;

        public int? EmployeeLeaveId { get; set; }

        public int? EmployeeSpecialLeaveId { get; set; }

        public LeaveCategory Category { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
