using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.DTOs;

namespace MicroERP.Application.Features.Audit.Interfaces;

public interface IAuditService
{
    Task<Result<PagedResult<AuditLogDto>>> GetAllAsync(
        AuditLogFilterDto dto);

    Task<Result<AuditLogDetailsDto>> GetByIdAsync(int id);

    Task LogAsync(
    string action,
    string entityName,
    string? entityId = null,
    object? oldValues = null,
    object? newValues = null,
    CancellationToken cancellationToken = default);
}