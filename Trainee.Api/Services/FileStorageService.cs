using Microsoft.Extensions.Options;
using Trainee.Api.Settings;

namespace Trainee.Api.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _rootPath;
        private readonly ILogger<FileStorageService> _logger;

        public FileStorageService(IOptions<FileStorageSettings> options, ILogger<FileStorageService> logger)
        {
            _rootPath=options.Value.RootPath;
            Directory.CreateDirectory(_rootPath);
            _logger = logger;
        }
        public void DeleteAsync(string storageName)
        {
            var path = Path.Combine(_rootPath, storageName);
            if(!File.Exists(path))
                throw new FileNotFoundException("File is not found");
            File.Delete(path);
            _logger.LogInformation("File Deleted from Local Storage");
        
        }

        public bool ExistsAsync(string storageName)
        {
            if(File.Exists(Path.Combine(_rootPath, storageName)))
            {
                _logger.LogInformation("File exists on physical storage");
                return true;
            }
            _logger.LogInformation("File doesn't exists on physical storage");

            return false;
        }

        public Stream OpenReadAsync(string storageName)
        {
            Stream stream = File.OpenRead(Path.Combine(_rootPath, storageName));
            _logger.LogInformation("File opened from Local Storage");
            return stream;
        }

        public async Task<string> SaveAsync(Stream stream, string extension)
        {
            var storageName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(_rootPath, storageName);
            using var fileStream = File.Create(fullPath);
            await stream.CopyToAsync(fileStream);
            _logger.LogInformation("File Saved to Local Storage");
            return storageName;
        }
    }
}