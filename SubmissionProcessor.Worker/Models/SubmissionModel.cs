using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubmissionProcessor.Worker.Models
{
    public enum SubmissionStatus
    {
        Submitted, Resubmitted
    }
    public class SubmissionModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id {get; set;}

        [ForeignKey("TaskAssignmentModel")]
        public int TaskAssignmentId {get; set;}
        public TaskAssignmentModel TaskAssignmentModel {get; set;} = null!;

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