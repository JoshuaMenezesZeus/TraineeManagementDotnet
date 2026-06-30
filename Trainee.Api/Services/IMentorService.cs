using Trainee.Api.Models;
using Trainee.Api.DTO;


namespace Trainee.Api.DTO
{
    public interface IMentorService
    {
        Task<(List<MentorResponse>?, int)> GetAllMentors(int? pageNumber, int? pageSize, string? search, MentorStatus? status);
        Task<MentorResponse?> GetMentorById(int id);

        Task<MentorResponse> AddANewMentor(CreateMentorRequest request);
        Task<MentorResponse?> EditMentor(int id, UpdateMentorRequest request);
        Task<bool> DeleteMentor(int id);
    }
}