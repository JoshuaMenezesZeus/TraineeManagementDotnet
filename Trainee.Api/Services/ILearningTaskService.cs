using Trainee.Api.Models;
using Trainee.Api.DTO;


namespace Trainee.Api.DTO
{
    public interface ILearningTaskService
    {
        Task<(List<LearningTaskResponse> ?, int)> GetAllLearningTasks(int? pageNumber, int? pageSize, string? search, LearningTaskStatus? status);
        Task<LearningTaskResponse?> GetLearningTaskById(int id);
        Task<LearningTaskResponse> AddNewLearningTask(CreateLearningTask request);
        Task<LearningTaskResponse?> EditLearningTask(int id, UpdateLearningTask request);
        Task<bool> DeleteLearningTask(int id);
    }
}