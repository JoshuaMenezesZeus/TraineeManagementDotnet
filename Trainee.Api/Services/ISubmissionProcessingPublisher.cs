using Trainee.Api.Models;

namespace Trainee.Api.Services
{
    public interface ISubmissionProcessingPublisher
    {
        Task PublishAsync(SubmissionProcessRequested message);
    }
}