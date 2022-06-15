namespace Route256.RateLimiter.Models;

public class ClientRequestIdentity
{
    public string ClientIp { get; set; }
    public string Path { get; set; }
    public string HttpVerb { get; set; }
}