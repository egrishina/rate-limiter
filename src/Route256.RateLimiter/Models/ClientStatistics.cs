namespace Route256.RateLimiter.Models;

public class ClientStatistics
{
    public ClientStatistics()
    {
        Timestamp = DateTime.UtcNow;
        Count = 0;
    }
    
    public DateTime Timestamp { get; }
    public int Count { get; private set; }

    public void IncrementCounter()
    {
        Count++;
    }
}