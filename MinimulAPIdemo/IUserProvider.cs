namespace MinimulAPIdemo;

public interface IUserProvider
{
    User Add(User user);
    bool Delete(int id);
    IEnumerable<User> Get();
    User Get(int id);
    User Update(User user);
}
