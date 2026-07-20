using MicroERP.Application.Common;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Documents.EmployeeDocuments.DTOs;

namespace MicroERP.Application.Features.Documents.EmployeeDocuments.Interfaces;

public interface IEmployeeDocumentService
{
    Task<Result<EmployeeDocumentDto>> CreateAsync(
        int employeeId,
        CreateEmployeeDocumentDto dto,
        CancellationToken cancellationToken = default);


    Task<Result<EmployeeDocumentDto>> UpdateAsync(
        int id,
        UpdateEmployeeDocumentDto dto,
        CancellationToken cancellationToken = default);


    Task<Result> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);
}