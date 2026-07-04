using RoverDotNet.Demo.Api.Users.Models;

namespace RoverDotNet.Demo.Api.Users.Data;

public class UserRepository
{
    private readonly List<User> _users = new()
    {
        new User
        {
            Id = "1",
            Username = "johndoe",
            Email = "john.doe@example.com",
            FirstName = "John",
            LastName = "Doe",
            CreatedAt = DateTime.UtcNow.AddMonths(-6),
            IsActive = true
        },
        new User
        {
            Id = "2",
            Username = "janesmithdev",
            Email = "jane.smith@example.com",
            FirstName = "Jane",
            LastName = "Smith",
            CreatedAt = DateTime.UtcNow.AddMonths(-3),
            IsActive = true
        },
        new User
        {
            Id = "3",
            Username = "bobbuilder",
            Email = "bob.builder@example.com",
            FirstName = "Bob",
            LastName = "Builder",
            CreatedAt = DateTime.UtcNow.AddMonths(-1),
            IsActive = true
        },
        new User
        {
            Id = "4",
            Username = "alicewonder",
            Email = "alice@example.com",
            FirstName = "Alice",
            LastName = "Wonderland",
            CreatedAt = DateTime.UtcNow.AddDays(-15),
            IsActive = false
        },
        new User
        {
            Id = "5",
            Username = "charliebrown",
            Email = "charlie.brown@example.com",
            FirstName = "Charlie",
            LastName = "Brown",
            CreatedAt = DateTime.UtcNow.AddDays(-7),
            IsActive = true
        }
    };

    public User? GetUserById(string id) => _users.FirstOrDefault(u => u.Id == id);

    public List<User> GetAllUsers() => _users;

    public User? GetMe() => _users.FirstOrDefault();
}
