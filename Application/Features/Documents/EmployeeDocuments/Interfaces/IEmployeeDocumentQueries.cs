using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Documents.EmployeeDocuments.DTOs;

namespace MicroERP.Application.Features.Documents.EmployeeDocuments.Interfaces;

public interface IEmployeeDocumentQueries
{
    Task<Result<List<EmployeeDocumentDto>>> GetByEmployeeIdAsync(
        int employeeId,
        CancellationToken cancellationToken = default);

    Task<Result<EmployeeDocumentDto>> GetByIdAsync(
        int employeeId,
        int id,
        CancellationToken cancellationToken = default);
}