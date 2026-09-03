using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Ticketing.DTOs;

namespace MicroERP.Application.Features.Ticketing.Interfaces;

public interface ITicketAttachmentService
{
    Task<Result<TicketAttachmentDto>> UploadAsync(AddTicketAttachmentDto dto, CancellationToken ct = default);
    Task<Result<List<TicketAttachmentDto>>> GetByTicketIdAsync(int ticketId, CancellationToken ct = default);
    Task<Result> DeleteAsync(int id, CancellationToken ct = default);
}
