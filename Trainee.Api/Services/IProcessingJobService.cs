using Trainee.Api.DTO;

namespace Trainee.Api.Services
{
    public interface IProcessingJobService
    {
        Task<ProcessingJobResponse?> GetByIdAsync(int id);
    }
}