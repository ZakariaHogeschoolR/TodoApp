using System.Runtime.CompilerServices;
using Model;
public interface IUserRepository
{
    IMyCollection<Users> LoadUsers();
    void SaveUsers(IMyCollection<Users> users);
}