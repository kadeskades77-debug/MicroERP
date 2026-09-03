

namespace MicroERP.Application.Authorization.Permissions
{
    public static class TicketPermissions
    {
        // =========================================================
        // Tickets
        // =========================================================

        public static class Ticket
        {
            public const string View = "Ticket.View";
            public const string Create = "Ticket.Create";
            public const string Update = "Ticket.Update";
            public const string Delete = "Ticket.Delete";

            public const string ChangeStatus = "Ticket.ChangeStatus";
            public const string ChangePriority = "Ticket.ChangePriority";

            public const string Assign = "Ticket.Assign";
            public const string Unassign = "Ticket.Unassign";

            public const string Resolve = "Ticket.Resolve";
            public const string Close = "Ticket.Close";
            public const string Reopen = "Ticket.Reopen";
        }

        // =========================================================
        // Categories
        // =========================================================

        public static class Category
        {
            public const string View = "TicketCategory.View";
            public const string Create = "TicketCategory.Create";
            public const string Update = "TicketCategory.Update";
            public const string Delete = "TicketCategory.Delete";
        }

        // =========================================================
        // Assignments
        // =========================================================

        public static class Assignment
        {
            public const string View = "TicketAssignment.View";
            public const string Assign = "TicketAssignment.Assign";
            public const string Unassign = "TicketAssignment.Unassign";
        }

        // =========================================================
        // Comments
        // =========================================================

        public static class Comment
        {
            public const string View = "TicketComment.View";
            public const string Create = "TicketComment.Create";
            public const string Update = "TicketComment.Update";
            public const string Delete = "TicketComment.Delete";
        }

        // =========================================================
        // Attachments
        // =========================================================

        public static class Attachment
        {
            public const string View = "TicketAttachment.View";
            public const string Upload = "TicketAttachment.Upload";
            public const string Download = "TicketAttachment.Download";
            public const string Delete = "TicketAttachment.Delete";
        }

        // =========================================================
        // History
        // =========================================================

        public static class History
        {
            public const string View = "TicketHistory.View";
        }
    }
}
