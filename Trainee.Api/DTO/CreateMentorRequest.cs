using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Trainee.Api.Models;

namespace Trainee.Api.DTO
{
    public class CreateMentorRequest
    {

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
    }
}