using Route256.RateLimiter.Demo.Models;

namespace Route256.RateLimiter.Demo.Storage;

public class ProductCatalogStorage : IProductCatalogStorage
{
    private readonly Dictionary<int, Product> _products = new();

    public ProductCatalogStorage()
    {
        InitializeProductStore();
    }

    public List<Product> GetAll()
    {
        return _products.Values.ToList();
    }

    public Product GetById(int id)
    {
        if (!_products.TryGetValue(id, out var value))
        {
            return null;
        }
        
        return value;
    }

    private void InitializeProductStore()
    {
        for (int i = 1; i <= 10; i++)
        {
            _products.Add(i, new Product(i, $"Product {i}"));
        }
    }
}