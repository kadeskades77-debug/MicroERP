using MicroERP.Application.Features.Ticketing.DTOs;
using MicroERP.Domin.Entities.Ticketing;

namespace MicroERP.Application.Features.Ticketing.Mappings;

public static class TicketHistoryMapping
{
    public static TicketHistoryDto ToDto(
        this TicketHistory history,
        string? performedByName = null)
    {
        return new TicketHistoryDto
        {
            // =====================================================
            // Basic Information
            // =====================================================

            Id =
                history.Id,

            TicketId =
                history.TicketId,

            // =====================================================
            // History
            // =====================================================

            Action =
                history.Action.ToString(),

            OldValue =
                history.OldValue,

            NewValue =
                history.NewValue,

            Notes =
                history.Notes,

            PerformedBy =
                performedByName,

            // =====================================================
            // Date
            // =====================================================

            CreatedOn =
                history.CreatedOn
        };
    }
}