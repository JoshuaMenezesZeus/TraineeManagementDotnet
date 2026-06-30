using Microsoft.AspNetCore.Http.HttpResults;
using Trainee.Api.Data;
using Trainee.Api.DTO;
using Trainee.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Trainee.Api.Services
{
    public class TaskAssignmentService : ITaskAssignmentService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TaskAssignmentService> _logger;
        private readonly IRedisService _cache;

        public TaskAssignmentService(AppDbContext context, ILogger<TaskAssignmentService> logger, IRedisService cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        public TaskAssignmentResponse MaptoDTO(TaskAssignmentModel taskAssignment)
        {
            return new TaskAssignmentResponse
            {
                Id = taskAssignment.Id,
                TraineeId = taskAssignment.TraineeId,
                TraineeName = $"{taskAssignment.TraineeModel.FirstName} {taskAssignment.TraineeModel.LastName}",
                MentorId = taskAssignment.MentorId,
                MentorName = $"{taskAssignment.MentorModel.FirstName} {taskAssignment.MentorModel.LastName}",
                LearningTaskId = taskAssignment.LearningTaskId,
                LearningTaskTitle = taskAssignment.LearningTaskModel.Title,
                LearningTaskDescription = taskAssignment.LearningTaskModel.Description,
                Status = taskAssignment.Status,
                AssignedDate = taskAssignment.AssignedDate,
                DueDate = taskAssignment.DueDate,
                Remarks = taskAssignment.Remarks
            };
        }
        public async Task<TaskAssignmentResponse> AddANewTaskAssignment(CreateTaskAssignmentRequest request)
        {
                var trainee = await _context.Trainees.FindAsync(request.TraineeId);
                if (trainee == null)
                {
                    throw new KeyNotFoundException("Trainee Not found in the database");
                }
                var mentor = await _context.Mentors.FindAsync(request.MentorId);
                if (mentor == null)
                {
                    throw new KeyNotFoundException("Mentor Not found in the database");
                }
                var learningTask = await _context.LearningTasks.FindAsync(request.LearningTaskId);
                if (learningTask == null)
                {
                    throw new KeyNotFoundException("Learning Task Not found in the database");
                }
                if (request.DueDate < request.AssignedDate)
                {
                    throw new ArgumentException("Due date cannot be before assigned date");
                }
                var newtaskassignment = new TaskAssignmentModel
                {
                    TraineeId = request.TraineeId,
                    MentorId = request.MentorId,
                    LearningTaskId = request.LearningTaskId,
                    AssignedDate = request.AssignedDate,
                    DueDate = request.DueDate,
                    Status = request.Status,
                    Remarks = request.Remarks ?? ""
                };
                await _context.TaskAssignments.AddAsync(newtaskassignment);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Task Assignment added successfully! Id: {Id}", newtaskassignment.Id);
                return MaptoDTO(newtaskassignment);
        }

        public async Task<List<TaskAssignmentResponse>> GetAllTaskAssignments()
        {
                var taskAssignments = await _context.TaskAssignments
                .Include(t => t.TraineeModel)
                .Include(t => t.MentorModel)
                .Include(t => t.LearningTaskModel)
                .ToListAsync();
                return taskAssignments.Select(MaptoDTO).ToList();
        }

        public async Task<TaskAssignmentResponse?> GetByIdTaskAssignment(int id)
        {
                var data = await _cache.GetAsync<TaskAssignmentResponse>($"task-assignment:{id}");
                if (data != null)
                {
                    return data;
                }
                var taskAssignments = await _context.TaskAssignments
                .Include(t => t.TraineeModel)
                .Include(t => t.MentorModel)
                .Include(t => t.LearningTaskModel)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
                

                if (taskAssignments == null)
                {
                    throw new KeyNotFoundException("Task Assignment not found in the database");
                }
                else
                {
                    var resp = MaptoDTO(taskAssignments);
                    await _cache.SetAsync(resp, $"task-assignment:{id}");
                    return resp;
                }
        }

        public async Task<bool> UpdateTaskAssingnmentStatus(int id, UpdateTaskAssignmentRequest request)
        {
                await _cache.RemoveAsync($"task-assignment:{id}");

                var taskAssignment = await _context.TaskAssignments.FindAsync(id);
                if (taskAssignment == null)
                {
                    throw new KeyNotFoundException("Task Assignment not found in the database");
                }
                taskAssignment.Status = request.Status;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Task Assignment Edited successfully! Id: {Id}", id);

                return true;
        }
    }
}