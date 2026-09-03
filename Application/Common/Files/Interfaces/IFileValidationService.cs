using MicroERP.Application.Common.Models;
using Microsoft.AspNetCore.Http;

namespace MicroERP.Application.Common.Files.Interfaces;

public interface IFileValidationService
{
    Task<Result> ValidateAsync(
          IFormFile file,
          FileValidationOptions options,
          CancellationToken cancellationToken = default);
}