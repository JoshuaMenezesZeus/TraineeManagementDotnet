using Microsoft.AspNetCore.Http.HttpResults;
using Trainee.Api.Data;
using Trainee.Api.DTO;
using Trainee.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Trainee.Api.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SubmissionService> _logger;
        private readonly IRedisService _cache;

        public SubmissionService(AppDbContext context, ILogger<SubmissionService> logger, IRedisService cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;

        }

        public SubmissionResponse MaptoDTO(SubmissionModel subm)
        {
            return new SubmissionResponse
            {
                Id = subm.Id,
                TaskAssignmentId = subm.TaskAssignmentId,
                AssignedDate = subm.TaskAssignmentModel.AssignedDate,
                DueDate = subm.TaskAssignmentModel.DueDate,
                TaskAssignmentStatus = subm.TaskAssignmentModel.Status,
                SubmissionUrl = subm.SubmissionUrl,
                Notes = subm.Notes,
                SubmittedDate = subm.SubmittedDate,
                Status = subm.Status,
            };
        }
        public async Task<SubmissionResponse> AddANewSubmission(CreateSubmissionRequest request)
        {
                var taskAssignment = await _context.TaskAssignments.FindAsync(request.TaskAssignmentId);
                if (taskAssignment == null)
                {
                    throw new KeyNotFoundException("Task Assignment Not found in the database");
                }
                var subm = new SubmissionModel
                {
                    TaskAssignmentId = request.TaskAssignmentId,
                    SubmissionUrl = request.SubmissionUrl,
                    Notes = request.Notes ?? "",
                    SubmittedDate = request.SubmittedDate,
                    Status = request.Status,
                };
                await _context.Submissions.AddAsync(subm);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Task Assignment added successfully! Id: {Id}", subm.Id);
                return MaptoDTO(subm);
        }

        public async Task<List<SubmissionResponse>> GetAllSubmissions()
        {
                var subm = await _context.Submissions
                .Include(t => t.TaskAssignmentModel)
                .ToListAsync();
                return subm.Select(MaptoDTO).ToList();
        }

        public async Task<SubmissionResponse?> GetByIdSubmission(int id)
        {
                var data = await _cache.GetAsync<SubmissionResponse>($"submission-summary:{id}");
                if (data != null)
                {
                    return data;
                }
                var subm = await _context.Submissions
                .Include(t => t.TaskAssignmentModel)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
                

                if (subm == null)
                {
                    throw new KeyNotFoundException("Task Assignment not found in the database");

                }
                else
                {
                    var resp = MaptoDTO(subm);
                    await _cache.SetAsync(resp, $"submission-summary:{id}");
                    return resp;
                }
        }

    }
}