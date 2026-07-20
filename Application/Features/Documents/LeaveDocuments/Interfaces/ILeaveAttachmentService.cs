using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Documents.LeaveDocuments.DTOS;
using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Documents.LeaveDocuments.Interfaces;

public interface ILeaveAttachmentService
{
    Task<Result<LeaveAttachmentDto>> UploadEmployeeLeaveAsync(
        int leaveId,
        UploadLeaveAttachmentDto dto,
        CancellationToken cancellationToken = default);


    Task<Result<LeaveAttachmentDto>> UploadEmployeeSpecialLeaveAsync(
        int leaveId,
        UploadLeaveAttachmentDto dto,
        CancellationToken cancellationToken = default);

    Task<Result<List<LeaveAttachmentDto>>> GetByLeaveAsync(
      int leaveId,
      LeaveCategory category,
      CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(
        int attachmentId,
        CancellationToken cancellationToken = default);
}