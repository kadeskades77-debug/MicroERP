using MicroERP.Application.Common.Files;
using MicroERP.Application.Common.Files.Interfaces;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.Ticketing.DTOs;
using MicroERP.Application.Features.Ticketing.Interfaces;
using MicroERP.Application.Features.Ticketing.Mappings;
using MicroERP.Domain.Audit;
using MicroERP.Domin.Entities.Ticketing;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MicroERP.Application.Features.Ticketing.Services;

public class TicketAttachmentService : ITicketAttachmentService
{
    private readonly IFileStorageService _fileStorage;
    private readonly IFileValidationService _fileValidationService;
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;
    private readonly FileValidationOptions _fileValidationOptions;

    public TicketAttachmentService(IFileStorageService fileStorage,
        IFileValidationService fileValidationService,
        IApplicationDbContext context, IAuditService auditService,
        IOptions<FileValidationSettings> fileValidationOptions)
    {
        _fileStorage = fileStorage;
        _fileValidationService = fileValidationService;
        _context = context;
        _auditService = auditService;
        _fileValidationOptions = fileValidationOptions.Value.TicketAttachments;
    }





    // =========================================================
    // Upload
    // =========================================================


public async Task<Result<TicketAttachmentDto>> UploadAsync(
    AddTicketAttachmentDto dto,
    CancellationToken ct = default)
    {
        // =====================================================
        // Validate Request
        // =====================================================

        if (dto is null)
        {
            return Result<TicketAttachmentDto>.Failure(
                "Request is required.");
        }

        if (dto.TicketId <= 0)
        {
            return Result<TicketAttachmentDto>.Failure(
                "Invalid ticket ID.");
        }

        if (dto.File is null)
        {
            return Result<TicketAttachmentDto>.Failure(
                "File is required.");
        }

        // =====================================================
        // Get Ticket
        // =====================================================

        var ticket =
            await _context.Tickets
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == dto.TicketId &&
                        x.IsActive,
                    ct);

        if (ticket is null)
        {
            return Result<TicketAttachmentDto>.Failure(
                "Ticket not found or inactive.");
        }

        // =====================================================
        // Validate Ticket Status
        // =====================================================

        if (ticket.Status == TicketStatus.Closed)
        {
            return Result<TicketAttachmentDto>.Failure(
                "Attachments cannot be added to closed tickets.");
        }

        if (ticket.Status == TicketStatus.Cancelled)
        {
            return Result<TicketAttachmentDto>.Failure(
                "Attachments cannot be added to cancelled tickets.");
        }

        // =====================================================
        // Validate File
        // =====================================================

        var validationResult =
            await _fileValidationService.ValidateAsync(
                dto.File,
                _fileValidationOptions,
                ct);

        if (!validationResult.Success)
        {
            return Result<TicketAttachmentDto>.Failure(
                validationResult.Message);
        }

        // =====================================================
        // Save Physical File
        // =====================================================

        var fileResult =
            await _fileStorage.SaveFileAsync(
                dto.File,
                "Tickets/Attachments",
                ct);

        if (!fileResult.Success ||
            string.IsNullOrWhiteSpace(fileResult.Data))
        {
            return Result<TicketAttachmentDto>.Failure(
                fileResult.Message);
        }

        var filePath =
            fileResult.Data;

        // =====================================================
        // Create Entity
        // =====================================================

        var attachment =
            new TicketAttachment
            {
                TicketId =
                    ticket.Id,

                FileName =
                    Path.GetFileName(
                        dto.File.FileName),

                FilePath =
                    filePath,

                ContentType =
                    dto.File.ContentType.Trim(),

                FileSize =
                    dto.File.Length
            };

        await _context.TicketAttachments.AddAsync(
            attachment,
            ct);

        // =====================================================
        // Create History
        // =====================================================

        var history =
            new TicketHistory
            {
                TicketId =
                    ticket.Id,

                Action =
                    TicketHistoryAction.AttachmentAdded,

                OldValue =
                    null,

                NewValue =
                    attachment.FileName,

                Notes =
                    $"Attachment uploaded. " +
                    $"ContentType: {attachment.ContentType}, " +
                    $"Size: {attachment.FileSize} bytes."
            };

        await _context.TicketHistories.AddAsync(
            history,
            ct);

        // =====================================================
        // Save Database
        // =====================================================

        try
        {
            await _context.SaveChangesAsync(ct);
        }
        catch
        {
            // =================================================
            // Rollback Physical File
            // =================================================

            await _fileStorage.DeleteFileAsync(
                filePath,
                CancellationToken.None);

            return Result<TicketAttachmentDto>.Failure(
                "Unable to save ticket attachment.");
        }

        // =====================================================
        // Audit
        // =====================================================

        await _auditService.LogAsync(
            AuditActions.Create,
            nameof(TicketAttachment),
            attachment.Id.ToString(),
            null,
            new
            {
                attachment.TicketId,
                attachment.FileName,
                attachment.FilePath,
                attachment.ContentType,
                attachment.FileSize
            });

        // =====================================================
        // Result
        // =====================================================

        return Result<TicketAttachmentDto>.Succeeded(
            attachment.ToDto(),
            "Attachment uploaded successfully.");
    }



    // =========================================================
    // Get By Ticket
    // =========================================================

    public async Task<Result<List<TicketAttachmentDto>>>
        GetByTicketIdAsync(
            int ticketId,
            CancellationToken ct = default)
    {
        // =====================================================
        // Validate
        // =====================================================

        if (ticketId <= 0)
        {
            return Result<List<TicketAttachmentDto>>.Failure(
                "Invalid ticket ID.");
        }

        // =====================================================
        // Check Ticket
        // =====================================================

        var ticketExists =
            await _context.Tickets
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == ticketId,
                    ct);

        if (!ticketExists)
        {
            return Result<List<TicketAttachmentDto>>.Failure(
                "Ticket not found.");
        }

        // =====================================================
        // Get Attachments
        // =====================================================

        var attachments =
            await _context.TicketAttachments
                .AsNoTracking()
                .Where(
                    x => x.TicketId == ticketId)
                .OrderByDescending(
                    x => x.CreatedOn)
                .Select(
                    x => new TicketAttachmentDto
                    {
                        Id =
                            x.Id,

                        TicketId =
                            x.TicketId,

                        FileName =
                            x.FileName,

                        FilePath =
                            x.FilePath,

                        ContentType =
                            x.ContentType,

                        FileSize =
                            x.FileSize,

                        CreatedOn =
                            x.CreatedOn
                    })
                .ToListAsync(ct);

        return Result<List<TicketAttachmentDto>>.Succeeded(
            attachments);
    }

    // =========================================================
    // Delete
    // =========================================================

    public async Task<Result> DeleteAsync(int id,
        CancellationToken ct = default)
    {
        // =====================================================
        // Validate
        // =====================================================

        if (id <= 0)
        {
            return Result.Failure(
                "Invalid attachment ID.");
        }

        // =====================================================
        // Get Attachment
        // =====================================================

        var attachment =
            await _context.TicketAttachments
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    ct);

        if (attachment is null)
        {
            return Result.Failure(
                "Attachment not found.");
        }

        // =====================================================
        // Old Values
        // =====================================================

        var oldValues = new
        {
            attachment.TicketId,
            attachment.FileName,
            attachment.FilePath,
            attachment.ContentType,
            attachment.FileSize,
            attachment.IsActive,
            attachment.IsDeleted
        };

        // =====================================================
        // Soft Delete
        // =====================================================

        attachment.IsDeleted = true;
        attachment.IsActive = false;

        await _context.SaveChangesAsync(ct);

        // =====================================================
        // Delete Physical File
        // =====================================================

        var fileResult =
             _fileStorage.DeleteFileAsync(
                attachment.FilePath,
                ct);

        // =====================================================
        // Audit
        // =====================================================

        await _auditService.LogAsync(
            AuditActions.Delete,
            nameof(TicketAttachment),
            attachment.Id.ToString(),
            oldValues,
            new
            {
                attachment.TicketId,
                attachment.FileName,
                attachment.FilePath,
                attachment.ContentType,
                attachment.FileSize,
                attachment.IsActive,
                attachment.IsDeleted
            });

        // =====================================================
        // Result
        // =====================================================

        if (!fileResult.IsCompletedSuccessfully)
        {
            return Result.Succeeded(
                "Attachment deleted from the system, but the physical file could not be removed.");
        }

        return Result.Succeeded(
            "Attachment deleted successfully.");
    }

  

}

