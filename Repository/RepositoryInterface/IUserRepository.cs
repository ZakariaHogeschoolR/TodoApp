using System.Runtime.CompilerServices;
using Model;
public interface IUserRepository
{
    IMyCollection<Users> LoadTasks();
    void SaveTasks(IMyCollection<Users> users);
}