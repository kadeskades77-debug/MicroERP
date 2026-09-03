using MicroERP.Application.Common.Files.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Infrastructure.Services.Files;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace MicroERP.Infrastructure.Services.Files;


public class FileStorageService : IFileStorageService
{
    private readonly string _storagePath;
    private readonly string _storageRoot;
    private readonly FileStorageOptions _options;

    public FileStorageService(
        IWebHostEnvironment environment,
        IOptions<FileStorageOptions> options)
    {
        // =========================================================
        // Validate Dependencies
        // =========================================================

        ArgumentNullException.ThrowIfNull(environment);
        ArgumentNullException.ThrowIfNull(options);

        _options = options.Value
            ?? throw new InvalidOperationException(
                "File storage options are not configured.");

        // =========================================================
        // Validate Configuration
        // =========================================================

        if (string.IsNullOrWhiteSpace(
                _options.RootPath))
        {
            throw new InvalidOperationException(
                "File storage root path is not configured.");
        }

        if (_options.MaxFileSize <= 0)
        {
            throw new InvalidOperationException(
                "File storage maximum file size " +
                "must be greater than zero.");
        }

        // =========================================================
        // Build Storage Path
        // =========================================================

        _storagePath =
            Path.GetFullPath(
                Path.Combine(
                    environment.ContentRootPath,
                    _options.RootPath));

        // =========================================================
        // Normalize Storage Root
        // =========================================================

        _storageRoot =
            EnsureTrailingSeparator(
                _storagePath);

        // =========================================================
        // Ensure Storage Directory Exists
        // =========================================================

        Directory.CreateDirectory(
            _storagePath);
    }

    // =========================================================
    // Save File
    // =========================================================

    public async Task<Result<string>> SaveFileAsync(
        IFormFile file,
        string folder,
        CancellationToken cancellationToken = default)
    {
        // =========================================================
        // Validate File
        // =========================================================

        if (file is null)
        {
            return Result<string>.Failure(
                "File is required.");
        }

        if (file.Length <= 0)
        {
            return Result<string>.Failure(
                "File is empty.");
        }

        // =========================================================
        // Validate File Size
        // =========================================================

        if (file.Length > _options.MaxFileSize)
        {
            return Result<string>.Failure(
                $"File size exceeds the allowed limit of " +
                $"{FormatFileSize(_options.MaxFileSize)}.");
        }

        // =========================================================
        // Validate Folder
        // =========================================================

        if (string.IsNullOrWhiteSpace(folder))
        {
            return Result<string>.Failure(
                "Storage folder is required.");
        }

        // =========================================================
        // Validate Cancellation
        // =========================================================

        cancellationToken.ThrowIfCancellationRequested();

        // =========================================================
        // Normalize Folder
        // =========================================================

        var normalizedFolder =
            NormalizeRelativePath(folder);

        if (string.IsNullOrWhiteSpace(
                normalizedFolder))
        {
            return Result<string>.Failure(
                "Storage folder is invalid.");
        }

        // =========================================================
        // Prevent Path Traversal
        // =========================================================

        if (ContainsParentTraversal(
                normalizedFolder))
        {
            return Result<string>.Failure(
                "Invalid storage folder.");
        }

        // =========================================================
        // Get Extension
        // =========================================================

        var extension =
            Path.GetExtension(
                Path.GetFileName(file.FileName));

        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = string.Empty;
        }
        else
        {
            extension =
                extension.ToLowerInvariant();
        }

        // =========================================================
        // Generate Safe File Name
        // =========================================================

        var storedFileName =
            $"{Guid.NewGuid():N}{extension}";

        // =========================================================
        // Build Folder Path
        // =========================================================

        string fullFolderPath;

        try
        {
            fullFolderPath =
                GetSafeFullPath(
                    normalizedFolder);
        }
        catch
        {
            return Result<string>.Failure(
                "Invalid storage folder.");
        }

        // =========================================================
        // Create Folder
        // =========================================================

        try
        {
            Directory.CreateDirectory(
                fullFolderPath);
        }
        catch (UnauthorizedAccessException)
        {
            return Result<string>.Failure(
                "Unable to access storage folder.");
        }
        catch (PathTooLongException)
        {
            return Result<string>.Failure(
                "Storage path is too long.");
        }
        catch (DirectoryNotFoundException)
        {
            return Result<string>.Failure(
                "Storage folder was not found.");
        }
        catch (IOException)
        {
            return Result<string>.Failure(
                "Unable to create storage folder.");
        }

        // =========================================================
        // Build File Path
        // =========================================================

        string fullPath;

        try
        {
            fullPath =
                Path.Combine(
                    fullFolderPath,
                    storedFileName);

            fullPath =
                Path.GetFullPath(fullPath);
        }
        catch
        {
            return Result<string>.Failure(
                "Unable to create file path.");
        }

        // =========================================================
        // Final Security Check
        // =========================================================

        if (!IsInsideStorage(fullPath))
        {
            return Result<string>.Failure(
                "Invalid file path.");
        }

        // =========================================================
        // Save File
        // =========================================================

        try
        {
            await using var stream =
                new FileStream(
                    fullPath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: 64 * 1024,
                    useAsync: true);

            await file.CopyToAsync(
                stream,
                cancellationToken);

            await stream.FlushAsync(
                cancellationToken);
        }
        catch (OperationCanceledException)
        {
            DeletePhysicalFile(fullPath);

            throw;
        }
        catch (UnauthorizedAccessException)
        {
            DeletePhysicalFile(fullPath);

            return Result<string>.Failure(
                "Unable to access storage file.");
        }
        catch (PathTooLongException)
        {
            DeletePhysicalFile(fullPath);

            return Result<string>.Failure(
                "File path is too long.");
        }
        catch (DirectoryNotFoundException)
        {
            DeletePhysicalFile(fullPath);

            return Result<string>.Failure(
                "Storage directory was not found.");
        }
        catch (IOException)
        {
            DeletePhysicalFile(fullPath);

            return Result<string>.Failure(
                "Unable to save file.");
        }

        // =========================================================
        // Return Relative Path
        // =========================================================

        var relativePath =
            $"{normalizedFolder}/{storedFileName}";

        return Result<string>.Succeeded(
            relativePath,
            "File saved successfully.");
    }

    // =========================================================
    // Delete File
    // =========================================================

    public Task<Result> DeleteFileAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        // =========================================================
        // Validate Path
        // =========================================================

        if (string.IsNullOrWhiteSpace(filePath))
        {
            return Task.FromResult(
                Result.Failure(
                    "File path is required."));
        }

        // =========================================================
        // Validate Cancellation
        // =========================================================

        cancellationToken.ThrowIfCancellationRequested();

        // =========================================================
        // Normalize Path
        // =========================================================

        var normalizedPath =
            NormalizeRelativePath(filePath);

        if (string.IsNullOrWhiteSpace(
                normalizedPath))
        {
            return Task.FromResult(
                Result.Failure(
                    "Invalid file path."));
        }

        // =========================================================
        // Prevent Path Traversal
        // =========================================================

        if (ContainsParentTraversal(
                normalizedPath))
        {
            return Task.FromResult(
                Result.Failure(
                    "Invalid file path."));
        }

        // =========================================================
        // Build Safe Full Path
        // =========================================================

        string fullPath;

        try
        {
            fullPath =
                GetSafeFullPath(
                    normalizedPath);
        }
        catch
        {
            return Task.FromResult(
                Result.Failure(
                    "Invalid file path."));
        }

        // =========================================================
        // Check File Exists
        // =========================================================

        if (!File.Exists(fullPath))
        {
            return Task.FromResult(
                Result.Succeeded(
                    "File does not exist."));
        }

        // =========================================================
        // Delete File
        // =========================================================

        try
        {
            File.Delete(fullPath);
        }
        catch (UnauthorizedAccessException)
        {
            return Task.FromResult(
                Result.Failure(
                    "Unable to access file."));
        }
        catch (PathTooLongException)
        {
            return Task.FromResult(
                Result.Failure(
                    "File path is too long."));
        }
        catch (DirectoryNotFoundException)
        {
            return Task.FromResult(
                Result.Failure(
                    "Storage directory was not found."));
        }
        catch (IOException)
        {
            return Task.FromResult(
                Result.Failure(
                    "Unable to delete file."));
        }

        return Task.FromResult(
            Result.Succeeded(
                "File deleted successfully."));
    }

    // =========================================================
    // Build Safe Full Path
    // =========================================================

    private string GetSafeFullPath(
        string relativePath)
    {
        var combinedPath =
            Path.Combine(
                _storagePath,
                relativePath.Replace(
                    '/',
                    Path.DirectorySeparatorChar));

        var fullPath =
            Path.GetFullPath(
                combinedPath);

        if (!IsInsideStorage(fullPath))
        {
            throw new InvalidOperationException(
                "Path is outside storage directory.");
        }

        return fullPath;
    }

    // =========================================================
    // Check Path Is Inside Storage
    // =========================================================

    private bool IsInsideStorage(
        string fullPath)
    {
        var normalizedPath =
            Path.GetFullPath(fullPath);

        return normalizedPath.StartsWith(
            _storageRoot,
            StringComparison.OrdinalIgnoreCase);
    }

    // =========================================================
    // Normalize Relative Path
    // =========================================================

    private static string NormalizeRelativePath(
        string path)
    {
        return path
            .Trim()
            .Replace('\\', '/')
            .Trim('/');
    }

    // =========================================================
    // Detect Parent Traversal
    // =========================================================

    private static bool ContainsParentTraversal(
        string path)
    {
        var segments =
            path.Split(
                '/',
                StringSplitOptions.RemoveEmptyEntries);

        return segments.Any(
            x => x == "..");
    }

    // =========================================================
    // Ensure Trailing Separator
    // =========================================================

    private static string EnsureTrailingSeparator(
        string path)
    {
        return path.EndsWith(
            Path.DirectorySeparatorChar)
            ? path
            : path + Path.DirectorySeparatorChar;
    }

    // =========================================================
    // Delete Physical File
    // =========================================================

    private static void DeletePhysicalFile(
        string fullPath)
    {
        try
        {
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
        catch
        {
            // Ignore cleanup failure.
        }
    }

    // =========================================================
    // Format File Size
    // =========================================================

    private static string FormatFileSize(
        long bytes)
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