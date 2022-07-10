using Microsoft.AspNetCore.Mvc;
using Route256.RateLimiter.Demo.Models;
using Route256.RateLimiter.Demo.Storage;
using Route256.RateLimiter.Options;

namespace Route256.RateLimiter.Demo.Controllers;

[Route("products")]
public class ProductsController : Controller
{
    private readonly IProductCatalogStorage _storage;

    public ProductsController(IProductCatalogStorage storage)
    {
        _storage = storage;
    }
    
    [HttpGet] // https://localhost:7086/products
    public IActionResult GetAllProducts()
    {
        return Ok(_storage.GetAll());
    }
    
    [HttpGet("{id}")] // https://localhost:7086/products/1
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RateLimitRule(Endpoint = "products", Period = "10s", Limit = 1)]
    public IActionResult GetProduct(int id)
    {
        var product = _storage.GetById(id);
        return product is not null ? Ok(product) : NotFound();
    }
}