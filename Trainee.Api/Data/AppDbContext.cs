using Microsoft.EntityFrameworkCore;
using Trainee.Api.Models;

namespace Trainee.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<TraineeModel> Trainees { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<MentorModel> Mentors { get; set; }
        public DbSet<LearningTaskModel> LearningTasks { get; set; }
        public DbSet<TaskAssignmentModel> TaskAssignments { get; set; }
        public DbSet<SubmissionModel> Submissions { get; set; }
        public DbSet<ReviewModel> Reviews { get; set; }
        public DbSet<SubmissionFileModel> SubmissionFiles { get; set; }
        public DbSet<ProcessingJob> ProcessingJobs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserModel>().HasData(new UserModel
            {
                Id = 2,
                Username = "admin1",
                Email = "admin1@gmail.com",
                PasswordHash = "$2a$11$FqNM7CGqeNqTTmYr9RFUUu2T4ukh.Zv94NbXCzx8kwikVuSws8hki",
                Role = UserRole.Admin,
                CreatedDate = DateTime.Parse("2026-06-10T10:57:50"),
                UpdatedDate = DateTime.Parse("2026-06-10T10:57:50")
            });
        }
    } 
}
