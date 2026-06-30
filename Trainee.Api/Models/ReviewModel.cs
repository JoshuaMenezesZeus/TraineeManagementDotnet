using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trainee.Api.Models
{
    public enum ReviewStatus
    {
        Accepted, ChangesRequired, Rejected
    }
    public class ReviewModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("SubmissionModel")]
        public int SubmissionId { get; set; }
        public SubmissionModel SubmissionModel { get; set; } = null!;

        [ForeignKey("MentorModel")]
        public int MentorId { get; set; }
        public MentorModel MentorModel { get; set; } = null!;
        [Required(ErrorMessage ="Feedback is Required")]
        public string Feedback {get; set;} = String.Empty;
        public int? Score { get; set; } = 0;

        [Required(ErrorMessage ="Review Status is required")]
        public ReviewStatus ReviewStatus {get; set;}= ReviewStatus.Rejected;

        [Required(ErrorMessage = "Reviewed Date is reqiured")]
        public DateTime ReviewedDate { get; set; } = DateTime.Now;

    }
}