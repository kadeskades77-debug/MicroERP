using MicroERP.Application.Features.Ticketing.DTOs;
using MicroERP.Domin.Entities.Ticketing;

namespace MicroERP.Application.Features.Ticketing.Mappings;

public static class TicketAttachmentMapping
{
    public static TicketAttachmentDto ToDto(
        this TicketAttachment attachment)
    {
        return new TicketAttachmentDto
        {
            // =====================================================
            // Basic Information
            // =====================================================

            Id =
                attachment.Id,

            TicketId =
                attachment.TicketId,


            // =====================================================
            // File Information
            // =====================================================

            FileName =
                attachment.FileName,

            FilePath =
                attachment.FilePath,

            ContentType =
                attachment.ContentType,

            FileSize =
                attachment.FileSize,


            // =====================================================
            // Date
            // =====================================================

            CreatedOn =
                attachment.CreatedOn
        };
    }
}