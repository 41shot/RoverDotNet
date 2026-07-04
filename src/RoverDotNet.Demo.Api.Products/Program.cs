using RoverDotNet.Demo.Api.Products.Data;
using RoverDotNet.Demo.Api.Products.GraphQL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ProductRepository>();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddType<ProductType>()
    .AddType<UserType>();

var app = builder.Build();

app.MapGraphQL();

app.Run();
