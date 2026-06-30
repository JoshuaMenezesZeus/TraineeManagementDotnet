using Trainee.Api.DTO;
using Trainee.Api.Models;

namespace Trainee.Api.Services
{
    public interface ISubmissionFileService
    {
        Task<SubmissionFileResponse> UploadAsync(int submissionId, IFormFile file, int userId);
        Task<FileDownloadResponse> DownloadAsync(int fileId);
        Task DeleteAsync(int fileId);
    }
}
