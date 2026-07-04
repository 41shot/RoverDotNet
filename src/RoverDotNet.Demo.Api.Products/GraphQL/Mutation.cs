using RoverDotNet.Demo.Api.Products.Models;

namespace RoverDotNet.Demo.Api.Products.GraphQL;

public class Mutation
{
    public Product CreateProduct(CreateProductInput input)
    {
        return new Product
        {
            Id = Guid.NewGuid().ToString(),
            Name = input.Name,
            Description = input.Description,
            Price = input.Price,
            Currency = input.Currency,
            Category = input.Category,
            InStock = input.StockQuantity > 0,
            StockQuantity = input.StockQuantity,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public Product UpdateProduct([ID] string id, UpdateProductInput input)
    {
        return new Product
        {
            Id = id,
            Name = input.Name ?? "Updated Product",
            Description = input.Description,
            Price = input.Price ?? 0,
            Currency = input.Currency ?? "USD",
            Category = input.Category ?? ProductCategory.OTHER,
            InStock = input.InStock ?? true,
            StockQuantity = input.StockQuantity ?? 0,
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            UpdatedAt = DateTime.UtcNow
        };
    }

    public bool DeleteProduct([ID] string id)
    {
        return true;
    }
}
