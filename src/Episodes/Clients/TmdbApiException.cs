using System.Net;

namespace Episodes.Clients;

public sealed class TmdbApiException : HttpRequestException
{
    public TmdbApiException(HttpStatusCode statusCode)
        : base($"TMDb API responded with {(int)statusCode} {statusCode}.", null, statusCode) { }
}
