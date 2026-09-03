using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Ticketing.DTOs;

namespace MicroERP.Application.Features.Ticketing.Interfaces;

public interface ITicketAssignmentService
{
    Task<Result<TicketDto>> AssignAsync(AssignTicketDto dto,CancellationToken ct = default);
    Task<Result<TicketDto>> UnassignAsync(UnassignTicketDto dto,CancellationToken ct = default);
    Task<Result<List<TicketAssignmentDto>>>
            GetHistoryAsync(int ticketId,CancellationToken ct = default);

}
