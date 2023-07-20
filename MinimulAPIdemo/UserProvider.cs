namespace MinimulAPIdemo;
public class UserProvider : IUserProvider
{
    private readonly List<User> _users;

    public UserProvider()
    {
        // Initialize the list with some sample data
        _users = new List<User>
        {
            new User { Id = 1, Name = "jimit", Email = "jimitpatel9879@gmail.com", Mobile = "9016922326" }
        };
    }

    public IEnumerable<User> Get()
    {
        return _users;
    }

    public User Get(int id)
    {
        return _users.FirstOrDefault(u => u.Id == id);
    }

    public User Add(User user)
    {
        user.Id = _users.Count + 1;
        _users.Add(user);
        return user;
    }

    public User Update(User user)
    {
        var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
        if (existingUser != null)
        {
            existingUser.Name = user.Name;
            existingUser.Email = user.Email;
            existingUser.Mobile = user.Mobile;
        }
        return existingUser;
    }

    public bool Delete(int id)
    {
        var userToDelete = _users.FirstOrDefault(u => u.Id == id);
        if (userToDelete != null)
        {
            _users.Remove(userToDelete);
            return true;
        }
        return false;
    }
}

