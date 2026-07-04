using RoverDotNet.Demo.Api.Users.Data;
using RoverDotNet.Demo.Api.Users.Models;

namespace RoverDotNet.Demo.Api.Users.GraphQL;

public class Query
{
    public User? GetUser([Service] UserRepository repository, [ID] string id)
    {
        return repository.GetUserById(id);
    }

    public List<User> GetUsers([Service] UserRepository repository)
    {
        return repository.GetAllUsers();
    }

    public User? GetMe([Service] UserRepository repository)
    {
        return repository.GetMe();
    }
}
