using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubmissionProcessor.Worker.Models
{
    public enum TraineeStatus
    {
        Active, Inactive, Completed
    }

    public class TraineeModel
    {
        [Key] // Primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "First name is required")][MaxLength(50)] public String FirstName { get; set; }= String.Empty;
        [Required(ErrorMessage = "Last name is required")][MaxLength(50)] public String LastName { get; set; }= String.Empty;
        [Required(ErrorMessage = "Email is required")][EmailAddress] public String Email { get; set; }= String.Empty;
        [Required(ErrorMessage = "Tech Stack is required")] public String TechStack { get; set; }= String.Empty;
        [Required(ErrorMessage = "Status is required")] public TraineeStatus Status { get; set; }=TraineeStatus.Active;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}