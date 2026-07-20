using Domin.Entities;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Documents.EmployeeDocuments.DTOs;
using MicroERP.Application.Features.Documents.EmployeeDocuments.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.Documents.EmployeeDocuments.Service
{
 
    public class EmployeeDocumentService : IEmployeeDocumentService
    {
        private readonly IApplicationDbContext _context;
        private readonly IFileStorageService _fileStorage;

        public EmployeeDocumentService(
            IApplicationDbContext context, IFileStorageService fileStorage)
        {
            _context = context;
            _fileStorage = fileStorage;
        }


        public async Task<Result<EmployeeDocumentDto>> CreateAsync(
     int employeeId,
     CreateEmployeeDocumentDto dto,
     CancellationToken cancellationToken = default)
        {
            var employeeExists = await _context.Employees
                .AnyAsync(x => x.Id == employeeId, cancellationToken);


            if (!employeeExists)
                return Result<EmployeeDocumentDto>.Failure(
                    "Employee not found");


            var filePath = await _fileStorage.SaveFileAsync(
                dto.File,
                "Employees/Documents",
                cancellationToken);


            var document = new EmployeeDocument
            {
                EmployeeId = employeeId,

                Name = dto.Name,

                FileName = dto.File.FileName,

                StoredFileName = Path.GetFileName(filePath),

                FilePath = filePath,

                ContentType = dto.File.ContentType,

                FileSize = dto.File.Length,

                IssueDate = dto.IssueDate,

                ExpiryDate = dto.ExpiryDate,

                Notes = dto.Notes
            };


            _context.EmployeeDocuments.Add(document);

            await _context.SaveChangesAsync(cancellationToken);


            return Result<EmployeeDocumentDto>.Succeeded(
                MapToDto(document));
        }


        public async Task<Result<EmployeeDocumentDto>> UpdateAsync(
       int id,
       UpdateEmployeeDocumentDto dto,
       CancellationToken cancellationToken = default)
        {
            var document = await _context.EmployeeDocuments
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);


            if (document == null)
                return Result<EmployeeDocumentDto>.Failure(
                    "Employee document not found");


            if (dto.File != null)
            {
                await _fileStorage.DeleteFileAsync(
                    document.FilePath,
                    cancellationToken);


                var filePath = await _fileStorage.SaveFileAsync(
                    dto.File,
                    "Employees/Documents",
                    cancellationToken);


                document.FilePath = filePath;
                document.StoredFileName = Path.GetFileName(filePath);
                document.FileName = dto.File.FileName;
                document.ContentType = dto.File.ContentType;
                document.FileSize = dto.File.Length;
            }


            if (!string.IsNullOrWhiteSpace(dto.Name))
                document.Name = dto.Name;


            if (dto.IssueDate.HasValue)
                document.IssueDate = dto.IssueDate;


            if (dto.ExpiryDate.HasValue)
                document.ExpiryDate = dto.ExpiryDate;


            if (dto.Notes != null)
                document.Notes = dto.Notes;


            await _context.SaveChangesAsync(cancellationToken);


            return Result<EmployeeDocumentDto>.Succeeded(
                MapToDto(document));
        }


        public async Task<Result> DeleteAsync(int id,
            CancellationToken cancellationToken = default)
        {
            var document = await _context.EmployeeDocuments
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);


            if (document == null)
                return Result.Failure(
                    "Employee document not found");


            _context.EmployeeDocuments.Remove(document);


            await _context.SaveChangesAsync(cancellationToken);

            await _fileStorage.DeleteFileAsync( document.FilePath,cancellationToken);

            return Result.Succeeded();
        }



        private static EmployeeDocumentDto MapToDto(
            EmployeeDocument document)
        {
            return new EmployeeDocumentDto
            {
                Id = document.Id,

                EmployeeId = document.EmployeeId,

                Name = document.Name,

                FileName = document.FileName,

                ContentType = document.ContentType,

                FileSize = document.FileSize,

                FilePath = document.FilePath,

                IssueDate = document.IssueDate,

                ExpiryDate = document.ExpiryDate,

                Notes = document.Notes
            };
        }
    }
}
