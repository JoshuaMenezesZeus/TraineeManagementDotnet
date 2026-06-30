using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Trainee.Api.Models;

namespace Trainee.Api.DTO
{
    public class CreateSubmissionRequest
    {
        [Required]
        public int TaskAssignmentId {get; set;}

        [Required(ErrorMessage = "URL is required")]
        [Url]
        public string SubmissionUrl {get; set;} = String.Empty;
        public string Notes {get; set;} = String.Empty;

        [Required(ErrorMessage = "Submission Date is reqiured")]
        public DateTime SubmittedDate {get; set;} = DateTime.Now;

        [Required(ErrorMessage = "Sumbission Status is required")]
        public SubmissionStatus Status {get; set;} = SubmissionStatus.Submitted;
    }
}