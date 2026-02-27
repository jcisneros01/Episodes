namespace Episodes.Clients;

public sealed class TmdbApiException : HttpRequestException
{
    public TmdbApiException(int statusCode, string requestUri, string responseBody)
        : base($"TMDb request failed with status code {statusCode}.")
    {
        StatusCode = statusCode;
        RequestUri = requestUri;
        ResponseBody = responseBody;
    }

    public int StatusCode { get; }

    public string RequestUri { get; }

    public string ResponseBody { get; }
}