using MicroERP.Application.Common.Files;
using MicroERP.Application.Common.Files.Interfaces;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.Documents.LeaveDocuments.DTOS;
using MicroERP.Application.Features.Documents.LeaveDocuments.Interfaces;
using MicroERP.Domain.Audit;
using MicroERP.Domin.Entities.EmployeeLeaves;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


namespace MicroERP.Application.Features.Documents.LeaveDocuments.Service
{
    public class LeaveAttachmentService : ILeaveAttachmentService
    {
        private readonly IFileStorageService _fileStorage;
        private readonly IFileValidationService _fileValidationService;
        private readonly IApplicationDbContext _context;
        private readonly IAuditService _auditService;
        private readonly FileValidationOptions _fileValidationOptions;


        public LeaveAttachmentService(
            IApplicationDbContext context,
            IFileStorageService fileStorage,
            IFileValidationService fileValidationService,
            IAuditService auditService,
           IOptions<FileValidationSettings> fileValidationOptions)
        {
            _context = context;
            _fileStorage = fileStorage;
            _fileValidationService = fileValidationService;
            _auditService = auditService;
            _fileValidationOptions = fileValidationOptions.Value.LeaveAttachments;
        }



        public async Task<Result<LeaveAttachmentDto>> UploadEmployeeLeaveAsync(
         int leaveId,
         UploadLeaveAttachmentDto dto,
         CancellationToken cancellationToken = default)
        {
            // =========================================================
            // Validate Leave ID
            // =========================================================

            if (leaveId <= 0)
            {
                return Result<LeaveAttachmentDto>.Failure(
                    "Invalid leave ID.");
            }

            // =========================================================
            // Validate DTO
            // =========================================================

            if (dto is null)
            {
                return Result<LeaveAttachmentDto>.Failure(
                    "Request is required.");
            }

            // =========================================================
            // Validate File
            // =========================================================

            if (dto.File is null)
            {
                return Result<LeaveAttachmentDto>.Failure(
                    "File is required.");
            }

            // =========================================================
            // Validate Leave
            // =========================================================

            var leaveExists =
                await _context.EmployeeLeaves
                    .AsNoTracking()
                    .AnyAsync(
                        x => x.Id == leaveId &&
                             x.IsActive &&
                             !x.IsDeleted,
                        cancellationToken);

            if (!leaveExists)
            {
                return Result<LeaveAttachmentDto>.Failure(
                    "Leave not found or inactive.");
            }

            // =========================================================
            // Validate File Content
            // =========================================================

            var validationResult =
                await _fileValidationService.ValidateAsync(
                    dto.File,
                    _fileValidationOptions,
                    cancellationToken);

            if (!validationResult.Success)
            {
                return Result<LeaveAttachmentDto>.Failure(
                    validationResult.Message);
            }

            // =========================================================
            // Save Physical File
            // =========================================================

            var fileResult =
                await _fileStorage.SaveFileAsync(
                    dto.File,
                    "Leaves/EmployeeLeave",
                    cancellationToken);

            if (!fileResult.Success ||
                string.IsNullOrWhiteSpace(fileResult.Data))
            {
                return Result<LeaveAttachmentDto>.Failure(
                    fileResult.Message);
            }

            var filePath = fileResult.Data;

            // =========================================================
            // Create Attachment
            // =========================================================

            var attachment =
                new LeaveAttachment
                {
                    FileName =
                        Path.GetFileName(dto.File.FileName),

                    FilePath =
                        filePath,

                    EmployeeLeaveId =
                        leaveId
                };

            _context.LeaveAttachments.Add(
                attachment);

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
                // DB failed after physical file was saved.
                // Delete the newly created file.

                await _fileStorage.DeleteFileAsync(
                    filePath,
                    CancellationToken.None);

                return Result<LeaveAttachmentDto>.Failure(
                    "Unable to save leave attachment.");
            }

            // =========================================================
            // Audit
            // =========================================================

            await _auditService.LogAsync(
                AuditActions.Create,
                nameof(LeaveAttachment),
                attachment.Id.ToString(),
                null,
                new
                {
                    attachment.EmployeeLeaveId,
                    attachment.FileName,
                    attachment.FilePath
                });

            // =========================================================
            // Result
            // =========================================================

            return Result<LeaveAttachmentDto>.Succeeded(
                new LeaveAttachmentDto
                {
                    Id =
                        attachment.Id,

                    FileName =
                        attachment.FileName,

                    FilePath =
                        attachment.FilePath,

                    EmployeeLeaveId =
                        attachment.EmployeeLeaveId,

                    CreatedOn =
                        attachment.CreatedOn
                },
                "Attachment uploaded successfully.");
        }




        public async Task<Result<LeaveAttachmentDto>> UploadEmployeeSpecialLeaveAsync(
        int leaveId,
        UploadLeaveAttachmentDto dto,
        CancellationToken cancellationToken = default)
        {
            // =========================================================
            // Validate Special Leave ID
            // =========================================================

            if (leaveId <= 0)
            {
                return Result<LeaveAttachmentDto>.Failure(
                    "Invalid special leave ID.");
            }

            // =========================================================
            // Validate DTO
            // =========================================================

            if (dto is null)
            {
                return Result<LeaveAttachmentDto>.Failure(
                    "Request is required.");
            }

            // =========================================================
            // Validate File
            // =========================================================

            if (dto.File is null)
            {
                return Result<LeaveAttachmentDto>.Failure(
                    "File is required.");
            }

            // =========================================================
            // Validate Special Leave
            // =========================================================

            var leaveExists =
                await _context.EmployeeSpecialLeaves
                    .AsNoTracking()
                    .AnyAsync(
                        x => x.Id == leaveId &&
                             x.IsActive &&
                             !x.IsDeleted,
                        cancellationToken);

            if (!leaveExists)
            {
                return Result<LeaveAttachmentDto>.Failure(
                    "Special leave not found or inactive.");
            }

            // =========================================================
            // Validate File Content
            // =========================================================

            var validationResult =
                await _fileValidationService.ValidateAsync(
                    dto.File,
                    _fileValidationOptions,
                    cancellationToken);

            if (!validationResult.Success)
            {
                return Result<LeaveAttachmentDto>.Failure(
                    validationResult.Message);
            }

            // =========================================================
            // Save Physical File
            // =========================================================

            var fileResult =
                await _fileStorage.SaveFileAsync(
                    dto.File,
                    "Leaves/SpecialLeave",
                    cancellationToken);

            if (!fileResult.Success ||
                string.IsNullOrWhiteSpace(fileResult.Data))
            {
                return Result<LeaveAttachmentDto>.Failure(
                    fileResult.Message);
            }

            var filePath = fileResult.Data;

            // =========================================================
            // Create Attachment
            // =========================================================

            var attachment =
                new LeaveAttachment
                {
                    FileName =
                        Path.GetFileName(dto.File.FileName),

                    FilePath =
                        filePath,

                    EmployeeSpecialLeaveId =
                        leaveId
                };

            _context.LeaveAttachments.Add(
                attachment);

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
                // DB failed after physical file was saved.
                // Delete the newly created file.

                await _fileStorage.DeleteFileAsync(
                    filePath,
                    CancellationToken.None);

                return Result<LeaveAttachmentDto>.Failure(
                    "Unable to save special leave attachment.");
            }

            // =========================================================
            // Audit
            // =========================================================

            await _auditService.LogAsync(
                AuditActions.Create,
                nameof(LeaveAttachment),
                attachment.Id.ToString(),
                null,
                new
                {
                    attachment.EmployeeSpecialLeaveId,
                    attachment.FileName,
                    attachment.FilePath
                });

            // =========================================================
            // Result
            // =========================================================

            return Result<LeaveAttachmentDto>.Succeeded(
                new LeaveAttachmentDto
                {
                    Id =
                        attachment.Id,

                    FileName =
                        attachment.FileName,

                    FilePath =
                        attachment.FilePath,

                    EmployeeSpecialLeaveId =
                        attachment.EmployeeSpecialLeaveId,

                    CreatedOn =
                        attachment.CreatedOn
                },
                "Attachment uploaded successfully.");
        }



        public async Task<Result<List<LeaveAttachmentDto>>> GetByLeaveAsync(int leaveId,LeaveCategory category,
      CancellationToken cancellationToken = default)
        {
            IQueryable<LeaveAttachment> query;

            if (category == LeaveCategory.Regular)
            {
                query = _context.LeaveAttachments
                    .Where(x => x.EmployeeLeaveId == leaveId);
            }
            else if (category == LeaveCategory.Special)
            {
                query = _context.LeaveAttachments
                    .Where(x => x.EmployeeSpecialLeaveId == leaveId);
            }
            else
            {
                return Result<List<LeaveAttachmentDto>>
                    .Failure("Invalid leave category.");
            }

            var attachments = await query
                .Select(x => new LeaveAttachmentDto
                {
                    Id = x.Id,
                    FileName = x.FileName,
                    FilePath = x.FilePath,
                    EmployeeLeaveId = x.EmployeeLeaveId,
                    EmployeeSpecialLeaveId = x.EmployeeSpecialLeaveId,
                    Category = x.EmployeeLeaveId != null
                        ? LeaveCategory.Regular
                        : LeaveCategory.Special,
                    CreatedOn = x.CreatedOn
                })
                .ToListAsync(cancellationToken);

            return Result<List<LeaveAttachmentDto>>
                .Succeeded(attachments);
        }


        public async Task<Result> DeleteAsync(int attachmentId,
             CancellationToken cancellationToken = default)
        {
            var attachment = await _context.LeaveAttachments
                .FirstOrDefaultAsync(
                    x => x.Id == attachmentId,
                    cancellationToken);


            if (attachment == null)
                return Result.Failure(
                    "Attachment not found");


            await _fileStorage.DeleteFileAsync(
                attachment.FilePath,
                cancellationToken);


            _context.LeaveAttachments.Remove(attachment);


            await _context.SaveChangesAsync(cancellationToken);


            return Result.Succeeded();
        }
    }
}
