using RoverDotNet.Demo.Api.Products.Data;
using RoverDotNet.Demo.Api.Products.Models;

namespace RoverDotNet.Demo.Api.Products.GraphQL;

public class Query
{
    public Product? GetProduct([Service] ProductRepository repository, [ID] string id)
    {
        return repository.GetProductById(id);
    }

    public List<Product> GetProducts([Service] ProductRepository repository, int limit = 10, int offset = 0)
    {
        return repository.GetProducts(limit, offset);
    }

    public List<Product> GetProductsByCategory([Service] ProductRepository repository, ProductCategory category)
    {
        return repository.GetProductsByCategory(category);
    }
}
