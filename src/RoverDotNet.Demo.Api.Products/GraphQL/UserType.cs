using HotChocolate.ApolloFederation.Types;

namespace RoverDotNet.Demo.Api.Products.GraphQL;

public class UserType : ObjectType<User>
{
    protected override void Configure(IObjectTypeDescriptor<User> descriptor)
    {
        descriptor.Name("User");
        descriptor.Key("id");

        descriptor
            .Field(u => u.Id)
            .Type<NonNullType<IdType>>();
    }
}
