using Microsoft.AspNetCore.Http;

namespace MicroERP.Application.Features.Documents.LeaveDocuments.DTOS
{
    public class UploadLeaveAttachmentDto
    {
        public IFormFile File { get; set; } = null!;
    }
}
