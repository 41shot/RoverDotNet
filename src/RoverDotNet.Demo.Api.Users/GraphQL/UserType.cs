using RoverDotNet.Demo.Api.Users.Models;

namespace RoverDotNet.Demo.Api.Users.GraphQL;

public class UserType : ObjectType<User>
{
    protected override void Configure(IObjectTypeDescriptor<User> descriptor)
    {
        descriptor.Name("User");
        descriptor.ImplementsNode().IdField(u => u.Id).ResolveNode((ctx, id) => Task.FromResult<User?>(null));

        descriptor
            .Field(u => u.Id)
            .Type<NonNullType<IdType>>();

        descriptor
            .Field(u => u.Username)
            .Type<NonNullType<StringType>>();

        descriptor
            .Field(u => u.Email)
            .Type<NonNullType<StringType>>();

        descriptor
            .Field(u => u.FirstName)
            .Type<StringType>();

        descriptor
            .Field(u => u.LastName)
            .Type<StringType>();

        descriptor
            .Field(u => u.CreatedAt)
            .Type<NonNullType<StringType>>()
            .Resolve(ctx => ctx.Parent<User>().CreatedAt.ToString("O"));

        descriptor
            .Field(u => u.IsActive)
            .Type<NonNullType<BooleanType>>();
    }
}
