using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var trainingDirectory = builder.AddProject<Projects.TrainingDirectory_Api>("trainingdirectory");

builder.AddProject<Projects.SubmissionProcessor_Worker>("worker")
       .WithReference(trainingDirectory); 

builder.Build().Run();
