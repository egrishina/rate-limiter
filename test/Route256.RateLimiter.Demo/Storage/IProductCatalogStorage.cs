using Route256.RateLimiter.Demo.Models;

namespace Route256.RateLimiter.Demo.Storage;

public interface IProductCatalogStorage
{
    List<Product> GetAll();
    Product GetById(int id);
}