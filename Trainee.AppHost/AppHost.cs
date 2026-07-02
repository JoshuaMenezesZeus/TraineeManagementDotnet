using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Registers your API with a service name "trainingdirectory"
var trainingDirectory = builder.AddProject<Projects.TrainingDirectory_Api>("trainingdirectory");

// Registers your Worker and links it to the API
builder.AddProject<Projects.SubmissionProcessor_Worker>("worker")
       .WithReference(trainingDirectory); 

builder.Build().Run();
