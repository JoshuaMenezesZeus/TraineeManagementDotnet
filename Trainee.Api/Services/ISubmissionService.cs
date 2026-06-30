using Trainee.Api.DTO;
using Trainee.Api.Models;

namespace Trainee.Api.Services
{
    public interface ISubmissionService
    {
        Task<List<SubmissionResponse>> GetAllSubmissions();
        Task<SubmissionResponse?> GetByIdSubmission(int id);
        Task<SubmissionResponse> AddANewSubmission(CreateSubmissionRequest request);
    }
}