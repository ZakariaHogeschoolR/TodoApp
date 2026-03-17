using System.Runtime.CompilerServices;
using Model;

public interface IUserService
{
    public Users? CurrentUser { get; set; }
    IMyCollection<Users> GetAllUsers();
    void AddUser(string userName, string password, string repeatPassword);
    void UpdateUser(string password, string repeatPassword, int id);
    void RemoveUser(int id);
    bool CheckCredentials(string username, string password);

}