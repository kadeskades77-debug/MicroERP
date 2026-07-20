using MicroERP.Application.Features.Documents.EmployeeDocuments.DTOs;

namespace MicroERP.Application.Features.Documents.EmployeeDocuments.Interfaces;

public interface IEmployeeDocumentQueries
{
    Task<List<EmployeeDocumentDto>> GetByEmployeeIdAsync(
       int employeeId,
       CancellationToken cancellationToken = default);


    Task<EmployeeDocumentDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);
}