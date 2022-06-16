namespace Route256.RateLimiter.Models;

public class ClientRequestIdentity
{
    public string ClientIp { get; init; }
    public string Path { get; init; }
}