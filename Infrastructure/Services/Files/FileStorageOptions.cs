

namespace MicroERP.Infrastructure.Services.Files
{
    public class FileStorageOptions
    {
        public string RootPath { get; set; } = "Storage";

        public long MaxFileSize { get; set; }
            = 10 * 1024 * 1024;
    }
}
