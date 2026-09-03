using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Ticketing.DTOs;

namespace MicroERP.Application.Features.Ticketing.Interfaces;

public interface ITicketCategoryService
{
    Task<Result<TicketCategoryDto>> CreateAsync(CreateTicketCategoryDto dto, CancellationToken ct = default);
    Task<Result<TicketCategoryDto>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Result<List<TicketCategoryDto>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<TicketCategoryDto>> UpdateAsync(int id, UpdateTicketCategoryDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(int id, CancellationToken ct = default);
}
