using MicroERP.Application.Common.Models;
using Microsoft.AspNetCore.Http;

namespace MicroERP.Application.Common.Files.Interfaces;

public interface IFileStorageService
{
    Task<Result<string>> SaveFileAsync(
        IFormFile file,
        string folder,
        CancellationToken cancellationToken = default);


    Task<Result>DeleteFileAsync(
        string filePath,
        CancellationToken cancellationToken = default);
}