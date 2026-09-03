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

namespace MicroERP.Application.Features.Ticketing.Services;

public class TicketCommentService : ITicketCommentService
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;
    private readonly ICurrentUserService _currentUser;

    public TicketCommentService(
        IApplicationDbContext context,
        IAuditService auditService,
        ICurrentUserService currentUser)
    {
        _context = context;
        _auditService = auditService;
        _currentUser = currentUser;
    }


    // =========================================================
    // Create Comment
    // =========================================================


public async Task<Result<TicketCommentDto>> CreateAsync(
    AddTicketCommentDto dto,
    CancellationToken ct = default)
    {
        // =====================================================
        // Validate Request
        // =====================================================

        if (dto is null)
        {
            return Result<TicketCommentDto>.Failure(
                "Request is required.");
        }

        if (dto.TicketId <= 0)
        {
            return Result<TicketCommentDto>.Failure(
                "Invalid ticket ID.");
        }

        if (string.IsNullOrWhiteSpace(dto.Comment))
        {
            return Result<TicketCommentDto>.Failure(
                "Comment is required.");
        }

        var commentText =
            dto.Comment.Trim();

        if (commentText.Length > 5000)
        {
            return Result<TicketCommentDto>.Failure(
                "Comment cannot exceed 5000 characters.");
        }

        // =====================================================
        // Current User
        // =====================================================

        var userId =
            _currentUser.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result<TicketCommentDto>.Failure(
                "Current user could not be determined.");
        }

        // =====================================================
        // Get Employee
        // =====================================================

        var employee =
            await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x =>
                        x.UserId == userId &&
                        x.IsActive,
                    ct);

        if (employee is null)
        {
            return Result<TicketCommentDto>.Failure(
                "Employee not found or inactive.");
        }

        // =====================================================
        // Get Ticket
        // =====================================================

        var ticket =
            await _context.Tickets
                .FirstOrDefaultAsync(
                    x => x.Id == dto.TicketId,
                    ct);

        if (ticket is null)
        {
            return Result<TicketCommentDto>.Failure(
                "Ticket not found.");
        }

        // =====================================================
        // Closed Ticket
        // =====================================================

        if (ticket.Status == TicketStatus.Closed)
        {
            return Result<TicketCommentDto>.Failure(
                "Comments cannot be added to closed tickets.");
        }

        // =====================================================
        // Cancelled Ticket
        // =====================================================

        if (ticket.Status == TicketStatus.Cancelled)
        {
            return Result<TicketCommentDto>.Failure(
                "Comments cannot be added to cancelled tickets.");
        }

        // =====================================================
        // Create Comment
        // =====================================================

        var comment =
            new TicketComment
            {
                TicketId =
                    ticket.Id,

                EmployeeId =
                    employee.Id,

                Comment =
                    commentText
            };

        await _context.TicketComments.AddAsync(
            comment,
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
                    TicketHistoryAction.CommentAdded,

                OldValue =
                    null,

                NewValue =
                    commentText,

                Notes =
                    null
            };

        await _context.TicketHistories.AddAsync(
            history,
            ct);

        // =====================================================
        // Save Changes
        // =====================================================

        await _context.SaveChangesAsync(ct);

        // =====================================================
        // Audit
        // =====================================================

        await _auditService.LogAsync(
            AuditActions.Create,
            nameof(TicketComment),
            comment.Id.ToString(),
            null,
            new
            {
                comment.TicketId,
                comment.EmployeeId,
                comment.Comment
            });

        // =====================================================
        // Result
        // =====================================================

        return Result<TicketCommentDto>.Succeeded(
            comment.ToDto(),
            "Comment added successfully.");
    }


    // =========================================================
    // Update Comment
    // =========================================================

    public async Task<Result<TicketCommentDto>> UpdateAsync(
        int id,
        UpdateTicketCommentDto dto,
        CancellationToken ct = default)
    {
        // =====================================================
        // Validate ID
        // =====================================================

        if (id <= 0)
        {
            return Result<TicketCommentDto>.Failure(
                "Invalid comment ID.");
        }

        // =====================================================
        // Validate DTO
        // =====================================================

        if (dto is null)
        {
            return Result<TicketCommentDto>.Failure(
                "Request is required.");
        }

        if (dto.Comment is null)
        {
            return Result<TicketCommentDto>.Failure(
                "No fields provided for update.");
        }

        // =====================================================
        // Current User
        // =====================================================

        var userId =
            _currentUser.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result<TicketCommentDto>.Failure(
                "Current user could not be determined.");
        }

        // =====================================================
        // Get Employee
        // =====================================================

        var employeeId =
            await _context.Employees
                .AsNoTracking()
                .Where(x =>
                    x.UserId == userId &&
                    x.IsActive)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync(ct);

        if (!employeeId.HasValue)
        {
            return Result<TicketCommentDto>.Failure(
                "Employee not found or inactive.");
        }

        // =====================================================
        // Get Comment
        // =====================================================

        var comment =
            await _context.TicketComments
                .Include(x => x.Employee)
                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    ct);

        if (comment is null)
        {
            return Result<TicketCommentDto>.Failure(
                "Comment not found.");
        }

        // =====================================================
        // Ownership
        // =====================================================

        if (comment.EmployeeId != employeeId.Value)
        {
            return Result<TicketCommentDto>.Failure(
                "You can only modify your own comments.");
        }

        // =====================================================
        // Get Ticket
        // =====================================================

        var ticket =
            await _context.Tickets
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == comment.TicketId,
                    ct);

        if (ticket is null)
        {
            return Result<TicketCommentDto>.Failure(
                "Ticket not found.");
        }

        // =====================================================
        // Closed Ticket
        // =====================================================

        if (ticket.Status == TicketStatus.Closed)
        {
            return Result<TicketCommentDto>.Failure(
                "Comments on closed tickets cannot be modified.");
        }

        // =====================================================
        // Validate Comment
        // =====================================================

        if (string.IsNullOrWhiteSpace(dto.Comment))
        {
            return Result<TicketCommentDto>.Failure(
                "Comment cannot be empty.");
        }

        var newComment =
            dto.Comment.Trim();

        if (newComment.Length > 5000)
        {
            return Result<TicketCommentDto>.Failure(
                "Comment cannot exceed 5000 characters.");
        }

        // =====================================================
        // Check No Change
        // =====================================================

        if (comment.Comment == newComment)
        {
            return Result<TicketCommentDto>.Failure(
                "No changes were made.");
        }

        // =====================================================
        // Old Values
        // =====================================================

        var oldValues = new
        {
            comment.TicketId,
            comment.EmployeeId,
            comment.Comment
        };

        // =====================================================
        // Update
        // =====================================================

        comment.Comment =
            newComment;

        await _context.SaveChangesAsync(ct);

        // =====================================================
        // Audit
        // =====================================================

        await _auditService.LogAsync(
            AuditActions.Update,
            nameof(TicketComment),
            comment.Id.ToString(),
            oldValues,
            new
            {
                comment.TicketId,
                comment.EmployeeId,
                comment.Comment
            });

        // =====================================================
        // Result
        // =====================================================

        return Result<TicketCommentDto>.Succeeded(
      comment.ToDto(),
      "Comment updated successfully.");

    }



    // =========================================================
    // Delete Comment
    // =========================================================

    public async Task<Result> DeleteAsync(
        int id,
        CancellationToken ct = default)
    {
        // =====================================================
        // Validate ID
        // =====================================================

        if (id <= 0)
        {
            return Result.Failure(
                "Invalid comment ID.");
        }

        // =====================================================
        // Current User
        // =====================================================

        var userId =
            _currentUser.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result.Failure(
                "Current user could not be determined.");
        }

        // =====================================================
        // Get Employee
        // =====================================================

        var employeeId =
            await _context.Employees
                .AsNoTracking()
                .Where(x =>
                    x.UserId == userId &&
                    x.IsActive)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync(ct);

        if (!employeeId.HasValue)
        {
            return Result.Failure(
                "Employee not found or inactive.");
        }

        // =====================================================
        // Get Comment
        // =====================================================

        var comment =
            await _context.TicketComments
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    ct);

        if (comment is null)
        {
            return Result.Failure(
                "Comment not found.");
        }

        // =====================================================
        // Ownership
        // =====================================================

        if (comment.EmployeeId != employeeId.Value)
        {
            return Result.Failure(
                "You can only delete your own comments.");
        }

        // =====================================================
        // Old Values
        // =====================================================

        var oldValues = new
        {
            comment.TicketId,
            comment.EmployeeId,
            comment.Comment,
            comment.IsActive,
            comment.IsDeleted
        };

        // =====================================================
        // Soft Delete
        // =====================================================

        comment.IsDeleted = true;
        comment.IsActive = false;

        await _context.SaveChangesAsync(ct);

        // =====================================================
        // Audit
        // =====================================================

        await _auditService.LogAsync(
            AuditActions.Delete,
            nameof(TicketComment),
            comment.Id.ToString(),
            oldValues,
            new
            {
                comment.TicketId,
                comment.EmployeeId,
                comment.Comment,
                comment.IsActive,
                comment.IsDeleted
            });

        // =====================================================
        // Result
        // =====================================================

        return Result.Succeeded(
            "Comment deleted successfully.");
    }



    // =========================================================
    // Get Comments By Ticket
    // =========================================================

    public async Task<Result<List<TicketCommentDto>>>
      GetByTicketIdAsync(
          int ticketId,
          CancellationToken ct = default)
    {
        // =====================================================
        // Validate
        // =====================================================

        if (ticketId <= 0)
        {
            return Result<List<TicketCommentDto>>.Failure(
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
            return Result<List<TicketCommentDto>>.Failure(
                "Ticket not found.");
        }

        // =====================================================
        // Get Comments
        // =====================================================

        var comments =
            await _context.TicketComments
                .AsNoTracking()
                .Where(x => x.TicketId == ticketId)
                .OrderBy(x => x.CreatedOn)
                .ToListAsync(ct);

        // =====================================================
        // Map
        // =====================================================

        var result =
            comments
                .Select(x => x.ToDto())
                .ToList();

        // =====================================================
        // Result
        // =====================================================

        return Result<List<TicketCommentDto>>.Succeeded(
            result);
    }

 
}

