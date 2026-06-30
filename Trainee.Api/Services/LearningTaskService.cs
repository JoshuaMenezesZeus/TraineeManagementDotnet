using Microsoft.AspNetCore.Http.HttpResults;
using Trainee.Api.Data;
using Trainee.Api.DTO;
using Trainee.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Trainee.Api.Services
{
    public class LearningTaskService : ILearningTaskService
    {

        private readonly AppDbContext _context;
        private readonly ILogger<LearningTaskService> _logger;
        public LearningTaskService(AppDbContext context, ILogger<LearningTaskService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public LearningTaskResponse MaptoDTO(LearningTaskModel learningTask)
        {
            return new LearningTaskResponse
            {
                Title = learningTask.Title,
                Description = learningTask.Description,
                Id = learningTask.Id,
                ExpectedTechStack = learningTask.ExpectedTechStack,
                DueDate = learningTask.DueDate,
                Status = learningTask.Status,
                CreatedDate = learningTask.CreatedDate,
                UpdatedDate = learningTask.UpdatedDate
            };
        }
        public async Task<LearningTaskResponse> AddNewLearningTask(CreateLearningTask request)
        {
                var newlearningtask = new LearningTaskModel
                {
                    Title = request.Title,
                    Description = request.Description,
                    ExpectedTechStack = request.ExpectedTechStack,
                    DueDate = request.DueDate,
                    Status = request.Status,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                };

                await _context.LearningTasks.AddAsync(newlearningtask);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Learning Task added successfully! Id: {Id}", newlearningtask.Id);
                return MaptoDTO(newlearningtask);
        }

        public async Task<bool> DeleteLearningTask(int id)
        {
                var learningTask = await _context.LearningTasks.FindAsync(id);

                if (learningTask == null)
                {
                    throw new KeyNotFoundException("Learning Task not found in the database");
                }

                _context.LearningTasks.Remove(learningTask);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Learning Task Deleted successfully! Id: {Id}", id);

                return true;
        }

        public async Task<LearningTaskResponse?> EditLearningTask(int id, UpdateLearningTask request)
        {
                var learningTask = await _context.LearningTasks.FindAsync(id);

                if (learningTask == null)
                {
                    throw new KeyNotFoundException("Learning Task not found in the database");
                }

                learningTask.Title = request.Title;
                learningTask.Description = request.Description;
                learningTask.ExpectedTechStack = request.ExpectedTechStack;
                learningTask.DueDate = request.DueDate;
                learningTask.Status = request.Status;
                learningTask.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Learning Task Edited successfully! Id: {Id}", id);

                return MaptoDTO(learningTask);
        }

        public async Task<(List<LearningTaskResponse>?, int)> GetAllLearningTasks(int? pageNumber, int? pageSize, string? search, LearningTaskStatus? status)
        {
                var _learningtasks = await _context.LearningTasks.ToListAsync();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();
                    _learningtasks = _learningtasks.Where(
                        t => t.Title.ToLower().Contains(search) ||
                        t.Description.ToLower().Contains(search) ||
                        t.ExpectedTechStack.ToLower().Contains(search)).ToList();
                }
                if (status != null)
                    _learningtasks = _learningtasks.Where(t => t.Status == status).ToList();


                if (_learningtasks.Count() == 0 && (!string.IsNullOrWhiteSpace(search) || status != null))
                {
                    throw new KeyNotFoundException("No Learning Task Found.");

                }

                return (_learningtasks.Skip(((pageNumber ?? 1) - 1) * (pageSize ?? 10)).Take(pageSize ?? 10).Select(MaptoDTO).ToList(), _learningtasks.Count);
        }

        public async Task<LearningTaskResponse?> GetLearningTaskById(int id)
        {
            var learningtask = await _context.LearningTasks.FindAsync(id);

            if (learningtask == null)
            {
                throw new KeyNotFoundException("No Learning Task Found in the database.");
            }

            else
            {
                _logger.LogInformation("Learning Task Retrieved with Id: {id}", id);
                return MaptoDTO(learningtask);
            }
        }
    }
}