using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Episodes.HealthChecks;

public static class HealthCheckResponseWriter
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public static Task WriteAsync(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var response = new HealthCheckResponse(
            report.Status.ToString(),
            report.Entries.ToDictionary(
                entry => entry.Key,
                entry => new HealthCheckEntry(entry.Value.Status.ToString(), entry.Value.Description)));

        return context.Response.WriteAsync(JsonSerializer.Serialize(response, SerializerOptions));
    }

    private sealed record HealthCheckResponse(
        string Status,
        IReadOnlyDictionary<string, HealthCheckEntry> Checks);

    private sealed record HealthCheckEntry(
        string Status,
        string? Description);
}
