using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Trainee.Api.Models;

namespace Trainee.Api.DTO
{
    public class ReviewResponse
    {
        public int Id { get; set; }
        public int SubmissionId { get; set; }
        public string SubmissionUrl { get; set; } = null!;
        public DateTime SubmittedDate {get; set;} = DateTime.Now;
        public SubmissionStatus SubmissionStatus {get; set;} = SubmissionStatus.Submitted;
        public int MentorId { get; set; }
        public string MentorName { get; set; } = String.Empty;
        public string Feedback {get; set;} = String.Empty;
        public int? Score { get; set; } = 0;

        public ReviewStatus ReviewStatus {get; set;}= ReviewStatus.Rejected;
        public DateTime ReviewedDate { get; set; } = DateTime.Now;
    }
}