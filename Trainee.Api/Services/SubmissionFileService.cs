using Trainee.Api.Data;
using Trainee.Api.DTO;
using Trainee.Api.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using Trainee.Api.Models;

namespace Trainee.Api.Services
{
    public class SubmissionFileService : ISubmissionFileService
    {
        private readonly AppDbContext _context;
        private readonly IFileStorageService _storage;
        private readonly FileStorageSettings _settings;
        private readonly ILogger<SubmissionFileService> _logger;
        private readonly ISubmissionProcessingPublisher _messagePublisher;

        private readonly IHttpContextAccessor _httpContextAccessor;
        public SubmissionFileService(AppDbContext context, IFileStorageService storage, IOptions<FileStorageSettings> options, ILogger<SubmissionFileService> logger, ISubmissionProcessingPublisher messagePublisher, IHttpContextAccessor httpContextAccessor)
        {
            _settings = options.Value;
            _context = context;
            _storage = storage;
            _logger=logger;
            _messagePublisher = messagePublisher;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<SubmissionFileResponse> UploadAsync(int submissionId, IFormFile file, int userId)
        {
            if (file == null || file.Length==0)
                throw new ArgumentException("File is Null");
            if (file.Length > _settings.MaxFileSizeBytes)
                throw new BadHttpRequestException($"File size greater than {_settings.MaxFileSizeBytes/(1024*1024) } MB.");
            
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if(!_settings.AllowedExtensions.Contains(extension))
                throw new ArgumentException("File Type is not allowed");

            if(!_settings.AllowedContentTypes.Contains(file.ContentType))
                throw new ArgumentException("File Content Type is not supported");

            var submission = await _context.Submissions.FindAsync(submissionId);            
            if (submission==null)
                throw new KeyNotFoundException("Submission Not Found");
            
            var user = await _context.Trainees.FindAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("Trainee Not Found");

            string checksum;
            using(var sha256 = SHA256.Create())
            {
                using var checksumStream = file.OpenReadStream();
                checksum = Convert.ToHexString(await sha256.ComputeHashAsync(checksumStream));
            }
            var storageName = await _storage.SaveAsync(file.OpenReadStream(), extension);
            var metadata = new SubmissionFileModel
            {
                SubmissionId = submissionId,
                OriginalFileName = file.FileName,
                CheckSum = checksum,
                StorageFileName = storageName,
                CreatedDate = DateTime.Now,
                Size = file.Length,
                ContentType = file.ContentType,
                UploadedByUserId = userId
            };

            await _context.SubmissionFiles.AddAsync(metadata);
            await _context.SaveChangesAsync();


            
            // Rabbit MQ added here
            var message = new SubmissionProcessRequested
            {
                MessageID = Guid.NewGuid(),
                CorrelationId = _httpContextAccessor.HttpContext?.TraceIdentifier,
                SubmissionId = submissionId,
                FileId = metadata.Id,
                RequestedAt = DateTime.Now,
                ContractVersion=1
            };
            try
            {
                await _messagePublisher.PublishAsync(message);
            }
            catch
            {
                _context.SubmissionFiles.Remove(metadata);
                _storage.DeleteAsync(metadata.StorageFileName);
                await _context.SaveChangesAsync();
                throw new Exception("Connection to RabbitMQ Failed at Publisher side!");
            }

            var job = new ProcessingJob
            {
              MessageID = message.MessageID,
              CorrelationId = _httpContextAccessor.HttpContext?.TraceIdentifier ?? "no context",
              SubmissionId = message.SubmissionId,
              FileId = message.FileId,
              ProcessingJobStatus = ProcessingJobStatus.Queued,
              Attempts=0,
            //   StartedAt = DateTime.Now,
            };
            await _context.ProcessingJobs.AddAsync(job);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Processing Job Status: {Status}  with id: {jobId}", job.ProcessingJobStatus,job.Id);

            _logger.LogInformation("File Uploaded successfully with the following metadata: ID: {FileId}, Name: {FileName}, Size: {FileSize} bytes, ContentType: {ContentType}, CreatedDate: {CreatedDate}", metadata.Id, metadata.OriginalFileName, metadata.Size, metadata.ContentType,  metadata.CreatedDate);
            return new SubmissionFileResponse
            {
                Id = metadata.Id,
                OriginalFileName = metadata.OriginalFileName,
                ContentType = metadata.ContentType,
                FileSize = metadata.Size,
                CreatedDate = metadata.CreatedDate,
                CorrelationId = _httpContextAccessor.HttpContext?.TraceIdentifier ?? "no context"         
            };
        }

        public async Task<FileDownloadResponse> DownloadAsync(int fileId)
        {
            var metadata = await _context.SubmissionFiles.FindAsync(fileId);
            if (metadata == null)
                throw new KeyNotFoundException("File Not Found");
            var exists = _storage.ExistsAsync(metadata.StorageFileName);
            if(!exists)
                throw new KeyNotFoundException("Physical File is Missing");

            var stream = _storage.OpenReadAsync(metadata.StorageFileName);
            _logger.LogInformation("File Downloaded successfully: {fileId}", fileId);
            return new FileDownloadResponse
            {
                Stream = stream,
                ContentType = metadata.ContentType,
                FileName = metadata.OriginalFileName
            };

        }

        public async Task DeleteAsync(int fileId)
        {
            var metadata = await _context.SubmissionFiles.FindAsync(fileId);
                if (metadata == null)
                    throw new KeyNotFoundException("File Not Found");
                _storage.DeleteAsync(metadata.StorageFileName);
                _context.SubmissionFiles.Remove(metadata);
                await _context.SaveChangesAsync();
                _logger.LogInformation("File Deleted successfully: {fileId}", fileId);

        }
        public async Task<SubmissionFileResponse> GetById(int fileId)
        {
            var metadata = await _context.SubmissionFiles.FindAsync(fileId);
            if (metadata == null)
                throw new KeyNotFoundException("File Not Found");
            
            return new SubmissionFileResponse
            {
                Id = metadata.Id,
                OriginalFileName = metadata.OriginalFileName,
                ContentType = metadata.ContentType,
                FileSize = metadata.Size,
                CreatedDate = metadata.CreatedDate,
                CorrelationId = _httpContextAccessor.HttpContext?.TraceIdentifier ?? "no context"        
            };
        }
    }
}
