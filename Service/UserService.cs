using System.Security.Cryptography.X509Certificates;
using System.Diagnostics.CodeAnalysis;

public class UserService: IUserService
{
    private Users? _currentUser =  null;
    public Users? CurrentUser
    {
        get
        {
            return _currentUser;
        }    
        set
        {
            _currentUser = value;
        }
    }
    private static int _idCountUser = 0;
    public static int IdCountUser
    {
        get
        {
            return _idCountUser;
        }
        set
        {
            _idCountUser += value;
        }
    }
    private readonly IUserRepository _repository;
    private readonly IMyCollection<Users> _users;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
        _users = _repository.LoadUsers(); 
    }

    public IMyCollection<Users> GetAllUsers() => _users;
    public void AddUser(string userName, string password, string repeatPassword)
    {
        var userArray = _users.ToArray();
        bool duplicate = false;
        for(int i = 0; i < _users.Count; i++)
        {
            if(userName == userArray[i].Name)
            {
                duplicate = true;  
            }
        }
        if(password == repeatPassword || duplicate)
        {
            int id = _users.Count ;
            Users newUser = new Users
            {
                Id = _users.Count + 1, 
                Name = userName,
                Password = password,
                RepeatPassword = repeatPassword
            };
            _users.Add(newUser);
            _repository.SaveUsers(_users);
        }
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
        _users.Update(user, newUser);
        _repository.SaveUsers(_users);
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
        _users.Update(user, newUser);
        _repository.SaveUsers(_users);
    }

    public void RemoveUser(int id)
    {
        var user = _users.FindBy(id, (u, id) => u.Id == id);
        if (user != null)
        {
            _users.Remove(user); // Verwijdert direct het item en krimpt met 1
            _repository.SaveUsers(_users); // Let op: SaveUsers ipv SaveTasks
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
            if(users[i].Name == username && users[i].Password == password)
            {
                _currentUser = users[i];
                return true;
            }
        }
        return false;
    }
}