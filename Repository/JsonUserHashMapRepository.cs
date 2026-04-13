using System.Text.Json;
using Model;

public class JsonUserHashMapRepository : IUserRepository
{
    private readonly string _filePath;

    public JsonUserHashMapRepository(string filePath) => _filePath = filePath;

    public IMyCollection<Users> LoadUsers()
    {
        // We maken een HashMap aan met 'int' als Key type en 'Users' als Value type
        var hashMap = new HashMap<int, Users>(); 

        if (!File.Exists(_filePath)) return hashMap;

        string json = File.ReadAllText(_filePath);
        var users = JsonSerializer.Deserialize<List<Users>>(json);

        if (users != null)
        {
            foreach (var user in users)
            {
                // De HashMap gebruikt intern user.Id om de bucket te bepalen
                hashMap.Add(user);
            }
        }
        return hashMap;
    }

    public void SaveUsers(IMyCollection<Users> users)
    {
        // Dankzij jouw ToArray() methode in de HashMap werkt dit direct!
        var json = JsonSerializer.Serialize(users.ToArray(), new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }
}