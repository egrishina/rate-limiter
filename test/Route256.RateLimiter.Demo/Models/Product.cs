namespace Route256.RateLimiter.Demo.Models;

public class Product
{
    public Product(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; }
    public string Name { get; }
}