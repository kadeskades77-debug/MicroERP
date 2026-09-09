using MicroERP.Application.Common.Files;
using MicroERP.Application.Common.Files.Interfaces;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.Documents.EmployeeDocuments.DTOs;
using MicroERP.Application.Features.Documents.EmployeeDocuments.Interfaces;
using MicroERP.Domain.Audit;
using MicroERP.Domin.Entities.Employees;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MicroERP.Application.Features.Documents.EmployeeDocuments.Service
{
 
    public class EmployeeDocumentService : IEmployeeDocumentService
    {
        private readonly IFileStorageService _fileStorage;
        private readonly IFileValidationService _fileValidationService;
        private readonly IApplicationDbContext _context;
        private readonly IAuditService _auditService;
        private readonly FileValidationOptions _fileValidationOptions;
        


        public EmployeeDocumentService(
            IApplicationDbContext context, IFileStorageService fileStorage, IFileValidationService fileValidationService,
            IAuditService auditService,
            IOptions<FileValidationSettings> fileValidationOptions)

        {
            _context = context;
            _fileStorage = fileStorage;
            _fileValidationService = fileValidationService;
            _auditService = auditService;
            _fileValidationOptions = fileValidationOptions.Value.EmployeeDocuments;
     
        }


        public async Task<Result<EmployeeDocumentDto>> CreateAsync(
         int employeeId,
         CreateEmployeeDocumentDto dto,
         CancellationToken cancellationToken = default)
        {
            // =========================================================
            // Validate Request
            // =========================================================

            if (employeeId <= 0)
            {
                return Result<EmployeeDocumentDto>.Failure(
                    "Invalid employee ID.");
            }

            if (dto is null)
            {
                return Result<EmployeeDocumentDto>.Failure(
                    "Request is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return Result<EmployeeDocumentDto>.Failure(
                    "Document name cannot be empty.");
            }

            if (dto.File is null)
            {
                return Result<EmployeeDocumentDto>.Failure(
                    "File is required.");
            }

            // =========================================================
            // Validate Dates
            // =========================================================

            if (dto.IssueDate.HasValue &&
                dto.ExpiryDate.HasValue &&
                dto.ExpiryDate.Value < dto.IssueDate.Value)
            {
                return Result<EmployeeDocumentDto>.Failure(
                    "Expiry date cannot be earlier than issue date.");
            }

            // =========================================================
            // Validate Employee
            // =========================================================

            var employeeExists =
                await _context.Employees
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.Id == employeeId &&
                            x.IsActive &&
                            !x.IsDeleted,
                        cancellationToken);

            if (!employeeExists)
            {
                return Result<EmployeeDocumentDto>.Failure(
                    "Employee not found or inactive.");
            }

            // =========================================================
            // Validate File
            // =========================================================

            var validationResult =
                await _fileValidationService.ValidateAsync(
                    dto.File,
                    _fileValidationOptions,
                    cancellationToken);

            if (!validationResult.Success)
            {
                return Result<EmployeeDocumentDto>.Failure(
                    validationResult.Message);
            }

            // =========================================================
            // Save Physical File
            // =========================================================

            var fileResult =
                await _fileStorage.SaveFileAsync(
                    dto.File,
                    "Employees/Documents",
                    cancellationToken);

            if (!fileResult.Success ||
                string.IsNullOrWhiteSpace(fileResult.Data))
            {
                return Result<EmployeeDocumentDto>.Failure(
                    fileResult.Message);
            }

            var filePath = fileResult.Data;

            // =========================================================
            // Create Entity
            // =========================================================

            var document = new EmployeeDocument
            {
                EmployeeId = employeeId,

                Name = dto.Name.Trim(),

                FileName =
                    Path.GetFileName(dto.File.FileName),

                StoredFileName =
                    Path.GetFileName(filePath),

                FilePath = filePath,

                ContentType =
                    dto.File.ContentType.Trim(),

                FileSize = dto.File.Length,

                IssueDate = dto.IssueDate,

                ExpiryDate = dto.ExpiryDate,

                Notes = dto.Notes?.Trim()
            };

            // =========================================================
            // Save Database
            // =========================================================

            try
            {
                _context.EmployeeDocuments.Add(document);

                await _context.SaveChangesAsync(
                    cancellationToken);
            }
            catch
            {
                // DB failed after physical file was saved.
                // Remove the newly created physical file.
                await _fileStorage.DeleteFileAsync(
                    filePath,
                    CancellationToken.None);

                return Result<EmployeeDocumentDto>.Failure(
                    "Unable to save employee document.");
            }

            // =========================================================
            // Audit
            // =========================================================

            await _auditService.LogAsync(
                AuditActions.Create,
                nameof(EmployeeDocument),
                document.Id.ToString(),
                null,
                new
                {
                    document.EmployeeId,
                    document.Name,
                    document.FileName,
                    document.FileSize,
                    document.ContentType,
                    document.IssueDate,
                    document.ExpiryDate
                });

            // =========================================================
            // Return Result
            // =========================================================

            return Result<EmployeeDocumentDto>.Succeeded(
                MapToDto(document),
                "Employee document created successfully.");
        }

        public async Task<Result<EmployeeDocumentDto>> UpdateAsync(
        int employeeId,
        int id,
        UpdateEmployeeDocumentDto dto,
        CancellationToken cancellationToken = default)
        {
            // =========================================================
            // Validate Request
            // =========================================================

            if (employeeId <= 0)
            {
                return Result<EmployeeDocumentDto>.Failure(
                    "Invalid employee ID.");
            }

            if (id <= 0)
            {
                return Result<EmployeeDocumentDto>.Failure(
                    "Invalid employee document ID.");
            }

            if (dto is null)
            {
                return Result<EmployeeDocumentDto>.Failure(
                    "Request is required.");
            }

            // =========================================================
            // Validate Document Name
            // =========================================================

            if (dto.Name is not null &&
                string.IsNullOrWhiteSpace(dto.Name))
            {
                return Result<EmployeeDocumentDto>.Failure(
                    "Document name cannot be empty.");
            }

            // =========================================================
            // Get Document
            // =========================================================

            var document =
                await _context.EmployeeDocuments
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id == id &&
                            x.EmployeeId == employeeId,
                        cancellationToken);

            if (document is null)
            {
                return Result<EmployeeDocumentDto>.Failure(
                    "Employee document not found.");
            }

            // =========================================================
            // Validate Dates
            // =========================================================

            var issueDate =
                dto.IssueDate ?? document.IssueDate;

            var expiryDate =
                dto.ExpiryDate ?? document.ExpiryDate;

            if (issueDate.HasValue &&
                expiryDate.HasValue &&
                expiryDate.Value < issueDate.Value)
            {
                return Result<EmployeeDocumentDto>.Failure(
                    "Expiry date cannot be earlier than issue date.");
            }

            // =========================================================
            // Store Old File Path
            // =========================================================

            var oldFilePath = document.FilePath;

            string? newFilePath = null;

            // =========================================================
            // Replace File
            // =========================================================

            if (dto.File is not null)
            {
                if (dto.File.Length == 0)
                {
                    return Result<EmployeeDocumentDto>.Failure(
                        "File cannot be empty.");
                }

                // -----------------------------------------------------
                // Validate New File
                // -----------------------------------------------------

                var validationResult =
                    await _fileValidationService.ValidateAsync(
                        dto.File,
                        _fileValidationOptions,
                        cancellationToken);

                if (!validationResult.Success)
                {
                    return Result<EmployeeDocumentDto>.Failure(
                        validationResult.Message);
                }

                // -----------------------------------------------------
                // Save New Physical File
                // -----------------------------------------------------

                var fileResult =
                    await _fileStorage.SaveFileAsync(
                        dto.File,
                        "Employees/Documents",
                        cancellationToken);

                if (!fileResult.Success ||
                    string.IsNullOrWhiteSpace(fileResult.Data))
                {
                    return Result<EmployeeDocumentDto>.Failure(
                        fileResult.Message);
                }

                newFilePath = fileResult.Data;

                // -----------------------------------------------------
                // Update File Information
                // -----------------------------------------------------

                document.FilePath = newFilePath;

                document.StoredFileName =
                    Path.GetFileName(newFilePath);

                document.FileName =
                    Path.GetFileName(dto.File.FileName);

                document.ContentType =
                    dto.File.ContentType.Trim();

                document.FileSize =
                    dto.File.Length;
            }

            // =========================================================
            // Update Basic Information
            // =========================================================

            if (dto.Name is not null)
            {
                document.Name = dto.Name.Trim();
            }

            if (dto.IssueDate.HasValue)
            {
                document.IssueDate = dto.IssueDate.Value;
            }

            if (dto.ExpiryDate.HasValue)
            {
                document.ExpiryDate = dto.ExpiryDate.Value;
            }

            if (dto.Notes is not null)
            {
                document.Notes = dto.Notes.Trim();
            }

            // =========================================================
            // Save Database
            // =========================================================

            try
            {
                await _context.SaveChangesAsync(
                    cancellationToken);
            }
            catch
            {
                // DB failed.
                // Delete newly uploaded file.
                // Keep the old file untouched.
                if (newFilePath is not null)
                {
                    await _fileStorage.DeleteFileAsync(
                        newFilePath,
                        CancellationToken.None);
                }

                return Result<EmployeeDocumentDto>.Failure(
                    "Unable to update employee document.");
            }

            // =========================================================
            // Delete Old Physical File
            // =========================================================

            if (newFilePath is not null &&
                !string.IsNullOrWhiteSpace(oldFilePath) &&
                !string.Equals(
                    oldFilePath,
                    newFilePath,
                    StringComparison.OrdinalIgnoreCase))
            {
                await _fileStorage.DeleteFileAsync(
                    oldFilePath,
                    CancellationToken.None);
            }

            // =========================================================
            // Audit
            // =========================================================

            await _auditService.LogAsync(
                AuditActions.Update,
                nameof(EmployeeDocument),
                document.Id.ToString(),
                null,
                new
                {
                    document.EmployeeId,
                    document.Name,
                    document.FileName,
                    document.FileSize,
                    document.ContentType,
                    document.IssueDate,
                    document.ExpiryDate
                });

            // =========================================================
            // Return Result
            // =========================================================

            return Result<EmployeeDocumentDto>.Succeeded(
                MapToDto(document),
                "Employee document updated successfully.");
        }



        public async Task<Result> DeleteAsync(
        int employeeId,
        int id,
        CancellationToken cancellationToken = default)
        {
            if (employeeId <= 0)
            {
                return Result.Failure("Invalid employee ID.");
            }

            if (id <= 0)
            {
                return Result.Failure("Invalid employee document ID.");
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    cancellationToken);

            try
            {
                var document =
                    await _context.EmployeeDocuments
                        .FirstOrDefaultAsync(
                            x =>
                                x.Id == id &&
                                x.EmployeeId == employeeId,
                            cancellationToken);

                if (document is null)
                {
                    return Result.Failure(
                        "Employee document not found.");
                }

                document.IsDeleted = true;
                document.IsActive = false;

                await _context.SaveChangesAsync(
                    cancellationToken);

                await transaction.CommitAsync(
                    cancellationToken);

                if (!string.IsNullOrWhiteSpace(document.FilePath))
                {
                    await _fileStorage.DeleteFileAsync(
                        document.FilePath,
                        CancellationToken.None);
                }

                await _auditService.LogAsync(
                    AuditActions.Delete,
                    nameof(EmployeeDocument),
                    document.Id.ToString(),
                    null,
                    new
                    {
                        document.EmployeeId,
                        document.Name,
                        document.FileName,
                        document.FileSize,
                        document.ContentType
                    });

                return Result.Succeeded(
                    "Employee document deleted successfully.");
            }
            catch
            {
                await transaction.RollbackAsync(
                    CancellationToken.None);

                return Result.Failure(
                    "An unexpected error occurred.");
            }
        }

        public async Task<Result> DeleteByEmployeeAsync(
       int employeeId,
       CancellationToken cancellationToken = default)
        {
            if (employeeId <= 0)
            {
                return Result.Failure(
                    "Invalid employee ID.");
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    cancellationToken);

            try
            {
                var documents =
                    await _context.EmployeeDocuments
                        .Where(x => x.EmployeeId == employeeId)
                        .ToListAsync(cancellationToken);

                if (!documents.Any())
                {
                    await transaction.CommitAsync(
                        cancellationToken);

                    return Result.Succeeded(
                        "No employee documents found.");
                }

                foreach (var document in documents)
                {
                    document.IsDeleted = true;
                    document.IsActive = false;
                }

                await _context.SaveChangesAsync(
                    cancellationToken);

                await transaction.CommitAsync(
                    cancellationToken);

                foreach (var document in documents)
                {
                    if (!string.IsNullOrWhiteSpace(document.FilePath))
                    {
                        await _fileStorage.DeleteFileAsync(
                            document.FilePath,
                            CancellationToken.None);
                    }
                }

                await _auditService.LogAsync(
                    AuditActions.Delete,
                    nameof(EmployeeDocument),
                    employeeId.ToString(),
                    null,
                    new
                    {
                        EmployeeId = employeeId,
                        DeletedDocumentsCount = documents.Count,
                        DocumentIds = documents
                            .Select(x => x.Id)
                            .ToList()
                    });

                return Result.Succeeded(
                    "Employee documents deleted successfully.");
            }
            catch
            {
                await transaction.RollbackAsync(
                    CancellationToken.None);

                return Result.Failure(
                    "An unexpected error occurred.");
            }
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
