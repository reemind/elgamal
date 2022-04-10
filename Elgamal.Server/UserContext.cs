using System.Collections.Concurrent;

namespace Elgamal.Server;

public class UserContext
{
    public BlockingCollection<User> Users { get; } = new();

    public void Add(User user)
    {
        Users.TryAdd(user);
    }
}