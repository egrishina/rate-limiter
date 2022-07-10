namespace Route256.RateLimiter.Options;

public class RateLimitOptions
{
    public const string Name = "RateLimitOptions";

    public List<RateLimitRule> GeneralRules { get; set; } = new List<RateLimitRule>();
}