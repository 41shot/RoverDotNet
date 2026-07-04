using RoverDotNet.Demo.Api.Users.Models;

namespace RoverDotNet.Demo.Api.Users.GraphQL;

public class Mutation
{
    public User CreateUser(CreateUserInput input)
    {
        return new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = input.Username,
            Email = input.Email,
            FirstName = input.FirstName,
            LastName = input.LastName,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
    }

    public User UpdateUser([ID] string id, UpdateUserInput input)
    {
        return new User
        {
            Id = id,
            Username = input.Username ?? "updated",
            Email = input.Email ?? "updated@example.com",
            FirstName = input.FirstName,
            LastName = input.LastName,
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            IsActive = input.IsActive ?? true
        };
    }

    public bool DeleteUser([ID] string id)
    {
        return true;
    }
}
