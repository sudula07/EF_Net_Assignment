using FluentValidation;
using ProductApi.Application.Common;

namespace ProductApi.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException validationException)
        {
            _logger.LogWarning(validationException, "Validation failure");
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                traceId = context.TraceIdentifier,
                status = context.Response.StatusCode,
                title = "Validation failed.",
                errors = validationException.Errors
                    .GroupBy(error => error.PropertyName)
                    .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).ToArray())
            });
        }
        catch (ApiException apiException)
        {
            _logger.LogWarning(apiException, "Handled application exception");
            context.Response.StatusCode = apiException.StatusCode;
            await context.Response.WriteAsJsonAsync(new
            {
                traceId = context.TraceIdentifier,
                status = context.Response.StatusCode,
                title = apiException.Message
            });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new
            {
                traceId = context.TraceIdentifier,
                status = context.Response.StatusCode,
                title = "An unexpected error occurred."
            });
        }
    }
}
