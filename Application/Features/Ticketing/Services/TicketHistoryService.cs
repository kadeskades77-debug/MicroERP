using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Ticketing.DTOs;
using MicroERP.Application.Features.Ticketing.Interfaces;
using Microsoft.EntityFrameworkCore;
using static MicroERP.Application.Authorization.Permissions.TicketPermissions;
namespace MicroERP.Application.Features.Ticketing.Services;
public class TicketHistoryService : ITicketHistoryService
{
    private readonly IApplicationDbContext _context;

    public TicketHistoryService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<TicketHistoryDto>>> GetByTicketIdAsync(
    int ticketId,
    CancellationToken ct = default)
    {
        // =====================================================
        // Validate Ticket ID
        // =====================================================

        if (ticketId <= 0)
        {
            return Result<List<TicketHistoryDto>>.Failure(
                "Invalid ticket ID.");
        }

        // =====================================================
        // Validate Ticket
        // =====================================================

        var ticketExists =
            await _context.Tickets
                .AsNoTracking()
                .AnyAsync(
                    x =>
                        x.Id == ticketId &&
                        x.IsActive,
                    ct);

        if (!ticketExists)
        {
            return Result<List<TicketHistoryDto>>.Failure(
                "Ticket not found or inactive.");
        }

        // =====================================================
        // Get History
        // =====================================================

        var histories =
            await _context.TicketHistories
                .AsNoTracking()
                .Where(
                    x =>
                        x.TicketId == ticketId)
                .OrderByDescending(
                    x => x.CreatedOn)
                .Select(
                    x => new TicketHistoryDto
                    {
                        Id = x.Id,
                        TicketId = x.TicketId,
                        Action = x.Action.ToString(),
                        OldValue = x.OldValue,
                        NewValue = x.NewValue,
                        Notes = x.Notes,

                        PerformedBy =
                            _context.Users
                                .Where(
                                    u =>
                                        u.Id == x.CreatedBy)
                                .Select(
                                    u =>
                                        u.FullName)
                                .FirstOrDefault(),

                        CreatedOn = x.CreatedOn
                    })
                .ToListAsync(ct);

        // =====================================================
        // Result
        // =====================================================

        return Result<List<TicketHistoryDto>>.Succeeded(
            histories,
            "Ticket history retrieved successfully.");
    }
}
