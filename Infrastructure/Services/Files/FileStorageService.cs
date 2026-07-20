using MicroERP.Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace MicroERP.Infrastructure.Services.Files;

public class FileStorageService : IFileStorageService
{
    private readonly string _storagePath;


    public FileStorageService(
        IWebHostEnvironment environment)
    {
        _storagePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Storage");
    }



    public async Task<string> SaveFileAsync(
    IFormFile file,
    string folder,
    CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty");


        var folderPath = Path.Combine(
            _storagePath,
            folder.Replace("/", Path.DirectorySeparatorChar.ToString()));


        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);



        var extension = Path.GetExtension(file.FileName);

        var storedFileName = $"{Guid.NewGuid()}{extension}";


        var fullPath = Path.Combine(
            folderPath,
            storedFileName);



        await using var stream = new FileStream(
         fullPath,
         FileMode.Create,
         FileAccess.Write,
         FileShare.None);


        await file.CopyToAsync(
            stream,
            cancellationToken);



        return Path.Combine(
            folder,
            storedFileName)
            .Replace("\\", "/");
    }



    public Task DeleteFileAsync(string filePath,
CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(filePath))
            return Task.CompletedTask;


        var fullPath = Path.Combine(
            _storagePath,
            filePath);



        if (File.Exists(fullPath))
            File.Delete(fullPath);



        return Task.CompletedTask;
    }
}