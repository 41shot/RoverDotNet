using RoverDotNet.Demo.Api.Products.Models;

namespace RoverDotNet.Demo.Api.Products.GraphQL;

public class ProductType : ObjectType<Product>
{
    protected override void Configure(IObjectTypeDescriptor<Product> descriptor)
    {
        descriptor.Name("Product");
        descriptor.ImplementsNode().IdField(p => p.Id).ResolveNode((ctx, id) => Task.FromResult<Product?>(null));

        descriptor
            .Field(p => p.Id)
            .Type<NonNullType<IdType>>();

        descriptor
            .Field(p => p.Name)
            .Type<NonNullType<StringType>>();

        descriptor
            .Field(p => p.Description)
            .Type<StringType>();

        descriptor
            .Field(p => p.Price)
            .Type<NonNullType<FloatType>>();

        descriptor
            .Field(p => p.Currency)
            .Type<NonNullType<StringType>>();

        descriptor
            .Field(p => p.Category)
            .Type<NonNullType<EnumType<ProductCategory>>>();

        descriptor
            .Field(p => p.InStock)
            .Type<NonNullType<BooleanType>>();

        descriptor
            .Field(p => p.StockQuantity)
            .Type<NonNullType<IntType>>();

        descriptor
            .Field(p => p.CreatedBy)
            .Type<UserType>();

        descriptor
            .Field(p => p.CreatedAt)
            .Type<NonNullType<StringType>>()
            .Resolve(ctx => ctx.Parent<Product>().CreatedAt.ToString("O"));

        descriptor
            .Field(p => p.UpdatedAt)
            .Type<NonNullType<StringType>>()
            .Resolve(ctx => ctx.Parent<Product>().UpdatedAt.ToString("O"));
    }
}

public class UserType : ObjectType<User>
{
    protected override void Configure(IObjectTypeDescriptor<User> descriptor)
    {
        descriptor.Name("User");
        descriptor.ImplementsNode().IdField(u => u.Id).ResolveNode((ctx, id) => Task.FromResult<User?>(null));

        descriptor
            .Field(u => u.Id)
            .Type<NonNullType<IdType>>();
    }
}
