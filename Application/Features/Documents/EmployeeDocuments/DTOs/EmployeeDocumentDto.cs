namespace MicroERP.Application.Features.Documents.EmployeeDocuments.DTOs;

public class EmployeeDocumentDto
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public string Name { get; set; } = null!; 

    public string FileName { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public long FileSize { get; set; }

    public string FilePath { get; set; } = null!;

    public DateTime? IssueDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public string? Notes { get; set; }
}