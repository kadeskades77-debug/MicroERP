using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Documents.EmployeeDocuments.DTOs;
using MicroERP.Application.Features.Documents.EmployeeDocuments.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Persistence.Queries;

public class EmployeeDocumentQueries : IEmployeeDocumentQueries
{
    private readonly IApplicationDbContext _context;

    public EmployeeDocumentQueries(
        IApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<Result<List<EmployeeDocumentDto>>> GetByEmployeeIdAsync(
    int employeeId,
    CancellationToken cancellationToken = default)
    {
        if (employeeId <= 0)
        {
            return Result<List<EmployeeDocumentDto>>.Failure(
                "Invalid employee ID.");
        }

        try
        {
            var documents =
                await _context.EmployeeDocuments
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

            return Result<List<EmployeeDocumentDto>>.Succeeded(
                documents);
        }
        catch
        {
            return Result<List<EmployeeDocumentDto>>.Failure(
                "An unexpected error occurred.");
        }
    }


    public async Task<Result<EmployeeDocumentDto>> GetByIdAsync(
     int employeeId,
     int id,
     CancellationToken cancellationToken = default)
    {
        if (employeeId <= 0)
            return Result<EmployeeDocumentDto>.Failure(
                "Invalid employee ID.");

        if (id <= 0)
            return Result<EmployeeDocumentDto>.Failure(
                "Invalid employee document ID.");

        var document = await _context.EmployeeDocuments
            .AsNoTracking()
            .Where(x =>
                x.Id == id &&
                x.EmployeeId == employeeId)
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

        if (document is null)
            return Result<EmployeeDocumentDto>.Failure(
                "Employee document not found.");

        return Result<EmployeeDocumentDto>.Succeeded(document);
    }
}