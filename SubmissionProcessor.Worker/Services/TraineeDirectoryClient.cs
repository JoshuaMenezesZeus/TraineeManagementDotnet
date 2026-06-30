using System.Net;
using System.Net.Http.Json;
using SubmissionProcessor.Worker.DTO;
using SubmissionProcessor.Worker.Services;
 
namespace SubmissionProcessor.Worker.Services;
 
public class TrainingDirectoryClient : ITraineeDirectoryClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TrainingDirectoryClient> _logger;
 
    public TrainingDirectoryClient(
        HttpClient httpClient,
        ILogger<TrainingDirectoryClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
 
    public async Task<TraineeProfileResponse?> GetProfile(
        int id,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/api/TraineeProfile/{id}");
 
        if (!string.IsNullOrWhiteSpace(correlationId))
        {
            request.Headers.Add("X-Correlation-Id", correlationId);
        }
 
        var response = await _httpClient.SendAsync(request, cancellationToken);
 
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogWarning(
                "Trainee profile not found in TrainingDirectory for id {id}",
                id);
 
            return null;
        }
 
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
 
            _logger.LogError(
                "TrainingDirectory trainee lookup failed for id {id}. StatusCode: {StatusCode}",
                id,
                (int)response.StatusCode);
 
            throw new HttpRequestException(
                $"TrainingDirectory trainee lookup failed. StatusCode={(int)response.StatusCode}. Response={error}");
        }
 
        return await response.Content.ReadFromJsonAsync<TraineeProfileResponse>(
            cancellationToken: cancellationToken);
    }
 
}