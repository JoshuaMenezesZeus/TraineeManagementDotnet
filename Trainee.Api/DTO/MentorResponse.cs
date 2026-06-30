using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Trainee.Api.Models;

namespace Trainee.Api.DTO
{
    public class MentorResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = String.Empty;
        public string LastName { get; set; } = String.Empty;
        public string Email {get; set;} = String.Empty;
        public string Expertise { get; set; } = String.Empty;
        public MentorStatus Status { get; set; } = MentorStatus.Active;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}