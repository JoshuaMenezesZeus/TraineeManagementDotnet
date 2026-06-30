using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Trainee.Api.Models
{
    public enum MentorStatus
    {
        Active, Inactive
    }
    public class MentorModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; } = String.Empty;
        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; } = String.Empty;
        [Required(ErrorMessage = "Email is required")][EmailAddress]
        public string Email {get; set;} = String.Empty;
        [Required(ErrorMessage = "Expertise is required")]
        public string Expertise { get; set; } = String.Empty;
   
        [Required(ErrorMessage = "Mentor Status is required")]
        public MentorStatus Status { get; set; } = MentorStatus.Active;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}