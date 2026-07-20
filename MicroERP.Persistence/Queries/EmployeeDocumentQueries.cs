using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Features.Documents.EmployeeDocuments.DTOs;
using MicroERP.Application.Features.Documents.EmployeeDocuments.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Infrastructure.Services.EmployeeDocuments;

public class EmployeeDocumentQueries : IEmployeeDocumentQueries
{
    private readonly IApplicationDbContext _context;

    public EmployeeDocumentQueries(
        IApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<List<EmployeeDocumentDto>> GetByEmployeeIdAsync(int employeeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.EmployeeDocuments
            .AsNoTracking()
            .Where(x => x.EmployeeId == employeeId)
            .Select(x => new EmployeeDocumentDto
            {
                Id = x.Id,

                EmployeeId = x.EmployeeId,

                Name = x.Name,

                FileName = x.FileName,

                ContentType = x.ContentType,

                FileSize = x.FileSize,

                FilePath = x.FilePath,

                IssueDate = x.IssueDate,

                ExpiryDate = x.ExpiryDate,

                Notes = x.Notes
            })
            .ToListAsync(cancellationToken);
    }


    public async Task<EmployeeDocumentDto?> GetByIdAsync(int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.EmployeeDocuments
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new EmployeeDocumentDto
            {
                Id = x.Id,

                EmployeeId = x.EmployeeId,

                Name = x.Name,

                FileName = x.FileName,

                ContentType = x.ContentType,

                FileSize = x.FileSize,

                FilePath = x.FilePath,

                IssueDate = x.IssueDate,

                ExpiryDate = x.ExpiryDate,

                Notes = x.Notes
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}