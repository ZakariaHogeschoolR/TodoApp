using System.Text.Json;
using Microsoft.VisualBasic;
using System.Diagnostics.CodeAnalysis;
using Model;

public class JsonUserRepository: IUserRepository 
{
    private readonly string _filePath;
    public JsonUserRepository(string filePath) => _filePath = filePath;
    public IMyCollection<Users> LoadUsers()
    {
        if(!File.Exists(_filePath))
        {
            return new Array<Users>(1, new Users[0]);
        }
        string json = File.ReadAllText(_filePath);
        var users = JsonSerializer.Deserialize<Users[]>(json);
        return new Array<Users>(1, users) ?? new Array<Users>(1,new Users[users.Length]);
    }
    public void SaveUsers(IMyCollection<Users> users)
    {
        var array = users.ToArray();
        string json = JsonSerializer.Serialize(array, new
            JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json); 
    }
}