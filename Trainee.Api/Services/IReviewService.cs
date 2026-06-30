using Trainee.Api.DTO;
using Trainee.Api.Models;

namespace Trainee.Api.Services
{
    public interface IReviewService
    {
        Task<List<ReviewResponse>> GetAllReviews();
        Task<ReviewResponse?> GetByIdReview(int id);
        Task<ReviewResponse> AddANewReview(CreateReviewRequest request);
    }
}