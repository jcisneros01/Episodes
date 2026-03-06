using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Episodes.HealthChecks;

public static class HealthCheckResponseWriter
{
    private static readonly JsonSerializerOptions SerializerOptions
        = new JsonSerializerOptions
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

    public static Task WriteAsync(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var response = new HealthCheckResponse(
            report.Status.ToString(),
            report.Entries.ToDictionary(
                entry => entry.Key,
                entry => new HealthCheckEntry(entry.Value.Status.ToString(), entry.Value.Description,
                    entry.Value.Duration.ToString())));

        return context.Response.WriteAsync(JsonSerializer.Serialize(response, SerializerOptions));
    }

    private sealed record HealthCheckResponse(
        string Status,
        IReadOnlyDictionary<string, HealthCheckEntry> Checks);

    private sealed record HealthCheckEntry(
        string Status,
        string? Description,
        string Duration);
}
