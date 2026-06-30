using Microsoft.AspNetCore.Http.HttpResults;
using Trainee.Api.Data;
using Trainee.Api.DTO;
using Trainee.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Trainee.Api.Services
{
    public class ReviewService : IReviewService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ReviewService> _logger;
        public ReviewService(AppDbContext context, ILogger<ReviewService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public ReviewResponse MaptoDTO(ReviewModel review)
        {
            return new ReviewResponse
            {
                Id = review.Id,
                SubmissionId = review.SubmissionId,
                SubmissionUrl = review.SubmissionModel.SubmissionUrl,
                SubmittedDate = review.SubmissionModel.SubmittedDate,
                SubmissionStatus = review.SubmissionModel.Status,
                MentorId = review.MentorId,
                MentorName = $"{review.MentorModel.FirstName} {review.MentorModel.LastName}",
                Feedback = review.Feedback,
                Score = review.Score,
                ReviewStatus = review.ReviewStatus,
                ReviewedDate = review.ReviewedDate
            };
        }
        public async Task<ReviewResponse> AddANewReview(CreateReviewRequest request)
        {
                var subm = await _context.Submissions.FindAsync(request.SubmissionId);
                if (subm == null)
                {
                    throw new KeyNotFoundException("Submission Not found in the database");
                }
                var mentor = await _context.Mentors.FindAsync(request.MentorId);
                if (mentor == null)
                {
                    throw new KeyNotFoundException("Mentor Not found in the database");
                }
                var review = new ReviewModel
                {
                    Feedback = request.Feedback,
                    Score = request.Score ?? 0,
                    ReviewStatus = request.ReviewStatus,
                    ReviewedDate = request.ReviewedDate,
                    SubmissionId = request.SubmissionId,
                    MentorId = request.MentorId
                };
                await _context.Reviews.AddAsync(review);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Review added successfully! Id: {Id}", review.Id);
                return MaptoDTO(review);
        }

        public async Task<List<ReviewResponse>> GetAllReviews()
        {
                var review = await _context.Reviews
                .Include(t => t.SubmissionModel)
                .Include(t => t.MentorModel)
                .ToListAsync();
                return review.Select(MaptoDTO).ToList();
        }

        public async Task<ReviewResponse?> GetByIdReview(int id)
        {
                var review = await _context.Reviews
                .Include(t => t.SubmissionModel)
                .Include(t => t.MentorModel)
                .FirstOrDefaultAsync(t => t.Id == id);
                

                if (review == null)
                {
                    throw new KeyNotFoundException("Review not found in the database");
                }
                else
                {
                    _logger.LogInformation("Retrieved Review with ID: {id}", id);
                    return MaptoDTO(review);
                }
        }


    }
}