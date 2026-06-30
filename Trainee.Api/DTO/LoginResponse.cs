using System.ComponentModel.DataAnnotations;
using Trainee.Api.Models;

namespace Trainee.Api.DTO
{
    public class LoginResponse
    {
        public string Token { get; set; } = String.Empty;

        public int ExpiresIn { get; set; }

        public UserResponse User { get; set; } = new UserResponse();
    }
}