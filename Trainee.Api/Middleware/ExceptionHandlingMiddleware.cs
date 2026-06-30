using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
 
namespace Trainee.api.Middleware;
 
public class ExceptionHandlingMiddleware : IExceptionHandler
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
 
    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }
 
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Error occurred: {Message}", exception.Message);
 
        var (statusCode, title) = exception switch
        {
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource Not Found"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Invalid Input"),
            FileNotFoundException => (StatusCodes.Status404NotFound, "File Not Found"),
            BadHttpRequestException => (StatusCodes.Status413PayloadTooLarge, "Exceeds Threshold limit."),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };
 
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message
        };
 
        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
 
        return true;
    }
}