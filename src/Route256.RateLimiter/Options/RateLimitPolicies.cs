namespace Route256.RateLimiter.Options;

public class RateLimitPolicies
{
    public const string Name = "RateLimitPolicies";
    
    public List<RateLimitPolicy> IpRules { get; set; } = new List<RateLimitPolicy>();
}