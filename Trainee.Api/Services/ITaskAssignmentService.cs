using Trainee.Api.DTO;
using Trainee.Api.Models;

namespace Trainee.Api.Services
{
    public interface ITaskAssignmentService
    {
        Task<List<TaskAssignmentResponse>> GetAllTaskAssignments();
        Task<TaskAssignmentResponse?> GetByIdTaskAssignment(int id);
        Task<TaskAssignmentResponse> AddANewTaskAssignment(CreateTaskAssignmentRequest request);
        Task<bool> UpdateTaskAssingnmentStatus(int id, UpdateTaskAssignmentRequest request);
    }
}