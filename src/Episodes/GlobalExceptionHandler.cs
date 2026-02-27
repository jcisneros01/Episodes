using System.Net;
using Episodes.Clients;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Episodes;

internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        HttpStatusCode statusCode;
        string title;

        if (exception is TmdbApiException tmdbEx)
        {
            (statusCode, title) = MapTmdbException(tmdbEx);
        }
        else
        {
            _logger.LogError(exception, "Unhandled exception");
            statusCode = HttpStatusCode.InternalServerError;
            title = "An unexpected error occurred.";
        }

        httpContext.Response.StatusCode = (int)statusCode;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title
        }, cancellationToken);

        return true;
    }

    private static (HttpStatusCode StatusCode, string Title) MapTmdbException(TmdbApiException ex)
    {
        switch (ex.StatusCode)
        {
            case HttpStatusCode.TooManyRequests:
                return (HttpStatusCode.ServiceUnavailable, "The service is temporarily unavailable.");
            default:
                return (HttpStatusCode.BadGateway, "An error occurred communicating with an upstream service.");
        }
    }
}
