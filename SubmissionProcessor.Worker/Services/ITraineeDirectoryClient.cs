using SubmissionProcessor.Worker.DTO;

namespace SubmissionProcessor.Worker.Services
{
    public interface ITraineeDirectoryClient
    {
        Task<TraineeProfileResponse?> GetProfile(int id, string? correlationId, CancellationToken cancellationToken);
    }
}