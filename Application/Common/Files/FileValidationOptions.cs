namespace MicroERP.Application.Common.Files;

public class FileValidationOptions
{
    public long MaxFileSize { get; set; }

    public List<string> AllowedExtensions { get; set; }
        = new();

    public List<string> AllowedContentTypes { get; set; }
        = new();
}