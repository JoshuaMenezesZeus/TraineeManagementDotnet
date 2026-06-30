using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SubmissionProcessor.Worker.Models
{
    public enum UserRole
    {
        Admin, Mentor, Trainee
    }
    public class UserModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = String.Empty;

        [Required(ErrorMessage = "Email is required")][EmailAddress]
        public string Email {get; set;} = String.Empty;

        [Required(ErrorMessage = "Password Hash is required")]
        public string PasswordHash { get; set; } = String.Empty;

        [Required(ErrorMessage = "User Role is required")]
        public UserRole Role { get; set; } = UserRole.Trainee;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}