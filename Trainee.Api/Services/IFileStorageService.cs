using Trainee.Api.DTO;
using Trainee.Api.Models;

namespace Trainee.Api.Services
{
    public interface IFileStorageService
    {
        Task <string> SaveAsync(Stream stream, string extension);
        Stream OpenReadAsync(string storageName);
        bool ExistsAsync(string storageName);
        void DeleteAsync(string storageName);
    }
}