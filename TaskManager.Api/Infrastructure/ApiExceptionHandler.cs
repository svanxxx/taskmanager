using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TaskManager.Api.Infrastructure;

public sealed class RequestValidationException : Exception
{
    public RequestValidationException(string message)
        : base(message)
    {
    }
}

public sealed class TaskAccessException : Exception
{
    public TaskAccessException(string message)
        : base(message)
    {
    }
}

public sealed class ApiExceptionHandler : IExceptionHandler
{
    private readonly IHostEnvironment _environment;
    private readonly ILogger<ApiExceptionHandler> _logger;

    public ApiExceptionHandler(IHostEnvironment environment, ILogger<ApiExceptionHandler> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is OperationCanceledException)
        {
            return false;
        }

        var (status, title, detail) = Map(exception);
        _logger.LogError(exception, "Request failed with status {StatusCode}.", status);

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };
        problem.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }

    private (int Status, string Title, string Detail) Map(Exception exception)
    {
        switch (exception)
        {
            case RequestValidationException validation:
                return (StatusCodes.Status400BadRequest, "Bad Request", validation.Message);
            case TaskAccessException access:
                return (StatusCodes.Status403Forbidden, "Forbidden", access.Message);
            default:
                var detail = _environment.IsDevelopment()
                    ? exception.Message
                    : "The request could not be completed.";
                return (StatusCodes.Status500InternalServerError, "Internal Server Error", detail);
        }
    }
}
