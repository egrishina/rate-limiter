namespace Route256.RateLimiter.Models;

public struct ClientStatistics
{
    public DateTime Timestamp { get; set; }
    public double Count { get; set; }
}