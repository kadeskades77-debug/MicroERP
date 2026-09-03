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

public class TicketService : ITicketService
{
    private readonly IApplicationDbContext _context;
    private readonly ITicketNumberGenerator _ticketNumberGenerator;
    private readonly IAuditService _auditService;

    public TicketService(
        IApplicationDbContext context,
        ITicketNumberGenerator ticketNumberGenerator,
        IAuditService auditService)
    {
        _context = context;
        _ticketNumberGenerator = ticketNumberGenerator;
        _auditService = auditService;
    }

    // =========================================================
    // Create
    // =========================================================


public async Task<Result<TicketDto>> CreateAsync(
    CreateTicketDto dto,
    CancellationToken ct = default)
    {
        // =====================================================
        // Validate
        // =====================================================

        if (dto is null)
        {
            return Result<TicketDto>.Failure(
                "Request is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            return Result<TicketDto>.Failure(
                "Ticket title is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Description))
        {
            return Result<TicketDto>.Failure(
                "Ticket description is required.");
        }

        if (dto.CategoryId <= 0)
        {
            return Result<TicketDto>.Failure(
                "Ticket category is required.");
        }

        // =====================================================
        // Category
        // =====================================================

        var categoryExists =
            await _context.TicketCategories
                .AsNoTracking()
                .AnyAsync(
                    x =>
                        x.Id == dto.CategoryId &&
                        x.IsActive,
                    ct);

        if (!categoryExists)
        {
            return Result<TicketDto>.Failure(
                "Ticket category not found or inactive.");
        }

        // =====================================================
        // Generate Ticket Number
        // =====================================================

        var ticketNumber =
            await _ticketNumberGenerator.GenerateAsync(ct);

        // =====================================================
        // Create Ticket
        // =====================================================

        var ticket = new Ticket
        {
            TicketNumber = ticketNumber,

            Title = dto.Title.Trim(),

            Description = dto.Description.Trim(),

            Status = TicketStatus.Open,

            Priority = dto.Priority,

            CategoryId = dto.CategoryId,

            AssignedToEmployeeId =
                dto.AssignedToEmployeeId,

            AssignedDepartmentId =
                dto.AssignedDepartmentId,

            DueOn = dto.DueOn
        };

        await _context.Tickets.AddAsync(
            ticket,
            ct);

        // =====================================================
        // Create History
        // =====================================================

        await CreateHistoryAsync(
            ticket,
            TicketHistoryAction.Created,
            null,
            TicketStatus.Open.ToString(),
            "Ticket created.",
            ct);

        // =====================================================
        // Save Ticket + History
        // =====================================================

        await _context.SaveChangesAsync(ct);

        // =====================================================
        // Audit
        // =====================================================

        await _auditService.LogAsync(
            AuditActions.Create,
            nameof(Ticket),
            ticket.Id.ToString(),
            null,
            new
            {
                ticket.TicketNumber,
                ticket.Title,
                ticket.Description,
                ticket.Status,
                ticket.Priority,
                ticket.CategoryId,
                ticket.AssignedToEmployeeId,
                ticket.AssignedDepartmentId,
                ticket.DueOn
            });

        // =====================================================
        // Get Creator Name
        // =====================================================

        var createdByName =
            await _context.Users
                .AsNoTracking()
                .Where(x =>
                    x.Id == ticket.CreatedBy)
                .Select(x =>
                    x.FullName)
                .FirstOrDefaultAsync(ct);

        // =====================================================
        // Result
        // =====================================================

        return Result<TicketDto>.Succeeded(
            ticket.ToDto(createdByName),
            "Ticket created successfully.");
    }




    // =========================================================
    // Get By Id
    // =========================================================

    public async Task<Result<TicketDto>> GetByIdAsync(
     int id,
     CancellationToken ct = default)
    {
        // =====================================================
        // Validate
        // =====================================================

        if (id <= 0)
        {
            return Result<TicketDto>.Failure(
                "Invalid ticket ID.");
        }

        // =====================================================
        // Get Ticket
        // =====================================================

        var ticket =
            await _context.Tickets
                .AsNoTracking()
                .Include(x => x.Category)
                .Include(x => x.AssignedToEmployee)
                    .ThenInclude(x => x.User)
                .Include(x => x.AssignedDepartment)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    ct);

        // =====================================================
        // Validate Ticket
        // =====================================================

        if (ticket is null)
        {
            return Result<TicketDto>.Failure(
                "Ticket not found.");
        }

        // =====================================================
        // Get Creator Name
        // =====================================================

        var createdByName =
            await _context.Users
                .AsNoTracking()
                .Where(
                    x =>
                        x.Id == ticket.CreatedBy)
                .Select(
                    x =>
                        x.FullName)
                .FirstOrDefaultAsync(ct);

        // =====================================================
        // Result
        // =====================================================

        return Result<TicketDto>.Succeeded(
            ticket.ToDto(createdByName),
            "Ticket retrieved successfully.");
    }

    // =========================================================
    // Get All
    // =========================================================


    public async Task<Result<List<TicketDto>>> GetAllAsync(
    TicketFilterDto filter,
    CancellationToken ct = default)
    {
        // =========================================================
        // Validate Filter
        // =========================================================

        if (filter is null)
        {
            return Result<List<TicketDto>>.Failure(
                "Filter is required.");
        }

        if (filter.PageNumber < 1)
        {
            return Result<List<TicketDto>>.Failure(
                "Page number must be greater than zero.");
        }

        if (filter.PageSize < 1)
        {
            return Result<List<TicketDto>>.Failure(
                "Page size must be greater than zero.");
        }

        if (filter.PageSize > 100)
        {
            return Result<List<TicketDto>>.Failure(
                "Page size cannot exceed 100.");
        }

        if (filter.FromDate.HasValue &&
            filter.ToDate.HasValue &&
            filter.FromDate.Value > filter.ToDate.Value)
        {
            return Result<List<TicketDto>>.Failure(
                "From date cannot be greater than To date.");
        }

        // =========================================================
        // Query
        // =========================================================

        var query =
            _context.Tickets
                .AsNoTracking()
                .AsQueryable();

        // =========================================================
        // Search
        // =========================================================

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search =
                filter.Search.Trim();

            query =
                query.Where(
                    x =>
                        x.TicketNumber.Contains(search) ||
                        x.Title.Contains(search) ||
                        x.Description.Contains(search));
        }

        // =========================================================
        // Status
        // =========================================================

        if (filter.Status.HasValue)
        {
            if (!Enum.IsDefined(
                    typeof(TicketStatus),
                    filter.Status.Value))
            {
                return Result<List<TicketDto>>.Failure(
                    "Invalid ticket status.");
            }

            query =
                query.Where(
                    x =>
                        (int)x.Status ==
                        filter.Status.Value);
        }

        // =========================================================
        // Priority
        // =========================================================

        if (filter.Priority.HasValue)
        {
            if (!Enum.IsDefined(
                    typeof(TicketPriority),
                    filter.Priority.Value))
            {
                return Result<List<TicketDto>>.Failure(
                    "Invalid ticket priority.");
            }

            query =
                query.Where(
                    x =>
                        (int)x.Priority ==
                        filter.Priority.Value);
        }

        // =========================================================
        // Category
        // =========================================================

        if (filter.CategoryId.HasValue)
        {
            if (filter.CategoryId.Value <= 0)
            {
                return Result<List<TicketDto>>.Failure(
                    "Invalid category ID.");
            }

            query =
                query.Where(
                    x =>
                        x.CategoryId ==
                        filter.CategoryId.Value);
        }

        // =========================================================
        // Assigned Employee
        // =========================================================

        if (filter.AssignedToEmployeeId.HasValue)
        {
            if (filter.AssignedToEmployeeId.Value <= 0)
            {
                return Result<List<TicketDto>>.Failure(
                    "Invalid assigned employee ID.");
            }

            query =
                query.Where(
                    x =>
                        x.AssignedToEmployeeId ==
                        filter.AssignedToEmployeeId.Value);
        }

        // =========================================================
        // Assigned Department
        // =========================================================

        if (filter.AssignedDepartmentId.HasValue)
        {
            if (filter.AssignedDepartmentId.Value <= 0)
            {
                return Result<List<TicketDto>>.Failure(
                    "Invalid assigned department ID.");
            }

            query =
                query.Where(
                    x =>
                        x.AssignedDepartmentId ==
                        filter.AssignedDepartmentId.Value);
        }

        // =========================================================
        // From Date
        // =========================================================

        if (filter.FromDate.HasValue)
        {
            query =
                query.Where(
                    x =>
                        x.CreatedOn >=
                        filter.FromDate.Value);
        }

        // =========================================================
        // To Date
        // =========================================================

        if (filter.ToDate.HasValue)
        {
            query =
                query.Where(
                    x =>
                        x.CreatedOn <=
                        filter.ToDate.Value);
        }

        // =========================================================
        // Pagination
        // =========================================================

        var skip =
            (filter.PageNumber - 1) *
            filter.PageSize;

        // =========================================================
        // Projection
        // =========================================================

        var tickets =
            await query
                .OrderByDescending(x => x.CreatedOn)
                .ThenByDescending(x => x.Id)
                .Select(
                    x => new TicketDto
                    {
                        Id = x.Id,

                        TicketNumber =
                            x.TicketNumber,

                        Title =
                            x.Title,

                        Description =
                            x.Description,

                        Status =
                            x.Status,

                        Priority =
                            x.Priority,

                        CategoryId =
                            x.CategoryId,

                        CategoryName =
                            x.Category.Name,
                        CreatedByName =
                  _context.Users
                      .Where(u => u.Id == x.CreatedBy)
                      .Select(u => u.FullName)
                      .FirstOrDefault(),

                        AssignedToEmployeeId =
                            x.AssignedToEmployeeId,

                        AssignedToEmployeeName =
                            x.AssignedToEmployee != null
                                ? x.AssignedToEmployee.User.UserName
                                : null,

                        AssignedDepartmentId =
                            x.AssignedDepartmentId,

                        AssignedDepartmentName =
                            x.AssignedDepartment != null
                                ? x.AssignedDepartment.NameEn
                                : null,

                        ResolvedOn =
                            x.ResolvedOn,

                        ClosedOn =
                            x.ClosedOn,

                        DueOn =
                            x.DueOn,

                        CreatedOn =
                            x.CreatedOn
                    })
                .Skip(skip)
                .Take(filter.PageSize)
                .ToListAsync(ct);

        // =========================================================
        // Result
        // =========================================================

        return Result<List<TicketDto>>.Succeeded(
            tickets);
    }


    // =========================================================
    // Update - Partial Update
    // =========================================================

   
   public async Task<Result<TicketDto>> UpdateAsync(
    int id,
    UpdateTicketDto dto,
    CancellationToken ct = default)
    {
        // =========================================================
        // Validate ID
        // =========================================================

        if (id <= 0)
        {
            return Result<TicketDto>.Failure(
                "Invalid ticket ID.");
        }

        // =========================================================
        // Get Ticket
        // =========================================================

        var ticket =
            await _context.Tickets
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    ct);

        if (ticket is null)
        {
            return Result<TicketDto>.Failure(
                "Ticket not found.");
        }

        // =========================================================
        // Closed Ticket
        // =========================================================

        if (ticket.Status == TicketStatus.Closed)
        {
            return Result<TicketDto>.Failure(
                "Closed tickets cannot be modified.");
        }

        // =========================================================
        // Old Values
        // =========================================================

        var oldValues = new
        {
            ticket.Title,
            ticket.Description,
            ticket.CategoryId,
            ticket.Priority,
            ticket.DueOn
        };

        var hasChanges = false;

        // =========================================================
        // Title
        // =========================================================

        if (dto.Title is not null)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                return Result<TicketDto>.Failure(
                    "Ticket title cannot be empty.");
            }

            var title = dto.Title.Trim();

            if (!string.Equals(
                    ticket.Title,
                    title,
                    StringComparison.Ordinal))
            {
                ticket.Title = title;

                hasChanges = true;
            }
        }

        // =========================================================
        // Description
        // =========================================================

        if (dto.Description is not null)
        {
            if (string.IsNullOrWhiteSpace(dto.Description))
            {
                return Result<TicketDto>.Failure(
                    "Ticket description cannot be empty.");
            }

            var description =
                dto.Description.Trim();

            if (!string.Equals(
                    ticket.Description,
                    description,
                    StringComparison.Ordinal))
            {
                ticket.Description = description;

                hasChanges = true;
            }
        }

        // =========================================================
        // Category
        // =========================================================

        if (dto.CategoryId.HasValue)
        {
            if (dto.CategoryId.Value <= 0)
            {
                return Result<TicketDto>.Failure(
                    "Invalid ticket category ID.");
            }

            if (ticket.CategoryId != dto.CategoryId.Value)
            {
                var categoryExists =
                    await _context.TicketCategories
                        .AsNoTracking()
                        .AnyAsync(
                            x =>
                                x.Id == dto.CategoryId.Value &&
                                x.IsActive,
                            ct);

                if (!categoryExists)
                {
                    return Result<TicketDto>.Failure(
                        "Ticket category not found or inactive.");
                }

                ticket.CategoryId =
                    dto.CategoryId.Value;

                hasChanges = true;
            }
        }

        // =========================================================
        // Priority
        // =========================================================

        if (dto.Priority.HasValue)
        {
            if (!Enum.IsDefined(
                    typeof(TicketPriority),
                    dto.Priority.Value))
            {
                return Result<TicketDto>.Failure(
                    "Invalid ticket priority.");
            }

            if (ticket.Priority != dto.Priority.Value)
            {
                ticket.Priority =
                    dto.Priority.Value;

                hasChanges = true;
            }
        }

        // =========================================================
        // Due Date
        // =========================================================

        if (dto.DueOn.HasValue)
        {
            if (ticket.DueOn != dto.DueOn.Value)
            {
                ticket.DueOn =
                    dto.DueOn.Value;

                hasChanges = true;
            }
        }

        // =========================================================
        // No Changes
        // =========================================================

        if (!hasChanges)
        {
            return Result<TicketDto>.Failure(
                "No changes were provided.");
        }

        // =========================================================
        // Save
        // =========================================================

        await _context.SaveChangesAsync(ct);

        // =========================================================
        // New Values
        // =========================================================

        var newValues = new
        {
            ticket.Title,
            ticket.Description,
            ticket.CategoryId,
            ticket.Priority,
            ticket.DueOn
        };

        // =========================================================
        // Audit
        // =========================================================

        await _auditService.LogAsync(
            AuditActions.Update,
            nameof(Ticket),
            ticket.Id.ToString(),
            oldValues,
            newValues);
        // =====================================================
        // Get Creator Name
        // =====================================================

        var createdByName =
            await _context.Users
                .AsNoTracking()
                .Where(
                    x =>
                        x.Id == ticket.CreatedBy)
                .Select(
                    x =>
                        x.FullName)
                .FirstOrDefaultAsync(ct);

        // =====================================================
        // Result
        // =====================================================

        return Result<TicketDto>.Succeeded(
            ticket.ToDto(createdByName),
            "Ticket retrieved successfully.");
    }



    // =========================================================
    // Change Status
    // =========================================================

  
  public async Task<Result<TicketDto>> ChangeStatusAsync(
    ChangeTicketStatusDto dto,
    CancellationToken ct = default)
    {
        // =========================================================
        // Validate
        // =========================================================

        if (dto is null)
        {
            return Result<TicketDto>.Failure(
                "Request is required.");
        }

        if (dto.TicketId <= 0)
        {
            return Result<TicketDto>.Failure(
                "Invalid ticket ID.");
        }

        if (!Enum.IsDefined(
                typeof(TicketStatus),
                dto.Status))
        {
            return Result<TicketDto>.Failure(
                "Invalid ticket status.");
        }

        // =========================================================
        // Get Ticket
        // =========================================================

        var ticket =
            await _context.Tickets
                .FirstOrDefaultAsync(
                    x => x.Id == dto.TicketId,
                    ct);

        if (ticket is null)
        {
            return Result<TicketDto>.Failure(
                "Ticket not found.");
        }

        // =========================================================
        // Closed Ticket
        // =========================================================

        if (ticket.Status == TicketStatus.Closed)
        {
            return Result<TicketDto>.Failure(
                "Closed tickets cannot be modified.");
        }

        // =========================================================
        // Old Status
        // =========================================================

        var oldStatus =
            ticket.Status;

        // =========================================================
        // Same Status
        // =========================================================

        if (oldStatus == dto.Status)
        {
            return Result<TicketDto>.Failure(
                "Ticket is already in this status.");
        }

        // =========================================================
        // Validate Transition
        // =========================================================

        if (!IsValidStatusTransition(
                oldStatus,
                dto.Status))
        {
            return Result<TicketDto>.Failure(
                $"Cannot change ticket status from " +
                $"{oldStatus} to {dto.Status}.");
        }

        // =========================================================
        // Status Rules
        // =========================================================

        var now = DateTime.UtcNow;

        switch (dto.Status)
        {
            case TicketStatus.Resolved:

                ticket.ResolvedOn = now;

                break;

            case TicketStatus.Closed:

                if (!ticket.ResolvedOn.HasValue)
                {
                    return Result<TicketDto>.Failure(
                        "Ticket must be resolved before it can be closed.");
                }

                ticket.ClosedOn = now;

                break;

            case TicketStatus.Open:

                ticket.ResolvedOn = null;
                ticket.ClosedOn = null;

                break;

            case TicketStatus.InProgress:

                break;
        }

        // =========================================================
        // Update Status
        // =========================================================

        ticket.Status =
            dto.Status;

        // =========================================================
        // History Action
        // =========================================================

        var historyAction =
            dto.Status switch
            {
                TicketStatus.Resolved =>
                    TicketHistoryAction.Resolved,

                TicketStatus.Closed =>
                    TicketHistoryAction.Closed,

                _ =>
                    TicketHistoryAction.StatusChanged
            };

        // =========================================================
        // Create History
        // =========================================================

        await CreateHistoryAsync(
            ticket,
            historyAction,
            oldStatus.ToString(),
            ticket.Status.ToString(),
            dto.Notes,
            ct);

        // =========================================================
        // Save
        // =========================================================

        await _context.SaveChangesAsync(ct);

        // =========================================================
        // Audit
        // =========================================================

        await _auditService.LogAsync(
            AuditActions.Update,
            nameof(Ticket),
            ticket.Id.ToString(),
            new
            {
                Status = oldStatus.ToString()
            },
            new
            {
                Status = ticket.Status.ToString(),
                Notes = dto.Notes
            });

        // =========================================================
        // Get Updated Ticket
        // =========================================================

        var updatedTicket =
            await _context.Tickets
                .AsNoTracking()
                .Include(x => x.Category)
                .Include(x => x.AssignedToEmployee)
                    .ThenInclude(x => x.User)
                .Include(x => x.AssignedDepartment)
                .FirstOrDefaultAsync(
                    x => x.Id == ticket.Id,
                    ct);

        if (updatedTicket is null)
        {
            return Result<TicketDto>.Failure(
                "Ticket could not be retrieved after status change.");
        }

        // =========================================================
        // Get Creator Name
        // =========================================================

        var createdByName =
            await _context.Users
                .AsNoTracking()
                .Where(x => x.Id == updatedTicket.CreatedBy)
                .Select(x => x.FullName)
                .FirstOrDefaultAsync(ct);

        // =========================================================
        // Result
        // =========================================================

        return Result<TicketDto>.Succeeded(
            updatedTicket.ToDto(createdByName),
            "Ticket status changed successfully.");
    }


    // =========================================================
    // Change Priority
    // =========================================================

public async Task<Result<TicketDto>> ChangePriorityAsync(
    ChangeTicketPriorityDto dto,
    CancellationToken ct = default)
    {
        // =========================================================
        // Validate
        // =========================================================

        if (dto is null)
        {
            return Result<TicketDto>.Failure(
                "Request is required.");
        }

        if (dto.TicketId <= 0)
        {
            return Result<TicketDto>.Failure(
                "Invalid ticket ID.");
        }

        if (!Enum.IsDefined(
                typeof(TicketPriority),
                dto.Priority))
        {
            return Result<TicketDto>.Failure(
                "Invalid ticket priority.");
        }

        // =========================================================
        // Get Ticket
        // =========================================================

        var ticket =
            await _context.Tickets
                .FirstOrDefaultAsync(
                    x => x.Id == dto.TicketId,
                    ct);

        if (ticket is null)
        {
            return Result<TicketDto>.Failure(
                "Ticket not found.");
        }

        // =========================================================
        // Closed Ticket
        // =========================================================

        if (ticket.Status == TicketStatus.Closed)
        {
            return Result<TicketDto>.Failure(
                "Closed tickets cannot be modified.");
        }

        // =========================================================
        // Cancelled Ticket
        // =========================================================

        if (ticket.Status == TicketStatus.Cancelled)
        {
            return Result<TicketDto>.Failure(
                "Cancelled tickets cannot be modified.");
        }

        // =========================================================
        // Same Priority
        // =========================================================

        if (ticket.Priority == dto.Priority)
        {
            return Result<TicketDto>.Failure(
                "Ticket is already using this priority.");
        }

        // =========================================================
        // Old Priority
        // =========================================================

        var oldPriority =
            ticket.Priority;

        // =========================================================
        // Update Priority
        // =========================================================

        ticket.Priority =
            dto.Priority;

        // =========================================================
        // Create History
        // =========================================================

        await CreateHistoryAsync(
            ticket,
            TicketHistoryAction.PriorityChanged,
            oldPriority.ToString(),
            ticket.Priority.ToString(),
            dto.Notes,
            ct);

        // =========================================================
        // Save Changes
        // =========================================================

        await _context.SaveChangesAsync(ct);

        // =========================================================
        // Audit
        // =========================================================

        await _auditService.LogAsync(
            AuditActions.Update,
            nameof(Ticket),
            ticket.Id.ToString(),
            new
            {
                Priority =
                    oldPriority.ToString()
            },
            new
            {
                Priority =
                    ticket.Priority.ToString(),

                Notes =
                    dto.Notes
            });

        // =========================================================
        // Get Creator Name
        // =========================================================

        var createdByName =
            await _context.Users
                .AsNoTracking()
                .Where(
                    x =>
                        x.Id == ticket.CreatedBy)
                .Select(
                    x =>
                        x.FullName)
                .FirstOrDefaultAsync(ct);

        // =========================================================
        // Result
        // =========================================================

        return Result<TicketDto>.Succeeded(
            ticket.ToDto(createdByName),
            "Ticket priority changed successfully.");
    }





    // =========================================================
    // Start Ticket
    // =========================================================

    public async Task<Result<TicketDto>> StartAsync(
        TicketActionDto dto,
        CancellationToken ct = default)
    {
        // =====================================================
        // Validate Request
        // =====================================================

        if (dto is null)
        {
            return Result<TicketDto>.Failure(
                "Request is required.");
        }

        if (dto.TicketId <= 0)
        {
            return Result<TicketDto>.Failure(
                "Invalid ticket ID.");
        }

        // =====================================================
        // Change Status
        // =====================================================

        return await ChangeStatusAsync(
            new ChangeTicketStatusDto
            {
                TicketId = dto.TicketId,
                Status = TicketStatus.InProgress,
                Notes = dto.Notes
            },
            ct);
    }


    // =========================================================
    // Resolve Ticket
    // =========================================================

    public async Task<Result<TicketDto>> ResolveAsync(
         TicketActionDto dto,
         CancellationToken ct = default)
    {
        if (dto is null)
        {
            return Result<TicketDto>.Failure(
                "Request is required.");
        }

        if (dto.TicketId <= 0)
        {
            return Result<TicketDto>.Failure(
                "Invalid ticket ID.");
        }

        return await ChangeStatusAsync(
            new ChangeTicketStatusDto
            {
                TicketId = dto.TicketId,
                Status = TicketStatus.Resolved,
                Notes = dto.Notes
            },
            ct);
    }
     
     
     
     
     // =========================================================
     // Close Ticket
     // =========================================================
     
     public async Task<Result<TicketDto>> CloseAsync(
         TicketActionDto dto,
         CancellationToken ct = default)
    {
        if (dto is null)
        {
            return Result<TicketDto>.Failure(
                "Request is required.");
        }

        if (dto.TicketId <= 0)
        {
            return Result<TicketDto>.Failure(
                "Invalid ticket ID.");
        }

        return await ChangeStatusAsync(
            new ChangeTicketStatusDto
            {
                TicketId = dto.TicketId,
                Status = TicketStatus.Closed,
                Notes = dto.Notes
            },
            ct);
    }


    // =========================================================
    // Reopen Ticket
    // =========================================================


   public async Task<Result<TicketDto>> ReopenAsync(
    TicketActionDto dto,
    CancellationToken ct = default)
    {
        // =========================================================
        // Validate
        // =========================================================

        if (dto is null)
        {
            return Result<TicketDto>.Failure(
                "Request is required.");
        }

        if (dto.TicketId <= 0)
        {
            return Result<TicketDto>.Failure(
                "Invalid ticket ID.");
        }

        // =========================================================
        // Get Ticket
        // =========================================================

        var ticket =
            await _context.Tickets
                .FirstOrDefaultAsync(
                    x => x.Id == dto.TicketId,
                    ct);

        if (ticket is null)
        {
            return Result<TicketDto>.Failure(
                "Ticket not found.");
        }

        // =========================================================
        // Validate Status
        // =========================================================

        if (ticket.Status != TicketStatus.Resolved &&
            ticket.Status != TicketStatus.Closed)
        {
            return Result<TicketDto>.Failure(
                "Only resolved or closed tickets can be reopened.");
        }

        // =========================================================
        // Validate Assignment
        // =========================================================

        if (!ticket.AssignedToEmployeeId.HasValue)
        {
            return Result<TicketDto>.Failure(
                "Ticket cannot be reopened because it has no assigned employee.");
        }

        // =========================================================
        // Store Previous Values
        // =========================================================

        var oldStatus =
            ticket.Status;

        var employeeId =
            ticket.AssignedToEmployeeId.Value;

        var departmentId =
            ticket.AssignedDepartmentId;

        // =========================================================
        // Begin Transaction
        // =========================================================

        await using var transaction =
            await _context.Database.BeginTransactionAsync(ct);

        try
        {
            // =====================================================
            // Step 1 - Reopen Ticket
            // =====================================================

            ticket.Status =
                TicketStatus.Open;

            ticket.ResolvedOn = null;
            ticket.ClosedOn = null;


            // =====================================================
            // History - Reopened
            // =====================================================

            await CreateHistoryAsync(
                ticket,
                TicketHistoryAction.Reopened,
                oldStatus.ToString(),
                TicketStatus.Open.ToString(),
                dto.Notes,
                ct);


            // =====================================================
            // Step 2 - Assign Ticket Again
            // =====================================================

            ticket.Status =
                TicketStatus.Assigned;


            // =====================================================
            // Create New Assignment
            // =====================================================

            var assignment =
                new TicketAssignment
                {
                    TicketId = ticket.Id,
                    EmployeeId = employeeId,
                    DepartmentId = departmentId,
                    IsCurrent = true
                };

            await _context.TicketAssignments.AddAsync(
                assignment,
                ct);


            // =====================================================
            // History - Assigned
            // =====================================================

            await CreateHistoryAsync(
                ticket,
                TicketHistoryAction.Assigned,
                null,
                employeeId.ToString(),
                "Ticket reassigned automatically after reopening.",
                ct);


            // =====================================================
            // Save All Changes
            // =====================================================

            await _context.SaveChangesAsync(ct);


            // =====================================================
            // Commit Transaction
            // =====================================================

            await transaction.CommitAsync(ct);
        }
        catch
        {
            // =====================================================
            // Rollback
            // =====================================================

            await transaction.RollbackAsync(ct);

            throw;
        }

        // =========================================================
        // Audit
        // =========================================================

        await _auditService.LogAsync(
            AuditActions.Update,
            nameof(Ticket),
            ticket.Id.ToString(),
            new
            {
                Status = oldStatus.ToString()
            },
            new
            {
                Status = ticket.Status.ToString(),
                AssignedToEmployeeId = employeeId,
                AssignedDepartmentId = departmentId,
                Notes = dto.Notes
            },
            ct);


        // =========================================================
        // Get Creator Name
        // =========================================================

        var createdByName =
            await _context.Users
                .AsNoTracking()
                .Where(
                    x =>
                        x.Id == ticket.CreatedBy)
                .Select(
                    x =>
                        x.FullName)
                .FirstOrDefaultAsync(ct);


        // =========================================================
        // Get Updated Ticket
        // =========================================================

        var updatedTicket =
            await _context.Tickets
                .AsNoTracking()
                .Include(x => x.Category)
                .Include(x => x.AssignedToEmployee)
                    .ThenInclude(x => x.User)
                .Include(x => x.AssignedDepartment)
                .FirstOrDefaultAsync(
                    x => x.Id == ticket.Id,
                    ct);

        if (updatedTicket is null)
        {
            return Result<TicketDto>.Failure(
                "Ticket could not be retrieved after reopening.");
        }


        // =========================================================
        // Result
        // =========================================================

        return Result<TicketDto>.Succeeded(
            updatedTicket.ToDto(createdByName),
            "Ticket reopened and reassigned successfully.");
    }






    // =========================================================
    // Delete Ticket
    // =========================================================

    public async Task<Result> DeleteAsync(
        int id,
        CancellationToken ct = default)
    {
        // =========================================================
        // Validate ID
        // =========================================================

        if (id <= 0)
        {
            return Result.Failure(
                "Invalid ticket ID.");
        }

        // =========================================================
        // Get Ticket
        // =========================================================

        var ticket =
            await _context.Tickets
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    ct);

        if (ticket is null)
        {
            return Result.Failure(
                "Ticket not found.");
        }

        // =========================================================
        // Old Values
        // =========================================================

        var oldValues = new
        {
            ticket.TicketNumber,
            ticket.Title,
            Status = ticket.Status.ToString(),
            Priority = ticket.Priority.ToString(),
            ticket.IsActive,
            ticket.IsDeleted
        };

        // =========================================================
        // Soft Delete
        // =========================================================

        ticket.IsDeleted = true;
        ticket.IsActive = false;

        await _context.SaveChangesAsync(ct);

        // =========================================================
        // Audit
        // =========================================================

        await _auditService.LogAsync(
            AuditActions.Delete,
            nameof(Ticket),
            ticket.Id.ToString(),
            oldValues,
            new
            {
                ticket.TicketNumber,
                ticket.Title,
                Status = ticket.Status.ToString(),
                Priority = ticket.Priority.ToString(),
                ticket.IsActive,
                ticket.IsDeleted
            });

        // =========================================================
        // Result
        // =========================================================

        return Result.Succeeded(
            "Ticket deleted successfully.");
    }



    // =========================================================
    // Status Transition Rules
    // =========================================================

    private static bool IsValidStatusTransition(
      TicketStatus currentStatus,
      TicketStatus newStatus)
    {
        return currentStatus switch
        {
            // =====================================================
            // Open
            // =====================================================

            TicketStatus.Open =>
                newStatus == TicketStatus.Cancelled,


            // =====================================================
            // Assigned
            // =====================================================

            TicketStatus.Assigned =>
                newStatus == TicketStatus.InProgress ||
                newStatus == TicketStatus.Cancelled,


            // =====================================================
            // In Progress
            // =====================================================

            TicketStatus.InProgress =>
                newStatus == TicketStatus.Pending ||
                newStatus == TicketStatus.Resolved ||
                newStatus == TicketStatus.Cancelled,


            // =====================================================
            // Pending
            // =====================================================

            TicketStatus.Pending =>
                newStatus == TicketStatus.InProgress ||
                newStatus == TicketStatus.Cancelled,


            // =====================================================
            // Resolved
            // =====================================================

            TicketStatus.Resolved =>
                newStatus == TicketStatus.Closed ||
                newStatus == TicketStatus.InProgress,


            // =====================================================
            // Closed
            // =====================================================

            TicketStatus.Closed =>
                false,


            // =====================================================
            // Cancelled
            // =====================================================

            TicketStatus.Cancelled =>
                false,


            _ => false
        };
    }

// =====================================================
// Create Ticket History
// =====================================================

  private async Task CreateHistoryAsync(
    Ticket ticket,
    TicketHistoryAction action,
    string? oldValue = null,
    string? newValue = null,
    string? notes = null,
    CancellationToken ct = default)
    {
        if (ticket is null)
            throw new ArgumentNullException(nameof(ticket));

        var history = new TicketHistory
        {
          
            Ticket = ticket,

            Action = action,

            OldValue = oldValue,

            NewValue = newValue,

            Notes = notes
        };

        await _context.TicketHistories.AddAsync(
            history,
            ct);
    }




}

