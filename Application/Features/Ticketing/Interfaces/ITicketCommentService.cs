using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Ticketing.DTOs;

namespace MicroERP.Application.Features.Ticketing.Interfaces;


public interface ITicketCommentService
{
    Task<Result<TicketCommentDto>> CreateAsync(
        AddTicketCommentDto dto,
        CancellationToken ct = default);

    Task<Result<TicketCommentDto>> UpdateAsync(
        int id,
        UpdateTicketCommentDto dto,
        CancellationToken ct = default);

    Task<Result> DeleteAsync(
        int id,
        CancellationToken ct = default);

    Task<Result<List<TicketCommentDto>>> GetByTicketIdAsync(
        int ticketId,
        CancellationToken ct = default);
}

