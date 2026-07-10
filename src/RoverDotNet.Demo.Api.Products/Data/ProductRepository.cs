using RoverDotNet.Demo.Api.Products.GraphQL;
using RoverDotNet.Demo.Api.Products.Models;

namespace RoverDotNet.Demo.Api.Products.Data;

public class ProductRepository
{
    private readonly List<Product> _products = new()
    {
        new Product
        {
            Id = "1",
            Name = "Wireless Mouse",
            Description = "Ergonomic wireless mouse with 2.4GHz connection",
            Price = 29.99,
            Currency = "USD",
            Category = ProductCategory.ELECTRONICS,
            InStock = true,
            StockQuantity = 150,
            CreatedBy = new User { Id = "1" },
            CreatedAt = DateTime.UtcNow.AddMonths(-3),
            UpdatedAt = DateTime.UtcNow.AddDays(-5)
        },
        new Product
        {
            Id = "2",
            Name = "Cotton T-Shirt",
            Description = "100% cotton, available in multiple colors",
            Price = 19.99,
            Currency = "USD",
            Category = ProductCategory.CLOTHING,
            InStock = true,
            StockQuantity = 200,
            CreatedBy = new User { Id = "2" },
            CreatedAt = DateTime.UtcNow.AddMonths(-2),
            UpdatedAt = DateTime.UtcNow.AddDays(-3)
        },
        new Product
        {
            Id = "3",
            Name = "Programming in C#",
            Description = "Comprehensive guide to C# programming",
            Price = 49.99,
            Currency = "USD",
            Category = ProductCategory.BOOKS,
            InStock = true,
            StockQuantity = 75,
            CreatedBy = new User { Id = "1" },
            CreatedAt = DateTime.UtcNow.AddMonths(-4),
            UpdatedAt = DateTime.UtcNow.AddDays(-10)
        },
        new Product
        {
            Id = "4",
            Name = "Coffee Maker",
            Description = "12-cup programmable coffee maker with thermal carafe",
            Price = 89.99,
            Currency = "USD",
            Category = ProductCategory.HOME,
            InStock = true,
            StockQuantity = 45,
            CreatedBy = new User { Id = "3" },
            CreatedAt = DateTime.UtcNow.AddMonths(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-2)
        },
        new Product
        {
            Id = "5",
            Name = "Yoga Mat",
            Description = "Non-slip exercise yoga mat, 6mm thick",
            Price = 24.99,
            Currency = "USD",
            Category = ProductCategory.SPORTS,
            InStock = true,
            StockQuantity = 120,
            CreatedBy = new User { Id = "2" },
            CreatedAt = DateTime.UtcNow.AddDays(-45),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        },
        new Product
        {
            Id = "6",
            Name = "Building Blocks Set",
            Description = "500-piece creative building blocks for kids",
            Price = 34.99,
            Currency = "USD",
            Category = ProductCategory.TOYS,
            InStock = false,
            StockQuantity = 0,
            CreatedBy = new User { Id = "1" },
            CreatedAt = DateTime.UtcNow.AddMonths(-5),
            UpdatedAt = DateTime.UtcNow.AddDays(-7)
        },
        new Product
        {
            Id = "7",
            Name = "Organic Granola",
            Description = "Crunchy organic granola with nuts and honey",
            Price = 8.99,
            Currency = "USD",
            Category = ProductCategory.FOOD,
            InStock = true,
            StockQuantity = 300,
            CreatedBy = new User { Id = "4" },
            CreatedAt = DateTime.UtcNow.AddDays(-20),
            UpdatedAt = DateTime.UtcNow
        },
        new Product
        {
            Id = "8",
            Name = "Laptop Stand",
            Description = "Adjustable aluminum laptop stand for better ergonomics",
            Price = 39.99,
            Currency = "USD",
            Category = ProductCategory.ELECTRONICS,
            InStock = true,
            StockQuantity = 85,
            CreatedBy = new User { Id = "5" },
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            UpdatedAt = DateTime.UtcNow.AddDays(-4)
        }
    };

    public Product? GetProductById(string id) => _products.FirstOrDefault(p => p.Id == id);

    public List<Product> GetProducts(int limit = 10, int offset = 0)
    {
        return _products.Skip(offset).Take(limit).ToList();
    }

    public List<Product> GetProductsByCategory(ProductCategory category)
    {
        return _products.Where(p => p.Category == category).ToList();
    }
}
