# Trainee Application

## Technology Used

C#, ASP.NET, MySql, Redis, RabbitMQ, Docker


## Project Structure

```text
TraineeManagement.Api
│
├── Controllers
├── Services
├── DTO
├── Models
├── Middleware
├── Migrations
├── Uploads
├── Data
├── logs
└── Settings

SubmissionProcessor.Worker
│
├── Workers
├── Services
├── DTO
├── Models
├── Data
├── logs
├── Settings
└── Logs

TrainingDirectory.Api
│
├── Controllers
└── DTO

Trainee.AppHost
│
└── AppHost.cs
```


## Architecture

<img width="1441" height="793" alt="workflow_diagram" src="https://github.com/user-attachments/assets/d016a0b5-2e46-48b7-a6b8-d101f5ab4db9" />

## Backend Setup Steps
- Download and extract the project
- Ensure dotnet and swagger is installed
- restore packages using dotnet restore
- dotnet run
- Navigate swagger endpoint to check the api's

## MySQL Setup Steps
- Install MySql Server
- Login to mysql using command: mysql -u root -p
- Create database of trainee management


## Configuration
- appsettings.json
- appsettings.Development.json
- .env files
- Docker Compose environment variables

## Docker Compose
```bash
docker compose up --build -d
```

The following services will be started automatically:
 
- MySQL
- Redis
- RabbitMQ
- Trainee.Api
- SubmissionProcessor.Worker
- TrainingDirectory.Api

## EF Core migration commands

```bash
dotnet ef migrations add Migration1
dotnet ef database update
```

## Login credentials for testing
```json
{ 
  "username": "admin1", 
  "password": "admin1" 
}
```
## JWT usage instructions
- navigate to /swagger
- click authorize button
- Enter: Bearer <token>
- Click authorize
- Check protected api's


## Features Completed
Trainee Management, Created web api 
- health check api
- created in memory list
- get all traineed
- get trainee by id
- create a new trainee
- Made DTO's for sending required fields (Create DTO, response dto, update dto)
- Used Service Layer and dependency injection.
- Create PUT and Delete API endpoints
- Validation applied on the dto's and models
- MySql Setup and connection. (Replaced with in memory database)
- Created User Entity
- Implemented Login api for the user model, with the respective DTOs
- Implemented JWT Token creation
- Implemented Password Hashing
- Protected the trainee api's using JWT, and made login and health routes public. 
- Configured swagger for sending bearer token. 
- Added pagination to Get All trainees route to avoid returning a large number of records. 
- Configured CORS for future React application. 
- Added Logging in the application to log important information.
- Created Mentor Model along with DTO's, Services and Controllers.
- Created Learning Task Model along with DTO's, Services and Controllers.
- Created Task Assignment Model along with DTO's, Services and Controllers.
- Created Review Model along with DTO's, Services and Controllers.
- Created Submission Model along with DTO's, Services and Controllers.
- Created FileStorage Service along with SaveAsync, OpenReadAsync, ExistsAsync, and DeleteAsync operations.
- Added validations and metadata tracking. 
- Added redis caching for faster retrieval of data.
- Logged cache failures and ensured fallback to database, whenever cache fails.

## API Endpoints

- GET /api/health: Returns health status of the server
- GET /api/Trainees?search=: Returns list of trainess. Also contains search param to filter by required trainees.
- POST /api/Trainees: Used for creating new Trainee
- GET /api/Trainees/{id}: Get specific trainee by id
- DELETE api/Trainees/{id}: Delete a trainee by specified ID
- PUT api/Trainees/{id}: Update trainee contents of a particular ID.

- POST /api/auth/login: Returns JWT Token along with login minutes on correct credentials.

- GET /api/mentors  :  Returns list of mentors. Also contains search param to filter by required mentors.
- GET /api/mentors/{id} : Get specific mentor by id
- POST /api/mentors : Used for creating new mentor
- PUT  /api/mentors/{id} : Update mentor contents of a particular ID.
- DELETE /api/mentors/{id} : Update mentor contents of a particular ID.

- GET    /api/learning-tasks : Returns list of Learning Tasks. Also contains search param to filter by required Tasks.
- GET    /api/learning-tasks/{id} : Get specific Tasks by id
- POST   /api/learning-tasks  : Used for creating new Tasks
- PUT    /api/learning-tasks/{id} : Update Tasks contents of a particular ID.
- DELETE /api/learning-tasks/{id} : Delete a Tasks by specified ID

- GET /api/task-assignments : Gets all the Task assignments from the database
- GET /api/task-assignments/{id} : Gets a particular Task assignment with the id parameter
- POST /api/task-assignments : Adds a new Task assignment to the database
- PUT /api/task-assignments/{id}/status : Changes status of a particular Task assignment using id parameter
 
- POST /api/submissions : Adds a new Submission to the database
- GET /api/submissions : Gets all the Submissions from the database
- GET /api/submissions/{id} : Gets a particular Submission with the id parameter
 
- POST /api/reviews : Adds a new Review to the database
- GET /api/reviews : Gets all the Reviews from the database
- GET /api/reviews/{id} : Gets a particular Review with the id parameter

- POST /api/submissions/{submissionId}/files: Post a new file to store it locally and update metadata in mysql.
- GET /api/submission-files/{id}/download: Download a file by id.
- DELETE /api/submission-files/{id} : Delete a file from local storage.
- GET /api/submission-files/{id}/download : Download the submission file

- DELETE /api/submission-files/{id} : Delete a submission file 
- POST /api/submissions/{submissionId}/files : Upload a submission file to the local storage

- GET /api/processing-jobs/{id} : Get the processing job by id.
- GET /health/live: Check to view status of the server.
- GET /health/ready: Views status of RabbitMQ, redis and MySQL
 
## Design Decisions
 
### Redis
- Cache-aside pattern
- Absolute TTL: 60 Minutes
- Sliding Expiration: 10 minutes
- Automatic database fallback
 
### RabbitMQ
- Durable Queue
- Persistent Messages
- Manual Acknowledgements
- Prefetch Count = 1
 
### Retry Strategy
- HTTP Client Resilience
- Timeout Handling
- Retry for Transient Failures
 
### Idempotency
- MessageId
- CorrelationId
These identifiers help prevent duplicate processing.
 
### Failure Behaviour

MySQL Unavailable
 
- Readiness check reports Unhealthy.
- API returns database-related errors.
 
Redis Unavailable
 
- Application falls back to MySQL.
- Cache failures are logged.
 
RabbitMQ Unavailable
 
- Publish operation logs failure.
- Worker remains unavailable until RabbitMQ recovers.
 
Worker Failure
 
- Message is negatively acknowledged (NACK).
- Processing is retried according to RabbitMQ configuration.



## Sample Request JSON
```json
{
  "firstName": "Joshua",
  "lastName": "Menezes",
  "email": "joshua@gmail.com",
  "techStack": "Python",
  "status": "Active"
}
```

## Sample Response JSON
```json
{
"id": 1,
"firstName": "Joshua",
"lastName": "Menezes",
"email": "joshua@gmail.com",
"techStack": "Python",
"status": Active,
"createdDate": "2026-06-08T12:54:05.2324036+00:00",
"updatedDate": "2026-06-08T12:54:05.2330662+00:00"
}
```

## Known Limitations
- No email verification
- No password reset

## Challenges Faced
- Dotnet installation of packages (proxy server blocker)

## Next Phase Scope
- Integration with frontend
