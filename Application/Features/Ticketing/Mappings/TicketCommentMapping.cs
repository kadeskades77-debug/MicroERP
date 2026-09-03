using MicroERP.Application.Features.Ticketing.DTOs;
using MicroERP.Domin.Entities.Ticketing;

namespace MicroERP.Application.Features.Ticketing.Mappings;

public static class TicketCommentMapping
{
    public static TicketCommentDto ToDto(
        this TicketComment comment)
    {
        return new TicketCommentDto
        {
            // =====================================================
            // Basic Information
            // =====================================================

            Id =
                comment.Id,

            TicketId =
                comment.TicketId,


            // =====================================================
            // Employee
            // =====================================================

            EmployeeId =
                comment.EmployeeId,

            EmployeeName =
                comment.Employee?.User.FullName
                ?? string.Empty,


            // =====================================================
            // Comment
            // =====================================================

            Comment =
                comment.Comment,


            // =====================================================
            // Dates
            // =====================================================

            CreatedOn =
                comment.CreatedOn,

            ModifiedOn =
                comment.ModifiedOn
        };
    }
}