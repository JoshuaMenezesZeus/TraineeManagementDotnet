using SubmissionProcessor.Worker;
using SubmissionProcessor.Worker.Settings;
using SubmissionProcessor.Worker.Workers;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using SubmissionProcessor.Worker.Data;
using SubmissionProcessor.Worker.Services;
using Microsoft.Extensions.Http.Resilience;
using System.Net.Http.Headers;
using Serilog;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection; 

DotNetEnv.Env.Load();
var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddEnvironmentVariables();

// 1. DELETE the bad AddServiceDiscoveryEnvironmentVariables line
// 2. DELETE builder.Services.AddServiceDiscovery()

builder.Services.AddHostedService<Worker>();



builder.Services.Configure<RabbitMQSettings>(
    builder.Configuration.GetSection("RabbitMQ")
);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.Logger(lc => lc
        .Filter.ByExcluding(logEvent =>
            logEvent.Properties.TryGetValue("SourceContext", out var value) &&
            (value.ToString().Contains("Microsoft") || value.ToString().Contains("System")))
        .WriteTo.File(
            path: "logs/app-.txt",
            rollingInterval: RollingInterval.Day,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}"))        
    .CreateLogger();
 
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();
 



 

var trainingDirectorySection = builder.Configuration.GetSection("TrainingDirectory");
builder.Services.Configure<TrainingDirectorySettings>(trainingDirectorySection);
 
var directorySettings = trainingDirectorySection.Get<TrainingDirectorySettings>()
    ?? throw new InvalidOperationException("TrainingDirectory settings are missing!");
 
builder.Services.AddHttpClient<ITraineeDirectoryClient, TrainingDirectoryClient>(client =>
{
    // 1. Look for Aspire's exact injected environment variable names (checking both uppercase and lowercase)
    var aspireUrl = Environment.GetEnvironmentVariable("services__trainingdirectory__http__0")
                    ?? Environment.GetEnvironmentVariable("SERVICES__TRAININGDIRECTORY__HTTP__0");

    if (!string.IsNullOrEmpty(aspireUrl))
    {
        client.BaseAddress = new Uri(aspireUrl);
    }
    else
    {
        // 2. Fallback to your local directorySettings if running outside of Aspire
        client.BaseAddress = new Uri(directorySettings.BaseUrl);
    }

    client.Timeout = TimeSpan.FromSeconds(directorySettings.TimeoutSeconds);
 
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
})

.AddStandardResilienceHandler(options =>
{
    options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(8);
    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(3);
 
    // retry only transient failures
    options.Retry.MaxRetryAttempts = 2;
    options.Retry.Delay = TimeSpan.FromSeconds(1);
 
    // circuit breaker
    options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(10);
    options.CircuitBreaker.MinimumThroughput = 2;
    options.CircuitBreaker.FailureRatio = 0.5;
    options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(10);
});


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString, 
        new MySqlServerVersion(new Version(8, 0, 36)) // Forces a fixed version instead of calling AutoDetect
    )
);

builder.Services.AddHostedService<SubmissionProcessingWorker>();


var host = builder.Build();
host.Run();
