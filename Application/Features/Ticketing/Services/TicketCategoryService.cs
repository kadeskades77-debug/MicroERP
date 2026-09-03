using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.Ticketing.DTOs;
using MicroERP.Application.Features.Ticketing.Interfaces;
using MicroERP.Domain.Audit;
using MicroERP.Domin.Entities.Ticketing;

using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.Ticketing.Services;

public class TicketCategoryService : ITicketCategoryService
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public TicketCategoryService(
        IApplicationDbContext context,
        IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    // =========================================================
    // Create
    // =========================================================

    public async Task<Result<TicketCategoryDto>> CreateAsync(
        CreateTicketCategoryDto dto,
        CancellationToken ct = default)
    {
        // =====================================================
        // Validate
        // =====================================================

        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return Result<TicketCategoryDto>.Failure(
                "Category name is required.");
        }

        var name = dto.Name.Trim();

        // =====================================================
        // Check Duplicate
        // =====================================================

        var exists =
            await _context.TicketCategories
                .AsNoTracking()
                .AnyAsync(
                    x => x.Name == name,
                    ct);

        if (exists)
        {
            return Result<TicketCategoryDto>.Failure(
                "A ticket category with this name already exists.");
        }

        // =====================================================
        // Create
        // =====================================================

        var category = new TicketCategory
        {
            Name = name,

            Description =
                string.IsNullOrWhiteSpace(dto.Description)
                    ? null
                    : dto.Description.Trim()
        };

        await _context.TicketCategories.AddAsync(
            category,
            ct);

        await _context.SaveChangesAsync(ct);

        // =====================================================
        // Audit
        // =====================================================

        await _auditService.LogAsync(
            AuditActions.Create,
            nameof(TicketCategory),
            category.Id.ToString(),
            null,
            new
            {
                category.Name,
                category.Description
            });

        // =====================================================
        // Result
        // =====================================================

        return Result<TicketCategoryDto>.Succeeded(
            MapToDto(category),
            "Ticket category created successfully.");
    }

    // =========================================================
    // Get By Id
    // =========================================================

    public async Task<Result<TicketCategoryDto>> GetByIdAsync(int id,
        CancellationToken ct = default)
    {
        if (id <= 0)
        {
            return Result<TicketCategoryDto>.Failure(
                "Invalid category ID.");
        }

        var category =
            await _context.TicketCategories
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    ct);

        if (category is null)
        {
            return Result<TicketCategoryDto>.Failure(
                "Ticket category not found.");
        }

        return Result<TicketCategoryDto>.Succeeded(
            MapToDto(category));
    }

    // =========================================================
    // Get All
    // =========================================================

    public async Task<Result<List<TicketCategoryDto>>> GetAllAsync(
        CancellationToken ct = default)
    {
        var categories =
            await _context.TicketCategories
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(
                    x => new TicketCategoryDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        IsActive = x.IsActive
                    })
                .ToListAsync(ct);

        return Result<List<TicketCategoryDto>>.Succeeded(
            categories);
    }

    // =========================================================
    // Update - Partial Update
    // =========================================================

    public async Task<Result<TicketCategoryDto>> UpdateAsync(
        int id,
        UpdateTicketCategoryDto dto,
        CancellationToken ct = default)
    {
        if (id <= 0)
        {
            return Result<TicketCategoryDto>.Failure(
                "Invalid category ID.");
        }

        // =====================================================
        // Get Entity
        // =====================================================

        var category =
            await _context.TicketCategories
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    ct);

        if (category is null)
        {
            return Result<TicketCategoryDto>.Failure(
                "Ticket category not found.");
        }

        // =====================================================
        // Old Values
        // =====================================================

        var oldValues = new
        {
            category.Name,
            category.Description,
            category.IsActive
        };

        // =====================================================
        // Partial Update - Name
        // =====================================================

        if (dto.Name is not null)
        {
            var name = dto.Name.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                return Result<TicketCategoryDto>.Failure(
                    "Category name cannot be empty.");
            }

            var duplicate =
                await _context.TicketCategories
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.Id != id &&
                            x.Name == name,
                        ct);

            if (duplicate)
            {
                return Result<TicketCategoryDto>.Failure(
                    "A ticket category with this name already exists.");
            }

            category.Name = name;
        }

        // =====================================================
        // Partial Update - Description
        // =====================================================

        if (dto.Description is not null)
        {
            category.Description =
                string.IsNullOrWhiteSpace(dto.Description)
                    ? null
                    : dto.Description.Trim();
        }

        // =====================================================
        // Partial Update - IsActive
        // =====================================================

        if (dto.IsActive.HasValue)
        {
            category.IsActive = dto.IsActive.Value;
        }

        // =====================================================
        // Save
        // =====================================================

        await _context.SaveChangesAsync(ct);

        // =====================================================
        // New Values
        // =====================================================

        var newValues = new
        {
            category.Name,
            category.Description,
            category.IsActive
        };

        // =====================================================
        // Audit
        // =====================================================

        await _auditService.LogAsync(
            AuditActions.Update,
            nameof(TicketCategory),
            category.Id.ToString(),
            oldValues,
            newValues);

        // =====================================================
        // Result
        // =====================================================

        return Result<TicketCategoryDto>.Succeeded(
            MapToDto(category),
            "Ticket category updated successfully.");
    }

    // =========================================================
    // Delete - Soft Delete
    // =========================================================

    public async Task<Result> DeleteAsync(
        int id,
        CancellationToken ct = default)
    {
        if (id <= 0)
        {
            return Result.Failure(
                "Invalid category ID.");
        }

        // =====================================================
        // Get Entity
        // =====================================================

        var category =
            await _context.TicketCategories
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    ct);

        if (category is null)
        {
            return Result.Failure(
                "Ticket category not found.");
        }

        // =====================================================
        // Check Ticket Usage
        // =====================================================

        var hasTickets =
            await _context.Tickets
                .AsNoTracking()
                .AnyAsync(
                    x => x.CategoryId == id,
                    ct);

        if (hasTickets)
        {
            return Result.Failure(
                "This category cannot be deleted because it is used by existing tickets.");
        }

        // =====================================================
        // Old Values
        // =====================================================

        var oldValues = new
        {
            category.Name,
            category.Description,
            category.IsActive
        };

        // =====================================================
        // Soft Delete
        // =====================================================

        category.IsDeleted = true;
        category.IsActive = false;

        await _context.SaveChangesAsync(ct);

        // =====================================================
        // Audit
        // =====================================================

        await _auditService.LogAsync(
            AuditActions.Delete,
            nameof(TicketCategory),
            category.Id.ToString(),
            oldValues,
            new
            {
                category.Name,
                category.Description,
                category.IsActive,
                category.IsDeleted
            });

        // =====================================================
        // Result
        // =====================================================

        return Result.Succeeded(
            "Ticket category deleted successfully.");
    }

    // =========================================================
    // Mapping
    // =========================================================

    private static TicketCategoryDto MapToDto(
        TicketCategory category)
    {
        return new TicketCategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive
        };
    }
}

