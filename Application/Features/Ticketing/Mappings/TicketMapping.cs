using MicroERP.Application.Features.Ticketing.DTOs;
using MicroERP.Domin.Entities.Ticketing;

namespace MicroERP.Application.Features.Ticketing.Mappings;

public static class TicketMapping
{
    public static TicketDto ToDto(
        this Ticket ticket,
        string? createdByName = null)
    {
        return new TicketDto
        {
            Id =
                ticket.Id,

            TicketNumber =
                ticket.TicketNumber,

            Title =
                ticket.Title,

            Description =
                ticket.Description,

            Status =
                ticket.Status,

            Priority =
                ticket.Priority,

            CategoryId =
                ticket.CategoryId,

            CategoryName =
                ticket.Category?.Name
                ?? string.Empty,

            AssignedToEmployeeId =
                ticket.AssignedToEmployeeId,

            AssignedToEmployeeName =
                ticket.AssignedToEmployee?.User?.FullName,

            AssignedDepartmentId =
                ticket.AssignedDepartmentId,

            AssignedDepartmentName =
                ticket.AssignedDepartment?.NameEn,

            DueOn =
                ticket.DueOn,

            ResolvedOn =
                ticket.ResolvedOn,

            ClosedOn =
                ticket.ClosedOn,

            CreatedByName =
                createdByName,

            CreatedOn =
                ticket.CreatedOn
        };
    }
}