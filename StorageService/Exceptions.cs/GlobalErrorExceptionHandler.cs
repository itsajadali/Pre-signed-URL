using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace StorageService.Exceptions.cs;

public class GlobalErrorExceptionHandler(IProblemDetailsService problemDetailsService,
                                         ILogger<GlobalErrorExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,
                                            Exception exception,
                                            CancellationToken cancellationToken)
    {

        logger.LogError(exception, "An error occurred while handling an exception.");

        httpContext.Response.StatusCode = exception switch
        {
            ApplicationException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = httpContext.Response.StatusCode,
                Title = "An error occurred while processing your request.",
                Detail = exception.Message
            }
        });


    }
}

