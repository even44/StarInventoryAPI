using Microsoft.AspNetCore.Diagnostics;

namespace StarInventoryAPI.Services;

// Maps exceptions to RFC 7807 ProblemDetails responses. Unknown exceptions become a generic 500
// without leaking exception details outside of development.
public class ApiExceptionHandler(IProblemDetailsService problemDetailsService, IHostEnvironment environment, ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource not found"),
            UnauthorizedAccessException => (StatusCodes.Status403Forbidden, "Forbidden"),
            BadHttpRequestException badRequest => (badRequest.StatusCode, "Bad request"),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred"),
        };

        if (status >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception for {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);
        }

        httpContext.Response.StatusCode = status;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails =
            {
                Status = status,
                Title = title,
                Detail = status < StatusCodes.Status500InternalServerError || environment.IsDevelopment() ? exception.Message : null,
            },
        });
    }
}
