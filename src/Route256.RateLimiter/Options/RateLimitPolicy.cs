namespace Route256.RateLimiter.Options;

public class RateLimitPolicy
{
    public string Ip { get; set; }
    public List<RateLimitRule> Rules { get; set; } = new List<RateLimitRule>();
}