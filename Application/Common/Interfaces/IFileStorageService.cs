using Microsoft.AspNetCore.Http;

namespace MicroERP.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(
        IFormFile file,
        string folder,
        CancellationToken cancellationToken = default);


    Task DeleteFileAsync(
        string filePath,
        CancellationToken cancellationToken = default);
}