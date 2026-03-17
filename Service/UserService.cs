using System.Security.Cryptography.X509Certificates;
using System.Diagnostics.CodeAnalysis;

public class UserService: IUserService
{
    private Users? _currentUser =  null;
    private static int _idCount = 0;
    public static int IdCount
    {
        get
        {
            return _idCount;
        }
        set
        {
            _idCount += value;
        }
    }
    private readonly IUserRepository _repository;
    private readonly IMyCollection<Users> _users;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
        _users = _repository.LoadTasks(); 
    }

    public IMyCollection<Users> GetAllUsers() => _users;
    public void AddUser(string userName, string password, string repeatPassword)
    {
        int id = _users.Count ;
        IdCount = 1;
        Users newUser = new Users
        {
            Id = _users.Count + 1, 
            Name = userName,
            Password = password,
            RepeatPassword = repeatPassword
        };
        var userArray = (UsersArray<Users>)_users;
        userArray.Add(newUser);
        _repository.SaveTasks(userArray);
    }

    public void UpdateUser(string password, string repeatPassword, int id)
    {
        var user = _users.FindBy(id, (t, id) => t.Id == id);
        Users newUser = new Users
        {
            Id = _users.Count + 1, 
            Name = user.Name,
            Password = password,
            RepeatPassword = repeatPassword
        };
        _users.Update(newUser, user);
        _repository.SaveTasks(_users);
    }

    public void UpdateUser(string name, int id)
    {
        var user = _users.FindBy(id, (t, id) => t.Id == id);
        Users newUser = new Users
        {
            Id = _users.Count + 1, 
            Name = name,
            Password = user.Password,
            RepeatPassword = user.RepeatPassword
        };
        _users.Update(newUser, user);
        _repository.SaveTasks(_users);
    }

    public void RemoveUser(int id)
    {
        var user = _users.FindBy(id, (t, id) => t.Id == id);
        if(user != null)
        {
            _users.Remove(user);
            _repository.SaveTasks(_users);
        }
    }

    public bool CheckCredentials(string username, string password)
    {
        Users[] users = _users.ToArray();
        for(int i = 0; i < users.Length; i++)
        {
            if(users[i] == null)
            {
                continue;
            }
            if(users[i].Name == username && users[i].Password == password )
            {
                _currentUser = users[i];
                return true;
            }
        }
        return false;
    }
}