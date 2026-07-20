using Microsoft.AspNetCore.Http;

namespace MicroERP.Application.Features.Documents.EmployeeDocuments.DTOs;

public class UpdateEmployeeDocumentDto
{
    public string? Name { get; set; }

    public IFormFile? File { get; set; }

    public DateTime? IssueDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public string? Notes { get; set; }
}