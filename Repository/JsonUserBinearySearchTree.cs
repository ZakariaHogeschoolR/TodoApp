using System.Text.Json;
using Model;
public class JsonUserBinearySearchTreeRepository : IUserRepository
{
    private readonly string _filePath;

    public JsonUserBinearySearchTreeRepository(string filePath) => _filePath = filePath;

    public IMyCollection<Users> LoadUsers()
    {
        var tree = new BinearySearchTree<Users>(); // Jouw User LinkedList

        if (!File.Exists(_filePath)) return tree;

        string json = File.ReadAllText(_filePath);
        var users = JsonSerializer.Deserialize<List<Users>>(json);

        if (users != null)
        {
            foreach (var user in users)
            {
                tree.Add(user);
            }
        }
        return tree;
    }

    public void SaveUsers(IMyCollection<Users> users)
    {
        var json = JsonSerializer.Serialize(users.ToArray(), new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }
}