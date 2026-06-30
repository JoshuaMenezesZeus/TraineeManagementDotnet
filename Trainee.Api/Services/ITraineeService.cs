using Trainee.Api.DTO;
using Trainee.Api.Models;

namespace Trainee.Api.Services
{
    public interface ITraineeService
    {
        // CreateTraineeService (CreateTraineeRequest request);
        Task<(List <TraineeResponse>?, int)> GetAllTrainees(int? pageNumber, int? pageSize, string? search, TraineeStatus? status);
        Task<TraineeResponse?> GetById(int id);

        Task<TraineeResponse> AddANewTrainee(CreateTraineeRequest request);
        Task<TraineeResponse?> EditTrainee(int id, UpdateTraineeRequest request);
        Task<bool> DeleteTrainee(int id);
    }
}