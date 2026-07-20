using Microsoft.Extensions.Options;
using Trainee.Api.Services;
using Microsoft.EntityFrameworkCore;
using Trainee.Api.Data;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Trainee.Api.Settings;
using System.Text;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.OpenApi;
using System.ComponentModel;
using Trainee.Api.DTO;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Trainee.api.Middleware;
using DotNetEnv;
using Serilog;
using Trainee.Api.Services.HealthCheckServices;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

DotNetEnv.Env.Load();

var MyAllowSpecificOrigins = "_AllowOrigins";

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins, policy => {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173");
        policy.AllowCredentials();
        policy.WithHeaders("accept", "content-type", "origin", "Authorization");
        });
} );

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();
builder.Services.AddExceptionHandler<ExceptionHandlingMiddleware>();
builder.Services.AddProblemDetails();
builder.Services.AddControllers();
builder.Services.AddScoped<ITraineeService, TraineeService>();
builder.Services.AddScoped<IMentorService, MentorService>();
builder.Services.AddScoped<ILearningTaskService, LearningTaskService>();
builder.Services.AddScoped<ITaskAssignmentService, TaskAssignmentService>();
builder.Services.AddScoped<ISubmissionService, SubmissionService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<ISubmissionFileService, SubmissionFileService>();
builder.Services.AddSingleton<IRedisService, RedisService>();
builder.Services.AddScoped<ISubmissionProcessingPublisher, SubmissionProcessingPublisher>();
builder.Services.AddScoped<IRabbitMQConnectionService, RabbitMQConnectionService>();
builder.Services.AddScoped<IProcessingJobService, ProcessingJobService>();

string redisConnectionString = builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379,abortConnect=false";
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    return ConnectionMultiplexer.Connect(redisConnectionString);
});

builder.Services.AddHealthChecks().AddCheck<MySQLHealthCheck>("mysql").AddCheck<RedisHealthCheck>("redis").AddCheck<RabbitMQHealthCheck>("rabbitmq");

builder.Services.AddHttpClient();

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt")
);
builder.Services.Configure<FileStorageSettings>(
    builder.Configuration.GetSection("FileStorage")
);
builder.Services.Configure<RedisSettings>(
    builder.Configuration.GetSection("Redis")
);
builder.Services.Configure<RabbitMQSettings>(
    builder.Configuration.GetSection("RabbitMQ")
);
builder.Services.AddStackExchangeRedisCache(options =>
 {
     options.Configuration = builder.Configuration["Redis:ConnectionString"];
     options.InstanceName = builder.Configuration["Redis:InstanceName"];
 });

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


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();



builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
       Title = "Trainee.Api" ,
       Version = "v1"

    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Enter your JWT Status Below"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement

    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []

    }
    );
});

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings!.Issuer, 
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true, 
                    ValidateIssuerSigningKey = true, 
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key!))
                };
            });



var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);



builder.Services.AddControllers().AddJsonOptions(
    options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }
    );
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<AppDbContext>();
 
    await context.Database.MigrateAsync();
 
    await DbSeeder.SeedAdminUserAsync(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Trainee api v1");
    });
}


app.UseCors(MyAllowSpecificOrigins);
app.UseExceptionHandler(); 
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


app.MapGet("/", () => "Hello World!");

app.MapControllers();

app.Run();

