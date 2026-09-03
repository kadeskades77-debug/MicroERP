namespace MicroERP.Application.Common.Files;

public class FileValidationSettings
{
    public FileValidationOptions EmployeeDocuments { get; set; } = new();

    public FileValidationOptions LeaveAttachments { get; set; } = new();

    public FileValidationOptions TicketAttachments { get; set; } = new();
}