using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Ticketing.DTOs;

namespace MicroERP.Application.Features.Ticketing.Interfaces;

public interface ITicketHistoryService
{
    Task<Result<List<TicketHistoryDto>>> GetByTicketIdAsync(int ticketId, CancellationToken ct = default);
}
