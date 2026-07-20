using Microsoft.AspNetCore.Http;

namespace MicroERP.Application.Features.Documents.EmployeeDocuments.DTOs;

public class CreateEmployeeDocumentDto
{
    public string Name { get; set; } = null!;

    public IFormFile File { get; set; } = null!;

    public DateTime? IssueDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public string? Notes { get; set; }
}