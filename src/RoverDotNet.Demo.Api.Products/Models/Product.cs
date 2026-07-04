namespace RoverDotNet.Demo.Api.Products.Models;

public class Product
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public double Price { get; set; }
    public string Currency { get; set; } = "USD";
    public ProductCategory Category { get; set; }
    public bool InStock { get; set; }
    public int StockQuantity { get; set; }
    public User? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class User
{
    public string Id { get; set; } = string.Empty;
}

public enum ProductCategory
{
    ELECTRONICS,
    CLOTHING,
    BOOKS,
    HOME,
    SPORTS,
    TOYS,
    FOOD,
    OTHER
}

public class CreateProductInput
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public double Price { get; set; }
    public string Currency { get; set; } = "USD";
    public ProductCategory Category { get; set; }
    public int StockQuantity { get; set; }
}

public class UpdateProductInput
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public double? Price { get; set; }
    public string? Currency { get; set; }
    public ProductCategory? Category { get; set; }
    public bool? InStock { get; set; }
    public int? StockQuantity { get; set; }
}
