using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Trainee.Api.Models;

namespace Trainee.Api.DTO
{
    public class CreateReviewRequest
    {
        public int SubmissionId { get; set; }

        public int MentorId { get; set; }
        public int? Score { get; set; } = 0;
        [Required(ErrorMessage ="Feedback is Required")]

        public string Feedback {get; set;} = String.Empty;

        [Required(ErrorMessage ="Review Status is required")]
        public ReviewStatus ReviewStatus {get; set;}= ReviewStatus.Rejected;

        [Required(ErrorMessage = "Reviewed Date is reqiured")]
        public DateTime ReviewedDate { get; set; } = DateTime.Now;

    }
}