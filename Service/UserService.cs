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
        _users = _repository.LoadTasks(); 
    }

    public IMyCollection<Users> GetAllUsers() => _users;
    public void AddUser(string userName, string password, string repeatPassword)
    {
        int id = _users.Count ;
        Users newUser = new Users
        {
            Id = _users.Count + 1, 
            Name = userName,
            Password = password,
            RepeatPassword = repeatPassword
        };
        var userArray = _users.ToArray();
        if (newUser == null) return;
        Users[] newArray = new Users[_users.Count + 1];
        for(int i = 0; i < _users.Count; i++)
        {
            newArray[i] = userArray[i];
        }
        newUser.Id = _users.Count + 1;
        for(int i = 0; i < _users.Count; i++)
        {
            if(userArray[i] == newUser)
            {
                return;
            }
        }
        newArray[_users.Count] = newUser; // place new item at end
        _users.Count += 1; // increment after
        _users.Dirty = true;
        _users.Add(newArray);
        _repository.SaveTasks(_users);
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
        var userArray = _users.ToArray();
        for(int i = 0; i < _users.Count; i++)
        {
            if(userArray[i] == user)
            {
                userArray[i] = newUser;
            }
        }
        _users.Dirty = true;
        _users.Update(userArray);
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
        var userArray = _users.ToArray();
        for(int i = 0; i < _users.Count; i++)
        {
            if(userArray[i] == user)
            {
                userArray[i] = newUser;
            }
        }
        _users.Dirty = true;
        _users.Update(userArray);
        _repository.SaveTasks(_users);
    }

    public void RemoveUser(int id)
    {
        var user = _users.FindBy(id, (t, id) => t.Id == id);
        if(user != null)
        {
            int index = -1;
            var userArray = _users.ToArray();
            for (int i = 0; i < _users.Count + 1; i++)
            {
                if (userArray[i] == user)
                {
                    index = i;
                    break;
                }
            }
            if(index == -1) return;

            Users[] newArray = new Users[_users.Count];
            
            for (int i = 0, j = 0; i < _users.Count; i++)
            {
                if (i == index) continue; // overslaan
                newArray[j] = userArray[i];
                j++;
            }
            userArray = newArray;
            _users.Dirty = false;
            _users.Remove(userArray);
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