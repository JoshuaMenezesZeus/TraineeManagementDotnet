using System.ComponentModel.DataAnnotations;
using Trainee.Api.Models;

namespace Trainee.Api.DTO
{
    public class UserResponse
    {
        public int Id { get; set; }

        public string Username { get; set; } = String.Empty;

        //comment out
        // public string PasswordHash { get; set; }

        public UserRole Role { get; set; } = UserRole.Trainee;

        // public DateTime CreatedDate { get; set; } = DateTime.Now;
        // public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}