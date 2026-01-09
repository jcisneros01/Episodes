using System.Text.Json;

namespace Episodes.Extensions;

public static class HttpResponseMessageExtensions
{
    private static readonly JsonSerializerOptions SnakeCaseOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true
    };

    public static async Task<T?> DeserializeJsonAsync<T>(
        this HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        return await JsonSerializer.DeserializeAsync<T>(
            stream,
            SnakeCaseOptions,
            cancellationToken);
    }
}