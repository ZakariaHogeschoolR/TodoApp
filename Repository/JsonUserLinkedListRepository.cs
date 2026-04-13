using System.Text.Json;
using Model;
public class JsonUserLinkedListRepository : IUserRepository
{
    private readonly string _filePath;

    public JsonUserLinkedListRepository(string filePath) => _filePath = filePath;

    public IMyCollection<Users> LoadUsers()
    {
        var linkedList = new LinkedList<Users>(); // Jouw User LinkedList

        if (!File.Exists(_filePath)) return linkedList;

        string json = File.ReadAllText(_filePath);
        var users = JsonSerializer.Deserialize<List<Users>>(json);

        if (users != null)
        {
            foreach (var user in users)
            {
                linkedList.Add(user);
            }
        }
        return linkedList;
    }

    public void SaveUsers(IMyCollection<Users> users)
    {
        var json = JsonSerializer.Serialize(users.ToArray(), new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }
}