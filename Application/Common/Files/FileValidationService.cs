using MicroERP.Application.Common.Files.Interfaces;
using MicroERP.Application.Common.Models;
using Microsoft.AspNetCore.Http;

namespace MicroERP.Application.Common.Files;

public class FileValidationService : IFileValidationService
{
    private const int MaxSignatureLength = 16;


    public async Task<Result> ValidateAsync(
    IFormFile file,
    FileValidationOptions options,
    CancellationToken cancellationToken = default)
    {
        // =========================================================
        // Validate File
        // =========================================================

        if (file is null)
            return Result.Failure("File is required.");

        if (file.Length <= 0)
            return Result.Failure("File is empty.");

        if (options is null)
            return Result.Failure(
                "File validation configuration is missing.");


        // =========================================================
        // Validate Max File Size
        // =========================================================

        if (options.MaxFileSize <= 0)
            return Result.Failure(
                "Maximum file size is not configured correctly.");

        if (file.Length > options.MaxFileSize)
            return Result.Failure(
                $"File size exceeds the allowed limit of " +
                $"{FormatFileSize(options.MaxFileSize)}.");


        // =========================================================
        // Validate Extension
        // =========================================================

        var extension =
            Path.GetExtension(
                Path.GetFileName(file.FileName));

        if (string.IsNullOrWhiteSpace(extension))
            return Result.Failure(
                "File extension is required.");

        extension = extension.ToLowerInvariant();

        var allowedExtensions =
            options.AllowedExtensions
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x =>
                    x.Trim().StartsWith(".")
                        ? x.Trim().ToLowerInvariant()
                        : "." + x.Trim().ToLowerInvariant())
                .ToHashSet(
                    StringComparer.OrdinalIgnoreCase);

        if (!allowedExtensions.Contains(extension))
            return Result.Failure(
                "File extension is not allowed.");


        // =========================================================
        // Validate Content Type
        // =========================================================

        if (string.IsNullOrWhiteSpace(file.ContentType))
            return Result.Failure(
                "File content type is required.");

        var contentType =
            file.ContentType.Trim().ToLowerInvariant();

        Console.WriteLine(
            $"FILE: {file.FileName}");

        Console.WriteLine(
            $"CONTENT TYPE: [{contentType}]");

        Console.WriteLine(
            $"EXTENSION: [{extension}]");


        var allowedContentTypes =
            options.AllowedContentTypes
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x =>
                    x.Trim().ToLowerInvariant())
                .ToHashSet(
                    StringComparer.OrdinalIgnoreCase);


        // =========================================================
        // Content Type Validation
        // =========================================================
        //
        // application/octet-stream can be sent by some clients
        // such as Postman.
        //
        // We do NOT add it globally to appsettings.
        //
        // Instead, if the client sends octet-stream,
        // we continue to the file signature validation.
        //

        var contentTypeAllowed =
            allowedContentTypes.Contains(contentType);

        var genericBinaryContentType =
            contentType == "application/octet-stream";

        if (!contentTypeAllowed &&
            !genericBinaryContentType)
        {
            return Result.Failure(
                "File content type is not allowed.");
        }


        // =========================================================
        // Validate File Signature
        // =========================================================

        if (!FileSignatures.Signatures
            .TryGetValue(
                extension,
                out var signatures))
        {
            return Result.Failure(
                "File signature validation is not supported " +
                "for this file type.");
        }

        var signatureValid =
            await ValidateSignatureAsync(
                file,
                signatures,
                cancellationToken);

        if (!signatureValid)
        {
            return Result.Failure(
                "File content does not match its extension.");
        }


        // =========================================================
        // Success
        // =========================================================

        return Result.Succeeded(
            "File is valid.");
    }



    // =============================================================
    // Validate Signature
    // =============================================================

    private static async Task<bool> ValidateSignatureAsync(
        IFormFile file,
        byte[][] signatures,
        CancellationToken cancellationToken)
    {
        var maxSignatureLength =
            signatures.Max(x => x.Length);

        maxSignatureLength =
            Math.Min(
                maxSignatureLength,
                MaxSignatureLength);

        var buffer =
            new byte[maxSignatureLength];

        await using var stream =
            file.OpenReadStream();

        var totalRead = 0;

        while (totalRead < buffer.Length)
        {
            var read =
                await stream.ReadAsync(
                    buffer.AsMemory(totalRead),
                    cancellationToken);

            if (read == 0)
                break;

            totalRead += read;
        }

        foreach (var signature in signatures)
        {
            if (totalRead < signature.Length)
                continue;

            var matches = true;

            for (var i = 0;
                 i < signature.Length;
                 i++)
            {
                if (buffer[i] != signature[i])
                {
                    matches = false;
                    break;
                }
            }

            if (matches)
                return true;
        }

        return false;
    }

    // =============================================================
    // Format File Size
    // =============================================================

    private static string FormatFileSize(long bytes)
    {
        if (bytes < 1024)
            return $"{bytes} B";

        if (bytes < 1024 * 1024)
            return $"{bytes / 1024.0:F1} KB";

        if (bytes < 1024L * 1024 * 1024)
            return $"{bytes / (1024.0 * 1024):F1} MB";

        return $"{bytes / (1024.0 * 1024 * 1024):F1} GB";
    }
}