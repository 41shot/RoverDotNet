using RoverDotNet.Demo.Api.Users.Data;
using RoverDotNet.Demo.Api.Users.GraphQL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<UserRepository>();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddType<UserType>();

var app = builder.Build();

app.MapGraphQL();

app.Run();
