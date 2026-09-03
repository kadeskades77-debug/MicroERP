using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Ticketing.DTOs;
using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Ticketing.Interfaces;

public interface ITicketService
{
    Task<Result<TicketDto>> CreateAsync(CreateTicketDto dto, CancellationToken ct = default);
    Task<Result<TicketDto>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Result<List<TicketDto>>> GetAllAsync(TicketFilterDto filter, CancellationToken ct = default);
    Task<Result<TicketDto>> UpdateAsync(int id, UpdateTicketDto dto, CancellationToken ct = default);
    Task<Result<TicketDto>> ChangeStatusAsync(ChangeTicketStatusDto dto,CancellationToken ct = default);
    Task<Result<TicketDto>> ChangePriorityAsync(ChangeTicketPriorityDto dto,CancellationToken ct = default);
    Task<Result> DeleteAsync(int id, CancellationToken ct = default);
    Task<Result<TicketDto>> StartAsync(TicketActionDto dto,CancellationToken ct = default);
    Task<Result<TicketDto>> ResolveAsync(TicketActionDto dto,CancellationToken ct = default);
    Task<Result<TicketDto>> CloseAsync(TicketActionDto dto,CancellationToken ct = default);
    Task<Result<TicketDto>> ReopenAsync(TicketActionDto dto,CancellationToken ct = default);
}
