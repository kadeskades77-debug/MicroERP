using MicroERP.Domin.Common;

namespace Domin.Entities;

public class EmployeeDocument : BaseEntity
{
    public int EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public string StoredFileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public long FileSize { get; set; }

    public DateTime? IssueDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public string? Notes { get; set; }
}