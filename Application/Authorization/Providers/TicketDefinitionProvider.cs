using MicroERP.Application.Authorization.Interfaces;

namespace MicroERP.Application.Authorization.Providers;

public class TicketDefinitionProvider
    : IMultiPermissionDefinitionProvider
{
    public IEnumerable<PermissionGroupDefinition> Groups =>
[
    new PermissionGroupDefinition(
        "Ticket",
        "Tickets"),

    new PermissionGroupDefinition(
        "TicketCategory",
        "Ticket Categories"),

    new PermissionGroupDefinition(
        "TicketAssignment",
        "Ticket Assignments"),

    new PermissionGroupDefinition(
        "TicketComment",
        "Ticket Comments"),

    new PermissionGroupDefinition(
        "TicketAttachment",
        "Ticket Attachments"),

    new PermissionGroupDefinition(
        "TicketHistory",
        "Ticket History")
];

    public IEnumerable<PermissionDefinition> GetPermissions()
    {
        // =========================================================
        // Ticket
        // =========================================================

        yield return new PermissionDefinition(
            "Ticket",
            "Ticket.View",
            "View Tickets");

        yield return new PermissionDefinition(
            "Ticket",
            "Ticket.Create",
            "Create Ticket");

        yield return new PermissionDefinition(
            "Ticket",
            "Ticket.Update",
            "Update Ticket");

        yield return new PermissionDefinition(
            "Ticket",
            "Ticket.Delete",
            "Delete Ticket");

        yield return new PermissionDefinition(
            "Ticket",
            "Ticket.ChangeStatus",
            "Change Ticket Status");

        yield return new PermissionDefinition(
            "Ticket",
            "Ticket.ChangePriority",
            "Change Ticket Priority");

        yield return new PermissionDefinition(
            "Ticket",
            "Ticket.Resolve",
            "Resolve Ticket");

        yield return new PermissionDefinition(
            "Ticket",
            "Ticket.Close",
            "Close Ticket");

        yield return new PermissionDefinition(
            "Ticket",
            "Ticket.Reopen",
            "Reopen Ticket");

        // =========================================================
        // Ticket Category
        // =========================================================

        yield return new PermissionDefinition(
            "TicketCategory",
            "TicketCategory.View",
            "View Ticket Categories");

        yield return new PermissionDefinition(
            "TicketCategory",
            "TicketCategory.Create",
            "Create Ticket Category");

        yield return new PermissionDefinition(
            "TicketCategory",
            "TicketCategory.Update",
            "Update Ticket Category");

        yield return new PermissionDefinition(
            "TicketCategory",
            "TicketCategory.Delete",
            "Delete Ticket Category");



        // =========================================================
        // Assignment
        // =========================================================

        yield return new PermissionDefinition(
            "TicketAssignment",
            "TicketAssignment.View",
            "View Assignments");

        yield return new PermissionDefinition(
            "TicketAssignment",
            "TicketAssignment.Assign",
            "Assign Ticket");

        yield return new PermissionDefinition(
            "TicketAssignment",
            "TicketAssignment.Unassign",
            "Unassign Ticket");

        // =========================================================
        // Comment
        // =========================================================

        yield return new PermissionDefinition(
            "TicketComment",
            "TicketComment.View",
            "View Comments");

        yield return new PermissionDefinition(
            "TicketComment",
            "TicketComment.Create",
            "Create Comment");

        yield return new PermissionDefinition(
            "TicketComment",
            "TicketComment.Update",
            "Update Comment");

        yield return new PermissionDefinition(
            "TicketComment",
            "TicketComment.Delete",
            "Delete Comment");

        // =========================================================
        // Attachment
        // =========================================================

        yield return new PermissionDefinition(
            "TicketAttachment",
            "TicketAttachment.View",
            "View Attachments");

        yield return new PermissionDefinition(
            "TicketAttachment",
            "TicketAttachment.Upload",
            "Upload Attachment");

        yield return new PermissionDefinition(
            "TicketAttachment",
            "TicketAttachment.Download",
            "Download Attachment");

        yield return new PermissionDefinition(
            "TicketAttachment",
            "TicketAttachment.Delete",
            "Delete Attachment");

        // =========================================================
        // History
        // =========================================================

        yield return new PermissionDefinition(
            "TicketHistory",
            "TicketHistory.View",
            "View Ticket History");
    }
}