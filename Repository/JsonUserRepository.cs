using System.Text.Json;
using Microsoft.VisualBasic;
using System.Diagnostics.CodeAnalysis;
using Model;

public class JsonUserRepository: IUserRepository 
{
    private readonly string _filePath;
    public JsonUserRepository(string filePath) => _filePath = filePath;
    public IMyCollection<Users> LoadTasks()
    {
        if(!File.Exists(_filePath))
        {
            return new UsersArray<Users>(new Users[0]);
        }
        string json = File.ReadAllText(_filePath);
        var users = JsonSerializer.Deserialize<Users[]>(json);
        return new UsersArray<Users>(users) ?? new UsersArray<Users>(new Users[users.Length]);
    }
    public void SaveTasks(IMyCollection<Users> users)
    {
        var array = users.ToArray();
        string json = JsonSerializer.Serialize(array, new
            JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json); 
    }
}