using System.Text.Json;

namespace Episodes.Extensions;

public static class HttpResponseMessageExtensions
{
    private static readonly JsonSerializerOptions SnakeCaseOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true
    };

    public static async Task<T> DeserializeJsonAsync<T>(
        this HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        var content = await JsonSerializer.DeserializeAsync<T>(
            stream,
            SnakeCaseOptions,
            cancellationToken);
        if (content == null)
        {
            throw new InvalidOperationException("TMDb returned an empty or invalid JSON payload.");
        }
        
        return content;
    }
}